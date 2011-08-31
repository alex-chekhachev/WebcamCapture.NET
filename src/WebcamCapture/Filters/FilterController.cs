using System;
using System.Collections.Generic;
using System.Linq;
using DirectShowLib;
using WebcamCapture.Plugin.Internal;

namespace WebcamCapture.Filters
{
    internal class FilterController
    {
        private readonly IInterceptorRegistry registry;
        private readonly List<Type> filterTypes = new List<Type>();

        private readonly List<IFilterBuilder> filterBuilders = new List<IFilterBuilder>();

        public IBaseFilter CompressorFilter { get; private set; }

        public FilterController(IInterceptorRegistry registry)
        {
            this.registry = registry;
        }

        public void RegisterFilterType(Type type)
        {
            if (filterTypes.Contains(type))
            {
                var message = string.Format("Filter type {0} already registered", type);
                throw new InvalidOperationException(message);
            }
            filterTypes.Add(type);
        }

        public IEnumerable<IBaseFilter> InitFilters(IGraphBuilder graphBuilder)
        {
            filterBuilders.Clear();
            foreach (var filter in filterTypes.Select(CreateFilter))
            {
                var builder = new FilterBuilder(filter);
                filterBuilders.Add(builder);
                builder.Init(graphBuilder);
                var baseFilter = CompressorFilter = builder.Build();
                yield return baseFilter;
            }
        }

        private IFilter CreateFilter(Type filterType)
        {
            var filter = (IFilter) Activator.CreateInstance(filterType);
            filter.Interceptors = registry;
            return filter;
        }

        public void ConfigureFilters()
        {
            filterBuilders.ForEach(fb => fb.Configure());
        }

        public void Clear()
        {
            filterBuilders.Clear();
        }
    }
}