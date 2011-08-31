using System;

namespace WebcamCapture.Plugin
{
    /// <summary>
    /// Preprocessing interceptor interface.
    /// </summary>
    public interface IPreprocessingInterceptor : IInterceptor
    {
        /// <summary>
        /// Executes the processing.
        /// </summary>
        /// <param name="pBuffer">Frame data buffer.</param>
        /// <param name="videoHeight">Height of the video frame.</param>
        /// <param name="videoStride">The video stride.</param>
        void Execute(IntPtr pBuffer, int videoHeight, int videoStride);
    }
}