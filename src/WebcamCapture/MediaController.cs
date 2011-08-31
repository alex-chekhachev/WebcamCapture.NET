using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;
using WebcamCapture.Filters;
using WebcamCapture.Helpers;
using WebcamCapture.Properties;

namespace WebcamCapture
{
    internal class MediaController : IDisposable
    {
        /// <summary>
        /// Application-defined message to notify app of filtergraph events
        /// </summary>
        public const int WM_GRAPHNOTIFY = 0x8000 + 1;

        public event EventHandler<EventArgs<string>> FormatChanged = delegate { };
        public event EventHandler<EventArgs<string>> VideoWindowSizeChanged = delegate { };
        public event EventHandler<EventArgs<Func<string>>> FpsChanged = delegate { };

        private readonly IntPtr ownerHandle;
        private DsDevice currentDevice;

        // a small enum to record the graph state
        private enum PlayState
        {
            Stopped, Paused, Running, Init
        }

        private const string SourceFilterName = "Video Capture";

        private IVideoWindow videoWindow;
        private IMediaControl mediaControl;
        private IMediaEventEx mediaEventEx;
        private IGraphBuilder graphBuilder;
        private IQualProp qualProp;
        private ICaptureGraphBuilder2 captureGraphBuilder;
        private PlayState currentState = PlayState.Stopped;

        private DsROTEntry rot;
        private AMMediaType currentFormat;
        private readonly Control videoWindowControl;
        private Timer timer;

        private readonly FilterController filtersController;

        public MediaController(IntPtr ownerHandle, Control videoWindowControl, FilterController filtersController)
        {
            this.ownerHandle = ownerHandle;
            this.filtersController = filtersController;
            this.videoWindowControl = videoWindowControl;
        }

        public void Dispose()
        {
            timer.Stop();
            CloseInterfaces();

            DsUtils.FreeAMMediaType(currentFormat);
        }

        #region Utility functions

        public void HandleGraphEvent()
        {
            EventCode evCode;
            IntPtr evParam1, evParam2;

            if (mediaEventEx == null)
                return;

            while (mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free event parameters to prevent memory leaks associated with
                // event parameter data.  While this application is not interested
                // in the received events, applications should always process them.
                int hr = mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                DsError.ThrowExceptionForHR(hr);

                // Insert event processing code here, if desired
            }
        }

        private void GetInterfaces()
        {
            // An exception is thrown if cast fail
            graphBuilder = (IGraphBuilder)new FilterGraph();
            captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)graphBuilder;
            videoWindow = (IVideoWindow)graphBuilder;
            mediaEventEx = (IMediaEventEx)graphBuilder;

            int hr = mediaEventEx.SetNotifyWindow(ownerHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
        }

        private void CloseInterfaces()
        {
            // Stop previewing data
            if (mediaControl != null)
                mediaControl.StopWhenReady();

            currentState = PlayState.Stopped;

            // Stop receiving events
            if (mediaEventEx != null)
                mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);

            // Relinquish ownership (IMPORTANT!) of the video window.
            // Failing to call put_Owner can lead to assert failures within
            // the video renderer, as it still assumes that it has a valid
            // parent window.
            if (videoWindow != null)
            {
                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }

            // Remove filter graph from the running object table
            if (rot != null)
            {
                rot.Dispose();
                rot = null;
            }

            // Release DirectShow interfaces
            if (qualProp != null)
            {
                Marshal.ReleaseComObject(qualProp);
                qualProp = null;
            }
            if (mediaControl != null)
            {
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
            }
            if (mediaEventEx != null)
            {
                Marshal.ReleaseComObject(mediaEventEx);
                mediaEventEx = null;
            }
            if (videoWindow != null)
            {
                Marshal.ReleaseComObject(videoWindow);
                videoWindow = null;
            }
            if (graphBuilder != null)
            {
                Marshal.ReleaseComObject(graphBuilder);
                graphBuilder = null;
            }
            if (captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(captureGraphBuilder);
                captureGraphBuilder = null;
            }
        }

        public void NotifyVideoWindow(ref Message m)
        {
            if (videoWindow != null)
                videoWindow.NotifyOwnerMessage(m.HWnd, m.Msg, m.WParam, m.LParam);
        }

        private static bool HasStoredSettings()
        {
            return Settings.Default.Width + Settings.Default.Height + Settings.Default.Bpp > 0;
        }

        public void ChangePreviewState(bool showVideo)
        {
            // If the media control interface isn't ready, don't call it
            if (mediaControl == null) return;

            if (showVideo)
            {
                if (currentState != PlayState.Running)
                {
                    // Start previewing video data
                    mediaControl.Run();
                    currentState = PlayState.Running;
                }
            }
            else
            {
                // Stop previewing video data
                mediaControl.StopWhenReady();
                currentState = PlayState.Stopped;
            }
        }

        #endregion

        #region Device methods

        public static IBaseFilter FindCaptureDevice()
        {
            // Get all video input devices
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            // Take the first device
            var device = devices[0];

            // Bind Moniker to a filter object
            var iid = typeof(IBaseFilter).GUID;
            object source;
            device.Mon.BindToObject(null, null, ref iid, out source);

            // An exception is thrown if cast fail
            return (IBaseFilter) source;
        }

        public void SelectDevice(DsDevice device)
        {
            CloseInterfaces();
            currentDevice = device;
            CaptureVideo();
        }

        public void ResetDevice()
        {
            CloseInterfaces();
        }

        private IBaseFilter GetCaptureDevice()
        {
            // Bind Moniker to a filter object
            var iid = typeof(IBaseFilter).GUID;
            object source;
            currentDevice.Mon.BindToObject(null, null, ref iid, out source);

            // An exception is thrown if cast fail
            return (IBaseFilter)source;
        }

        #endregion

        private void CaptureVideo()
        {
            try
            {
                // Get DirectShow interfaces
                GetInterfaces();

                // Attach the filter graph to the capture graph
                int hr = captureGraphBuilder.SetFiltergraph(graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                // Use the system device enumerator and class enumerator to find
                // a video capture/preview device, such as a desktop USB video camera.
                //var sourceFilter = FindCaptureDevice();
                var sourceFilter = GetCaptureDevice();

                // Add Capture filter to our graph.
                hr = graphBuilder.AddFilter(sourceFilter, SourceFilterName);
                DsError.ThrowExceptionForHR(hr);

                if (currentFormat == null)
                {
                    if (HasStoredSettings())
                    {
                        SetFormat(sourceFilter, Settings.Default.Width, Settings.Default.Height, Settings.Default.Bpp);
                    }
                    else
                    {
                        ChooseFormat(sourceFilter);
                    }
                }
                else
                {
                    var config = (IAMStreamConfig) GetOutputPin(sourceFilter);
                    config.SetFormat(currentFormat);
                }
                BuildAndRunGraph(sourceFilter);

                StartTimer();
            }
            catch
            {
                MessageBox.Show(@"An unrecoverable error has occurred.");
            }
        }

        private void BuildAndRunGraph(IBaseFilter sourceFilter)
        {
            GetFormat(sourceFilter);

            IBaseFilter filter = AddFiltersToGraph();

            qualProp = (IQualProp) new VideoRenderer();
            graphBuilder.AddFilter((IBaseFilter) qualProp, "Video Renderer");

            int hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, sourceFilter, filter, (IBaseFilter) qualProp);
            DsError.ThrowExceptionForHR(hr);

            ConfigureFilters();

            // Now that the filter has been added to the graph and we have
            // rendered its stream, we can release this reference to the filter.
            Marshal.ReleaseComObject(sourceFilter);

            // Set video window style and position
            SetupVideoWindow();

            // Add our graph to the running object table, which will allow
            // the GraphEdit application to "spy" on our graph
            //rot = new DsROTEntry(graphBuilder);

            // Start previewing video data
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Remember current state
            currentState = PlayState.Running;
        }

        private void StartTimer()
        {
            timer = new Timer {Interval = 1000};
            timer.Tick += (s, e) => OnFpsUpdated();
            timer.Start();
        }

        private void OnFpsUpdated()
        {
            if (currentState != PlayState.Running) return;

            Func<string> callback = () =>
                {
                    double fps = 0;
                    if (qualProp != null)
                    {
                        try
                        {
                            int piAvgFrameRate;
                            qualProp.get_AvgFrameRate(out piAvgFrameRate);
                            fps = piAvgFrameRate / 100D;
                        }
                        catch
                        {
                        }
                    }
                    return string.Format("{0}", fps);
                };
            FpsChanged(this, new EventArgs<Func<string>>(callback));
        }

        #region Filter configuration

        private void ConfigureFilters()
        {
            filtersController.ConfigureFilters();
        }

        /// <summary>
        /// Adds registered filters to graph.
        /// </summary>
        /// <returns>Compressor filter if any.</returns>
        private IBaseFilter AddFiltersToGraph()
        {
            var filters = filtersController.InitFilters(graphBuilder);
            var results = filters.Select(filter => graphBuilder.AddFilter(filter, filter.GetType().Name));
            results.ForEach(DsError.ThrowExceptionForHR);

            //FIXME: use predefined filters for fixed extension stages: preview (overlay), capture, beforesave etc
            return filtersController.CompressorFilter;
        }

        public void DisableFilters()
        {
            ChangeGraph(_ => filtersController.Clear());
        }

        public void SetFilterBuilderType(Type type)
        {
            ChangeGraph(_ =>
                {
                    //!!!
//                    filterBuilders = null;
//                    filterBuilderTypes = type;
                });
        }

        private void ChangeGraph(Action<IBaseFilter> action)
        {
            currentState = PlayState.Stopped;
            IBaseFilter sourceFilter = GetSourceFilter();
            
            mediaControl.Stop();
            ResetGraph(sourceFilter);

            action(sourceFilter);

            BuildAndRunGraph(sourceFilter);
        }

        #endregion

        #region Format manipulation

        private void GetFormat(IBaseFilter filter)
        {
            var config = (IAMStreamConfig) GetOutputPin(filter);

            AMMediaType media;
            var hr = config.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            if (currentFormat != null)
            {
                DsUtils.FreeAMMediaType(currentFormat);
            }
            currentFormat = media;

            var resolutionInfo = ResolutionInfo.Create(currentFormat);

            Settings.Default.Width = resolutionInfo.Width;
            Settings.Default.Height = resolutionInfo.Height;
            Settings.Default.Bpp = resolutionInfo.Bpp;
            Settings.Default.Save();

            OnFormatChanged(resolutionInfo);
        }

        private void OnFormatChanged(ResolutionInfo resolutionInfo)
        {
            FormatChanged(this, new EventArgs<string>(resolutionInfo.ToString()));
        }

        private void ChooseFormat(IBaseFilter filter)
        {
            IPin pin = GetOutputPin(filter);

            object configObject = pin as IAMStreamConfig;
            var propertyPages = configObject as ISpecifyPropertyPages;
            if (propertyPages == null)
            {
                return;
            }

            FilterInfo filterInfo;
            int hr = filter.QueryFilterInfo(out filterInfo);
            DsError.ThrowExceptionForHR(hr);

            if (filterInfo.pGraph != null)
                Marshal.ReleaseComObject(filterInfo.pGraph);

            DsCAUUID caGuid;
            hr = propertyPages.GetPages(out caGuid);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                hr = NativeMethods.OleCreatePropertyFrame(
                    ownerHandle, 30, 30,
                    filterInfo.achName,
                    1, ref configObject,
                    caGuid.cElems, caGuid.pElems,
                    0, 0,
                    IntPtr.Zero
                    );
                DsError.ThrowExceptionForHR(hr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Marshal.FreeCoTaskMem(caGuid.pElems);
            }
        }

        private static IPin GetOutputPin(IBaseFilter filter)
        {
            return DsFindPin.ByDirection(filter, PinDirection.Output, 0);
        }

        private static void SetFormat(IBaseFilter source, int width, int height, int bpp)
        {
            var config = GetOutputPin(source) as IAMStreamConfig;
            if (config == null) return;

            int count, size;
            config.GetNumberOfCapabilities(out count, out size);
            var pointer = Marshal.AllocHGlobal(size);

            for (int iFormat = 0; iFormat < count; iFormat++)
            {
                AMMediaType mediaType;

                /* Note:  Use of the VIDEO_STREAM_CONFIG_CAPS structure to configure a video device is deprecated. 
                 * Although the caller must allocate the buffer, it should ignore the contents after the method 
                 * returns. The capture device will return its supported formats through the pmt parameter. */
                var hr = config.GetStreamCaps(iFormat, out mediaType, pointer);
                DsError.ThrowExceptionForHR(hr);

                /* Examine the format, and possibly use it. */
                if (mediaType.majorType == MediaType.Video && mediaType.formatType == FormatType.VideoInfo)
                {
                    var videoInfo = new VideoInfoHeader();
                    Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);

                    if (videoInfo.BmiHeader.Width == width &&
                        videoInfo.BmiHeader.Height == height &&
                        videoInfo.BmiHeader.BitCount == bpp)
                    {
                        hr = config.SetFormat(mediaType);
                        Marshal.ThrowExceptionForHR(hr);
                        break;
                    }
                }

                // Delete the media type when you are done.
                DsUtils.FreeAMMediaType(mediaType);
            }

            Marshal.FreeHGlobal(pointer);
        }

        private void ResetGraph(IBaseFilter source)
        {
            IEnumFilters enumFilters;
            var hr = graphBuilder.EnumFilters(out enumFilters);
            DsError.ThrowExceptionForHR(hr);

            var array = new IBaseFilter[1];
            var filters = new List<IBaseFilter>();
            while (enumFilters.Next(array.Length, array, IntPtr.Zero) == 0)
            {
                filters.Add(array[0]);
            }

            foreach (var filter in filters.Where(filter => filter != source))
            {
                hr = graphBuilder.RemoveFilter(filter);
                DsError.ThrowExceptionForHR(hr);
                Marshal.FinalReleaseComObject(filter);
                //                    while (Marshal.ReleaseComObject(filter) > 0)
                //                    {
                //                    }
            }

            Marshal.ReleaseComObject(enumFilters);
        }

        public void ChangeCameraFormat()
        {
            ChangeGraph(ChooseFormat);
        }

        private IBaseFilter GetSourceFilter()
        {
            IBaseFilter source;
            var hr = graphBuilder.FindFilterByName(SourceFilterName, out source);
            DsError.ThrowExceptionForHR(hr);

            return source;
        }

//        private void ShowActualFormat()
//        {
//            IBaseFilter source = GetSourceFilter();
//
//            var config = (IAMStreamConfig)GetOutputPin(source);
//            AMMediaType media = null;
//            try
//            {
//                config.GetFormat(out media);
//
//                var resolution = ResolutionInfo.Create(media);
//                MessageBox.Show(resolution.ToString(), "Current camera format");
//            }
//            finally
//            {
//                DsUtils.FreeAMMediaType(media);
//            }
//        }

        #endregion

        #region Video window

        private void SetupVideoWindow()
        {
            // Set the video window to be a child of the main window
            //int hr = videoWindow.put_Owner(Handle);
            int hr = videoWindow.put_Owner(videoWindowControl.Handle);
            DsError.ThrowExceptionForHR(hr);

            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);

            // Use helper function to position video window in client rect 
            // of main application window
            ResizeVideoWindow();

            // Make the video window visible, now that it is properly positioned
            hr = videoWindow.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);
        }

        public void ResizeVideoWindow()
        {
            // Resize the video preview window to match owner window size
            if (videoWindow != null)
            {
                //                videoWindow.SetWindowPosition(0, 0, ClientSize.Width, ClientSize.Height);
                videoWindow.SetWindowPosition(videoWindowControl.Left, videoWindowControl.Top, videoWindowControl.Width, videoWindowControl.Height);
            }

            OnVideoWindowSizeChanged();
        }

        private void OnVideoWindowSizeChanged()
        {
           VideoWindowSizeChanged(this, new EventArgs<string>(string.Format("{0}x{1}", videoWindowControl.Width, videoWindowControl.Height))); 
        }

        #endregion
    }
}
