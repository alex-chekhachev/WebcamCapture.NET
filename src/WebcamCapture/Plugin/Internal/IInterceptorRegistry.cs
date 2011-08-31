using System.Collections.Generic;

namespace WebcamCapture.Plugin.Internal
{
    /// <summary>
    /// Pipeline interceptors registry.
    /// </summary>
    internal interface IInterceptorRegistry
    {
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">Interceptor to register.</param>
        void RegisterInterceptor(IInterceptor interceptor);

        /// <summary>
        /// Gets the interceptors of given type.
        /// </summary>
        /// <typeparam name="T">Interceptor interface type.</typeparam>
        /// <returns>Interceptors' enumeration.</returns>
        IEnumerable<T> GetInterceptors<T>() where T : IInterceptor;
    }
}