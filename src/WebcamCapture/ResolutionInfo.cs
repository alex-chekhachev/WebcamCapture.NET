using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;

namespace WebcamCapture
{
    class ResolutionInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Bpp { get; set; }

        public static ResolutionInfo Create(AMMediaType media)
        {
            return new ResolutionInfo(media);
        }

        private ResolutionInfo(AMMediaType media)
        {
            var videoInfo = new VideoInfoHeader();
            Marshal.PtrToStructure(media.formatPtr, videoInfo);
            Width = videoInfo.BmiHeader.Width;
            Height = videoInfo.BmiHeader.Height;
            Bpp = videoInfo.BmiHeader.BitCount;
        }

        #region Overrides of Object

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Resolution: ").AppendFormat("{0}x{1}x{2}", Width, Height, Bpp);

            return sb.ToString();
        }

        #endregion
    }
}
