using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Textures;
using OpenTK.Graphics.ES30;

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
    }
}
