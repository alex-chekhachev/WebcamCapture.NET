using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using WebcamCapture.Plugin;

namespace WebcamCapture.Plugins.Effects
{
    /// <summary>
    /// WebcamCapture.NET plugin: provides simple image processing effects.
    /// </summary>
    [Export(typeof(IPlugin))]
    internal class EffectsPlugin : IPlugin
    {
        private ToolStripMenuItem currentFilterItem;
        private EffectsInterceptor interceptor;

        public void InitUI(IUIHost host)
        {
            var noFilterItem = new ToolStripMenuItem("No Filter", null, OnNoFilterToolStripMenuItemClick)
                {CheckOnClick = true};
            host.AddMenuItem(UIExtensionSite.Options, noFilterItem);

            var negateItem = new ToolStripMenuItem("Negate", null, OnNegateToolStripMenuItemClick)
                {CheckOnClick = true};
            host.AddMenuItem(UIExtensionSite.Options, negateItem);

            currentFilterItem = noFilterItem;
            currentFilterItem.Checked = true;
        }

        /// <summary>
        /// Gets interceptors, provided by the plugin.
        /// </summary>
        /// <returns>Plugin's interceptors.</returns>
        public IEnumerable<IInterceptor> GetInterceptors()
        {
            yield return interceptor ?? (interceptor = new EffectsInterceptor());
        }

        #region Effects handling

        private void OnNoFilterToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeEffect(sender, () => interceptor.Disable());
        }

        private void ChangeEffect(object sender, Action action)
        {
            if (currentFilterItem != null)
                currentFilterItem.Checked = false;

            action();

            currentFilterItem = (ToolStripMenuItem) sender;
        }

        private void OnNegateToolStripMenuItemClick(object sender, EventArgs e)
        {
//            ChangeEffect(sender, () => controller.SetFilterBuilderType(typeof(FilterBuilder<EffectsInterceptor>)));
            ChangeEffect(sender, () => interceptor.SetEffect(EffectsInterceptor.Effect.Negate));
        }

        #endregion

    }
}