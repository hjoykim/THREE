using Pfim;
using StbImageSharp;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace THREE
{
    [Serializable]
    public class TextureLoader
    {
        public TextureLoader()
        {
        }

        public static Texture LoadTGA(string filePath)
        {
            return Load(filePath);

        }
        public static Texture LoadDDS(string filePath)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            var image = Pfimage.FromFile(filePath);
            ImageResult imageResult = new ImageResult
            {
                Width = image.Width,
                Height = image.Height,
                SourceComp = ColorComponents.RedGreenBlueAlpha,
                Comp = ColorComponents.RedGreenBlueAlpha
            };
            imageResult.Data = image.Data;
            // need to rotate  like as     bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var texture = new Texture();
            texture.Image = imageResult;
            texture.Format = Constants.RGBAFormat;
            texture.NeedsUpdate = true;

            return texture;
        }
        public static Texture Load(string filePath)
        {
            var texture = new Texture();
            ImageResult image = null;
            StbImage.stbi_set_flip_vertically_on_load(1); 
            using (var stream = File.OpenRead(filePath))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            if (image != null)
            {
                texture.Image = image;
                texture.Format = Constants.RGBAFormat;
                texture.NeedsUpdate = true;
            }
            return texture;
        }

        public static Texture LoadEmbedded(string EmbeddedPath)
        {
            string embeddedNameBase = "THREE.Resources.";

            var texture = new Texture();

            ImageResult image = null;
            StbImage.stbi_set_flip_vertically_on_load(1);
            using (var stream = File.OpenRead(embeddedNameBase + EmbeddedPath))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            if (image != null)
            {
                texture.Image = image;
                texture.Format = Constants.RGBAFormat;
                texture.NeedsUpdate = true;
            }
            return texture;
        }
    }
}
