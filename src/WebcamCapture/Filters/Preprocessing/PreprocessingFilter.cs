using System;
using WebcamCapture.Helpers;
using WebcamCapture.Plugin;

namespace WebcamCapture.Filters.Preprocessing
{
    internal class PreprocessingFilter : FilterBase<IPreprocessingInterceptor>
    {
        protected override void Execute(IntPtr pBuffer, int videoHeight, int videoStride)
        {
            GetInterceptors().ForEach(x => x.Execute(pBuffer, videoHeight, videoStride));
        }
    }
}