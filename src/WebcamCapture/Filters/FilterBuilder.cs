using System;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace WebcamCapture.Filters
{
    internal class FilterBuilder : IFilterBuilder
    {
        private const int SampleGrabberCallbackMethod = 0;

        private IGraphBuilder graphBuilder;
        private readonly IFilter filter;
        private ISampleGrabber sampleGrabber;

        public FilterBuilder(IFilter filter)
        {
            this.filter = filter;
        }

        /// <summary>
        /// Initializes the filter builder.
        /// </summary>
        /// <param name="graphBuilder">The graph builder.</param>
        public void Init(IGraphBuilder graphBuilder)
        {
            this.graphBuilder = graphBuilder;
        }

        /// <summary>
        /// Creates the filter.
        /// </summary>
        /// <returns>New filter instance.</returns>
        public IBaseFilter Build()
        {
            sampleGrabber = (ISampleGrabber) new SampleGrabber();
            ConfigSampleGrabber(sampleGrabber);

            sampleGrabber.SetCallback(filter, SampleGrabberCallbackMethod);

            return (IBaseFilter) sampleGrabber;
        }

        private static void ConfigSampleGrabber(ISampleGrabber sb)
        {
            // set the media type
            var media = new AMMediaType
                {
                    majorType = MediaType.Video,
                    subType = MediaSubType.RGB24,
                    formatType = FormatType.VideoInfo
                };

            // that's the call to the ISampleGrabber interface
            sb.SetMediaType(media);

            DsUtils.FreeAMMediaType(media);
        }

        /// <summary>
        /// Performs additional configuration after "Render..." method call if necessary.
        /// </summary>
        public void Configure()
        {
            var bv = (IBasicVideo) graphBuilder;
            int videoWidth, videoHeight;
            bv.GetVideoSize(out videoWidth, out videoHeight);

            int videoStride = GetStride(videoWidth);
            filter.VideoHeight = videoHeight;
            filter.VideoStride = videoStride;
        }

        //
        // retrieve the bitmap stride (the offset from one row of pixel to the next)
        //
        private int GetStride(int videoWidth)
        {
            var media = new AMMediaType();

            // GetConnectedMediaType retrieve the media type for a sample
            var hr = sampleGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if (media.formatType != FormatType.VideoInfo || media.formatPtr == IntPtr.Zero)
            {
                throw new Exception("Format type incorrect");
            }

            // save the stride
            var videoInfoHeader = (VideoInfoHeader) Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            int videoStride = videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);

            return videoStride;
        }
    }
}
