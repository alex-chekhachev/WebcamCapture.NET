using System;
using System.Windows.Forms;
using DirectShowLib;
using WebcamCapture.Filters;
using WebcamCapture.Filters.Preprocessing;
using WebcamCapture.Plugin;
using WebcamCapture.Plugin.Internal;

namespace WebcamCapture
{
    public partial class MainForm : Form, IUIHost
    {
        private readonly MediaController controller;
        private ToolStripMenuItem currentDeviceItem;
        private PluginManager pluginManager;
        private readonly FilterController filtersController;
        private IInterceptorRegistry registry;

        public MainForm()
        {
            InitializeComponent();

            registry = new InterceptorRegistry();
            filtersController = new FilterController(registry);
            InitFilters();

            controller = new MediaController(Handle, panelVideo, filtersController);
            controller.FormatChanged += (s, e) => statusCurrentFormat.Text = e.Data;
            controller.VideoWindowSizeChanged += (s, e) => statusVideoWindowSize.Text = e.Data;
            controller.FpsChanged += (s, e) =>
                {
                    var action = new Action(() => statusFPS.Text = e.Data());
                    if (InvokeRequired)
                    {
                        Invoke(action);
                    }
                    else
                    {
                        action();
                    }
                };

            Resize += FormResize;
        }

        private void InitFilters()
        {
            filtersController.RegisterFilterType(typeof(PreprocessingFilter));
        }

        #region Overrides of Form

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                controller.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitDeviceList();
            SelectDefaultDevice();

            LoadPlugins();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MediaController.WM_GRAPHNOTIFY:
                    {
                        controller.HandleGraphEvent();
                        break;
                    }
            }

            // Pass this message to the video window for notification of system changes
            // Check for null is NECESSARY, because window proc is starting called at the same time
            // as constructor.
            if (controller != null)
                controller.NotifyVideoWindow(ref m);

            base.WndProc(ref m);
        }

        #endregion

        #region Initialization

        private void InitDeviceList()
        {
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            foreach (var device in devices)
            {
                var menuItem = (ToolStripMenuItem) devicesToolStripMenuItem.DropDownItems.Add(device.Name);
                menuItem.Tag = device;
                menuItem.CheckOnClick = true;
                menuItem.Click += SelectDeviceClick;
            }
        }

        private void SelectDefaultDevice()
        {
            if (devicesToolStripMenuItem.DropDownItems.Count > 0)
            {
                var item = (ToolStripMenuItem) devicesToolStripMenuItem.DropDownItems[0];
                item.PerformClick();
            }
        }

        private void LoadPlugins()
        {
            pluginManager = new PluginManager(this, registry);
            pluginManager.LoadPlugins();
        }

        #endregion

        #region Event handlers

        private void ExitClick(object sender, EventArgs e)
        {
            Close();
        }

        private void FormResize(object sender, EventArgs e)
        {
            // Stop graph when Form is iconic
            // Restart Graph when window come back to normal state or maxed state.
            controller.ChangePreviewState(WindowState != FormWindowState.Minimized);
            controller.ResizeVideoWindow();
        }

        private void SelectDeviceClick(object sender, EventArgs e)
        {
            if (currentDeviceItem != null)
                currentDeviceItem.Checked = false;

            var item = (ToolStripMenuItem) sender;
            if (item.Checked)
            {
                var device = (DsDevice) item.Tag;
                controller.SelectDevice(device);

                currentDeviceItem = item;
            }
            else
            {
                controller.ResetDevice();
                currentDeviceItem = null;
            }
        }

        #endregion

        private void ChangeCameraFormatClick(object sender, EventArgs e)
        {
            controller.ChangeCameraFormat();
        }

        #region Implementation of IUIHost

        public void AddMenuItem(UIExtensionSite site, ToolStripMenuItem item)
        {
            switch (site)
            {
                case UIExtensionSite.Options:
                    commandsToolStripMenuItem.DropDownItems.Add(item);
                    break;
            }
        }

        #endregion
    }
}
