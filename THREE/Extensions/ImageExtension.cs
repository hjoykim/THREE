using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace THREE
{
    public struct RGBA
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }
    public static class ImageExtension
    {
         public static byte[] GetTextureImage(this Bitmap image)
        {
            List<byte> pixels = new List<byte>();
            BitmapData imageData = null;

            imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            unsafe
            {
                RGBA* pixelImage = (RGBA*)imageData.Scan0.ToPointer();
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int pixelIndex = y * image.Width + x;
                        RGBA pixelRGBA = pixelImage[pixelIndex];
                        pixels.Add(pixelRGBA.R);
                        pixels.Add(pixelRGBA.G);
                        pixels.Add(pixelRGBA.B);
                        pixels.Add(pixelRGBA.A);
                    }
                }
            }
            image.UnlockBits(imageData);

            return pixels.ToArray();
        }
    }
}
