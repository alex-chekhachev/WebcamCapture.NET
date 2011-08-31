using System;
using WebcamCapture.Plugin;

namespace WebcamCapture.Plugins.Effects
{
    internal class EffectsInterceptor : IPreprocessingInterceptor
    {
        private bool enabled;

        public void Execute(IntPtr pBuffer, int videoHeight, int videoStride)
        {
            if (!enabled) return;

            Negate(pBuffer, videoHeight, videoStride);
        }

        private static unsafe void Negate(IntPtr pBuffer, int videoHeight, int videoStride)
        {
            var b = (byte*) pBuffer;
            int x;

            for (x = 1; x <= videoHeight; x++)
            {
                int y;
                for (y = 0; y < videoStride; y++)
                {
                    *b ^= 0xff;
                    b++;
                }

                b = (byte*) (pBuffer);
                b += (x * videoStride);
            }
        }

        public void Disable()
        {
            enabled = false;
        }

        public void SetEffect(Effect effect)
        {
            switch (effect)
            {
                case Effect.Negate:
                    enabled = true;
                    break;
            }
        }

        /// <summary>
        /// Effects supported by the filter.
        /// </summary>
        internal enum Effect
        {
            Negate
        }
    }
}