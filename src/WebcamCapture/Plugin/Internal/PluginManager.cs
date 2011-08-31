using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using WebcamCapture.Helpers;

namespace WebcamCapture.Plugin.Internal
{
    /// <summary>
    /// Responsible for plugins loading, initialization and event notification.
    /// </summary>
    internal class PluginManager
    {
        private const string PluginSearchPattern = "*plugin*";
        private readonly IUIHost host;
        private readonly IInterceptorRegistry registry;

        public PluginManager(IUIHost host, IInterceptorRegistry registry)
        {
            this.host = host;
            this.registry = registry;
        }

        public void LoadPlugins()
        {
            GetPlugins().ForEach(LoadPlugin);
        }

        private void LoadPlugin(IPlugin plugin)
        {
            plugin.InitUI(host);
            LoadPluginInterceptors(plugin);
        }

        private void LoadPluginInterceptors(IPlugin plugin)
        {
            var interceptors = plugin.GetInterceptors() ?? Enumerable.Empty<IInterceptor>();
            interceptors.ForEach(registry.RegisterInterceptor);
        }

        private static IEnumerable<IPlugin> GetPlugins()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
            var catalog = new DirectoryCatalog(path, PluginSearchPattern);
            var container = new CompositionContainer(catalog);
            return container.GetExportedValues<IPlugin>();
        }
    }
}