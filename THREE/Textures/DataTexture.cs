
using System.Drawing;


namespace THREE
{
    public class DataTexture : Texture
    {
        public byte[] byteData;
        public float[] floatData;
        public int[] intData;

        public DataTexture() : base() { }
        public DataTexture(Bitmap image,int width,int height,int format,int type, int? mapping=null,int? wrapS=null, int? wrapT=null, int? magFilter=null,int? minFilter=null,int? anisotropy=null,int? encoding=null)
            :base(image,mapping,wrapS,wrapT,magFilter,minFilter,format,type,anisotropy,encoding)       
        {

            this.MagFilter = magFilter != null ? (int)magFilter : Constants.NearestFilter;
            this.MinFilter = minFilter != null ? (int)minFilter : Constants.NearestFilter;

            this.GenerateMipmaps = false;
            this.flipY = false;
            this.UnpackAlignment = 1;

            this.ImageSize.Width = width;
            this.ImageSize.Height = height;
            this.NeedsUpdate = true;
        }
           
    }
}
