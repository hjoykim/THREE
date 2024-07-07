using StbImageSharp;
using System.Collections.Generic;
using System.Drawing;

namespace THREE
{
    [Serializable]
    public class CubeTextureLoader
    {
        public CubeTextureLoader()
        {

        }
        public static CubeTexture Load(List<string> filePath)
        {
            CubeTexture texture = new CubeTexture();
            for (int i = 0; i < filePath.Count; i++)
            {
                using (Stream stream = File.OpenRead(filePath[i]))
                {
                    ImageResult bitmap = ImageResult.FromStream(stream);
                    //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    Texture image = new Texture();
                    image.Image = bitmap;
                    image.Format = bitmap.Comp==ColorComponents.RedGreenBlue ? Constants.RGBFormat : Constants.RGBAFormat ;
                    image.NeedsUpdate = true;

                    texture.Images[i] = image;
                }
            }
            texture.NeedsUpdate = true;
            return texture;
        }
    }
}
