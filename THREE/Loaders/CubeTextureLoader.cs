using System.Collections.Generic;
using System.Drawing;

namespace THREE
{
    public class CubeTextureLoader
    {
        public CubeTextureLoader()
        {

        }
        public static CubeTexture Load(List<string> filePath)
        {
            CubeTexture texture = new CubeTexture();
            for(int i = 0; i < filePath.Count; i++)
            {
                Bitmap bitmap = new Bitmap(filePath[i]);
                //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Texture image = new Texture();
                image.Image = bitmap;
                image.Format = Constants.RGBFormat;
                image.NeedsUpdate = true;

                texture.Images[i]=image;
            }
            texture.NeedsUpdate = true;
            return texture;
        }
    }
}
