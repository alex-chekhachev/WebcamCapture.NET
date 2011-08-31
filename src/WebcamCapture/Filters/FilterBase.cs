using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DirectShowLib;
using WebcamCapture.Plugin;
using WebcamCapture.Plugin.Internal;

namespace WebcamCapture.Filters
{
    internal abstract class FilterBase<TInterceptor> : IFilter where TInterceptor : IInterceptor
    {
        public IInterceptorRegistry Interceptors { get; set; }

        public int VideoHeight { get; set; }
        public int VideoStride { get; set; }

        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            IntPtr ptr;
            pSample.GetPointer(out ptr);

            Execute(ptr, VideoHeight, VideoStride);

            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            Execute(pBuffer, VideoHeight, VideoStride);
            return 0;
        }

        protected IEnumerable<TInterceptor> GetInterceptors()
        {
            //TODO: Initialize interceptors only once.
            return Interceptors.GetInterceptors<TInterceptor>();
        }

        protected abstract void Execute(IntPtr pBuffer, int videoHeight, int videoStride);
    }
}