using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Textures;
using OpenTK.Graphics.ES30;
using System.Reflection;

namespace THREE.Loaders
{
    public class TextureLoader
    {
        public TextureLoader()
        {
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
            Bitmap bitmap = new Bitmap(typeof(THREE.Core.Object3D).GetTypeInfo().Assembly.GetManifestResourceStream(embeddedNameBase + EmbeddedPath));

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            Texture texture = new Texture();
            texture.Image = bitmap;
            texture.Format = Constants.RGBFormat;
            texture.NeedsUpdate = true;

            return texture;
        }
    }
}
