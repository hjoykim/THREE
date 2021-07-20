using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Textures
{
    public class DataTexture : Texture
    {
        public DataTexture(Bitmap image,int width,int height,int format,int type, int? mapping=null,int? wrapS=null, int? wrapT=null, int? magFilter=null,int? minFilter=null,int? anisotropy=null,int? encoding=null)
            :base(image,mapping,wrapS,wrapT,magFilter,minFilter,format,type,anisotropy,encoding)       
        {

            this.MagFilter = magFilter != null ? (int)magFilter : Constants.NearestFilter;
            this.MinFilter = minFilter != null ? (int)minFilter : Constants.NearestFilter;

            this.GenerateMipmaps = false;
            this.flipY = false;
            this.UnpackAlignment = 1;

            this.NeedsUpdate = true;
        }
           
    }
}
