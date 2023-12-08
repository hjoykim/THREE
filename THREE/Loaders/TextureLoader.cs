using System.Drawing;
using System.Reflection;
using Pfim;
using System.Runtime.InteropServices;
using StbImageSharp;
using System.Drawing.Imaging;
using System.IO;

namespace THREE
{
    public class TextureLoader
    {
        public TextureLoader()
        {
        }

        public static Texture LoadTGA(string filePath)
        {
            var texture = new Texture();

            ImageResult image = null;

            using(var stream = File.OpenRead(filePath))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            if (image != null)
            {
                byte[] data = image.Data;
                for(int i = 0; i < image.Width * image.Height; ++i)
                {
                    byte r = data[i * 4];
                    byte g = data[i * 4 + 1];
                    byte b = data[i * 4 + 2];
                    byte a = data[i * 4 + 3];

                    data[i * 4] = b;
                    data[i * 4 + 1] = g;
                    data[i * 4 + 2] = r;
                    data[i * 4 + 3] = a;
                }

                Bitmap bmp = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);

                Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride * bmp.Height);
                bmp.UnlockBits(bmpData);

                texture.Image = bmp;
                texture.Format = Constants.RGBFormat;
                texture.NeedsUpdate = true;
            }
            return texture;


        }
        public static Texture LoadDDS(string filePath)
        {
            var image = Pfimage.FromFile(filePath);

            var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
            Bitmap bitmap = new Bitmap(image.Width, image.Height, image.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, data);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var texture = new Texture();
            texture.Image = bitmap;
            texture.Format = Constants.RGBFormat;
            texture.NeedsUpdate = true;

            return texture;
        }
        public static Texture Load(string filePath)
        {
            Bitmap bitmap = new Bitmap(filePath);

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            Texture texture = new Texture();
            texture.Image = bitmap;
            texture.Format = Constants.RGBFormat;
            texture.NeedsUpdate = true;

            return texture;
        }

        public static Texture LoadEmbedded(string EmbeddedPath)
        {
            string embeddedNameBase = "THREE.Resources.";
            Bitmap bitmap = new Bitmap(typeof(THREE.Object3D).GetTypeInfo().Assembly.GetManifestResourceStream(embeddedNameBase + EmbeddedPath));

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            Texture texture = new Texture();
            texture.Image = bitmap;
            texture.Format = Constants.RGBFormat;
            texture.NeedsUpdate = true;

            return texture;
        }
    }
}
