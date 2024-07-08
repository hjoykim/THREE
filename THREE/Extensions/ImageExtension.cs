using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace THREE
{
    [Serializable]
    public struct RGBA
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }
    [Serializable]
    public static class ImageExtension
    {
        public static byte[] ToByteArray(this float[] floatArray)
        {
            byte[] byteArray = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
                byteArray[i] = (byte)(floatArray[i] * 255.0f);

            return byteArray;
        }
        public static SKBitmap ToSKBitMap(this byte[] byteArray,int width,int height)
        {
            SKBitmap bitmap = new SKBitmap();

            // pin the managed array so that the GC doesn't move it
            var gcHandle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);

            // install the pixels with the color type of the pixel data
            var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
            bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes, null, delegate { gcHandle.Free(); }, null);

            return bitmap;
        }
        // => SKBitmap.Bytes -> byte[]
        //public static byte[] GetTextureImage(this SKBitmap image)
        //{
            //List<byte> pixels = new List<byte>();
            //BitmapData imageData = null;

            //imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            //unsafe
            //{
            //    RGBA* pixelImage = (RGBA*)imageData.Scan0.ToPointer();
            //    for (int y = 0; y < image.Height; y++)
            //    {
            //        for (int x = 0; x < image.Width; x++)
            //        {
            //            int pixelIndex = y * image.Width + x;
            //            RGBA pixelRGBA = pixelImage[pixelIndex];
            //            pixels.Add(pixelRGBA.R);
            //            pixels.Add(pixelRGBA.G);
            //            pixels.Add(pixelRGBA.B);
            //            pixels.Add(pixelRGBA.A);
            //        }
            //    }
            //}
            //image.UnlockBits(imageData);

            //return pixels.ToArray();
        //}
    }
}
