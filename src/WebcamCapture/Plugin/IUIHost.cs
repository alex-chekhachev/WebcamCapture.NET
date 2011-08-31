using System.Windows.Forms;

namespace WebcamCapture.Plugin
{
    public interface IUIHost
    {
        void AddMenuItem(UIExtensionSite site, ToolStripMenuItem item);
    }
}