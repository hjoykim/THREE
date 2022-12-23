using System;

namespace THREE
{
    public class DepthTexture : Texture
    {

        public DepthTexture(int width, int height, int? type, int? mapping=null, int? wrapS=null, int? wrapT=null, int? magFilter=null, int? minFilter=null, int? anisotropy=null,int? format=null)
            :base(null,mapping,wrapS,wrapT,magFilter,minFilter,format,anisotropy)
        {
            this.Format = format!=null ? (int)format : Constants.DepthFormat;

            if (this.Format != Constants.DepthFormat && this.Format != Constants.DepthStencilFormat)
            {
                throw new Exception("DepthTexture format must be either Constants.DepthFormat or Constants.DepthStencilFormat");
            }

            if (type == 0 && this.Format == Constants.DepthFormat) this.Type = Constants.UnsignedShortType;
            if (type == 0 && this.Format == Constants.DepthStencilFormat) this.Type = Constants.UnsignedInt248Type;

            this.ImageSize.Width = width;
            this.ImageSize.Height = height;

            this.flipY = false;
            this.GenerateMipmaps = false;

        }
    }
}
