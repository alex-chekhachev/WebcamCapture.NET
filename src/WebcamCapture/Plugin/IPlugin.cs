using System.Collections.Generic;

namespace WebcamCapture.Plugin
{
    /// <summary>
    /// Interface for WebcamCapture.NET plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Allows plugin to initialize it's UI - add menu items etc.
        /// </summary>
        /// <param name="host">Application's UI host.</param>
        void InitUI(IUIHost host);

        /// <summary>
        /// Gets interceptors, provided by the plugin.
        /// </summary>
        /// <returns>Plugin's interceptors.</returns>
        IEnumerable<IInterceptor> GetInterceptors();
    }
}