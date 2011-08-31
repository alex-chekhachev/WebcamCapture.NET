using DirectShowLib;

namespace WebcamCapture
{
    internal interface IFilterBuilder
    {
        /// <summary>
        /// Initializes the filter builder.
        /// </summary>
        /// <param name="graphBuilder">The graph builder.</param>
        void Init(IGraphBuilder graphBuilder);

        /// <summary>
        /// Creates the filter.
        /// </summary>
        /// <returns>New filter instance.</returns>
        IBaseFilter Build();

        /// <summary>
        /// Performs additional configuration after "Render..." method call if necessary.
        /// </summary>
        void Configure();
    }
}