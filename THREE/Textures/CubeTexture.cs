
namespace THREE
{
    public class CubeTexture : Texture
    {
        public CubeTexture() : base()
        {
            this.Mapping = Constants.CubeReflectionMapping;
            this.Format = Constants.RGBAFormat;

            this.flipY = false;
        }
        public CubeTexture(Texture[] images, int? mapping = null, int? wrapS = null, int? wrapT = null, int? magFilter = null, int? minFilter = null, int? format = null, int? type = null, int? anisotropy = null, int? encoding = null)
            : base(null, mapping, wrapS, wrapT, magFilter, minFilter, format, type, anisotropy, encoding)
        {
            if (images != null && images.Length > 0)
                Images = images;


            this.Mapping = mapping != null ? mapping.Value : (int)Constants.CubeReflectionMapping;
            this.Format = format != null ? format.Value : (int)Constants.RGBAFormat;

            this.flipY = false;
        }
    }
}
