using System;
using System.Collections.Generic;
using System.Linq;

namespace WebcamCapture.Plugin.Internal
{
    internal class InterceptorRegistry : IInterceptorRegistry
    {
        private static readonly List<Type> knownInterceptorTypes = InitInterceptorTypes();

        private static List<Type> InitInterceptorTypes()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetTypes()
                .Where(t => t.IsInterface)
                .Where(t => t != typeof(IInterceptor))
                .Where(t => typeof(IInterceptor).IsAssignableFrom(t)).ToList();
        }

        private readonly Dictionary<Type, List<IInterceptor>> interceptors = new Dictionary<Type, List<IInterceptor>>();

        #region Implementation of IInterceptorRegistry

        /// <summary>
        /// Registers the interceptor. Mutiple interceptors of the same type can
        /// be registered. They will be executed in registration order.
        /// </summary>
        /// <param name="interceptor">Interceptor to register.</param>
        public void RegisterInterceptor(IInterceptor interceptor)
        {
            List<IInterceptor> list;
            var interceptorType = GetInterceptorType(interceptor.GetType());
            if (!interceptors.TryGetValue(interceptorType, out list))
            {
                list = new List<IInterceptor>();
                interceptors[interceptorType] = list;
            }

            list.Add(interceptor);
        }

        /// <summary>
        /// Gets the interceptors of given type.
        /// </summary>
        /// <typeparam name="T">Interceptor interface type.</typeparam>
        /// <returns>Interceptors' enumeration.</returns>
        public IEnumerable<T> GetInterceptors<T>() where T : IInterceptor
        {
            var interceptorType = GetInterceptorType(typeof(T));
            List<IInterceptor> list;
            return interceptors.TryGetValue(interceptorType, out list) ? list.Cast<T>() : Enumerable.Empty<T>();
        }

        private static Type GetInterceptorType(Type type)
        {
            return knownInterceptorTypes.Where(t => t.IsAssignableFrom(type)).SingleOrDefault();
        }

        #endregion
    }
}