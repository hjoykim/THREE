using System.Drawing;

namespace THREE
{
    public class VideoTexture : Texture
    {
        public VideoTexture(Bitmap video = null, int? mapping = null, int wrapS = 0, int wrapT = 0, int magFilter = 0, int minFilter = 0, int format = 0, int type = 0, int anisotropy = 1)
            :base(video,mapping,wrapS,wrapT,magFilter,minFilter,format,type,anisotropy,null)
        {
            this.GenerateMipmaps = false;

        }
        public void Update()
        {
            var video = this.Image;

            this.NeedsUpdate = true;

        }
    }
}
