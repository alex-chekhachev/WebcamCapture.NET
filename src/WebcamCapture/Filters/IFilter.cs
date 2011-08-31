using DirectShowLib;
using WebcamCapture.Plugin.Internal;

namespace WebcamCapture.Filters
{
    internal interface IFilter : ISampleGrabberCB
    {
        int VideoHeight { get; set; }
        int VideoStride { get; set; }
        IInterceptorRegistry Interceptors { get; set; }
    }
}