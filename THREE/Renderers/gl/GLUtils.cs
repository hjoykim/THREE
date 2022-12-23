using OpenTK.Graphics.ES30;
namespace THREE
{
    public class GLUtils
    {
        private GLExtensions Extensions;

        private GLCapabilities Capabilities;

        public GLUtils(GLExtensions extensions, GLCapabilities capabilities)
        {
            this.Extensions = extensions;

            this.Capabilities = capabilities;
        }

        public All Convert(int p)
        {
            if (p == Constants.RepeatWrapping) return All.Repeat;
            if ( p == Constants.ClampToEdgeWrapping ) return All.ClampToEdge;
		    if ( p == Constants.MirroredRepeatWrapping ) return All.MirroredRepeat;

		    if ( p == Constants.NearestFilter ) return All.Nearest;
		    if ( p == Constants.NearestMipmapNearestFilter ) return All.NearestMipmapNearest;
		    if ( p == Constants.NearestMipmapLinearFilter ) return All.NearestMipmapLinear;

		    if ( p == Constants.LinearFilter ) return All.Linear;
		    if ( p == Constants.LinearMipmapNearestFilter ) return All.LinearMipmapNearest;
		    if ( p == Constants.LinearMipmapLinearFilter ) return All.LinearMipmapLinear;

		    if ( p ==Constants.UnsignedByteType ) return All.UnsignedByte;
		    if ( p ==Constants.UnsignedShort4444Type ) return All.UnsignedShort4444;
		    if ( p ==Constants.UnsignedShort5551Type ) return All.UnsignedShort5551;
		    if ( p ==Constants.UnsignedShort565Type ) return All.UnsignedShort565;

		    if ( p ==Constants.ByteType ) return All.Byte;
		    if ( p ==Constants.ShortType ) return All.Short;
		    if ( p ==Constants.UnsignedShortType ) return All.UnsignedShort;
		    if ( p ==Constants.IntType ) return All.Int;
		    if ( p ==Constants.UnsignedIntType ) return All.UnsignedInt;
		    if ( p ==Constants.FloatType ) return All.Float;

            if ( p ==Constants.HalfFloatType ) {

                if ( Capabilities.IsGL2 ) return All.HalfFloat;

                if(Extensions.ExtensionsName.Contains("GL_ARB_texture_half_float")) //'OES_texture_half_float'
                    return All.HalfFloatOes;                

            }

            if ( p ==Constants.AlphaFormat ) return All.Alpha;
            if ( p ==Constants.RGBFormat ) return All.Rgb;
            if ( p ==Constants.RGBAFormat ) return All.Rgba;
            if ( p ==Constants.LuminanceFormat ) return All.Luminance;
            if ( p ==Constants.LuminanceAlphaFormat ) return All.LuminanceAlpha;
            if ( p ==Constants.DepthFormat ) return All.DepthComponent;
            if ( p ==Constants.DepthStencilFormat ) return All.DepthStencil;
            if ( p ==Constants.RedFormat ) return All.Red;

            if (p == Constants.RedIntegerFormat) return All.RedInteger;
            if (p == Constants.RGFormat) return All.Rg;
            if (p == Constants.RGIntegerFormat) return All.RgInteger;
            if (p == Constants.RGBIntegerFormat) return All.RgbInteger;
            if (p == Constants.RGBAIntegerFormat) return All.RgbaInteger;

            if ( p ==Constants.AddEquation ) return All.FuncAdd;
            if ( p ==Constants.SubtractEquation ) return All.FuncSubtract;
            if ( p ==Constants.ReverseSubtractEquation ) return All.FuncReverseSubtract;

            if ( p ==Constants.ZeroFactor ) return All.Zero;
            if ( p ==Constants.OneFactor ) return All.One;
            if ( p ==Constants.SrcColorFactor ) return All.SrcColor;
            if ( p ==Constants.OneMinusSrcColorFactor ) return All.OneMinusSrcColor;
            if ( p ==Constants.SrcAlphaFactor ) return All.SrcAlpha;
            if ( p ==Constants.OneMinusSrcAlphaFactor ) return All.OneMinusSrcAlpha;
            if ( p ==Constants.DstAlphaFactor ) return All.DstAlpha;
            if ( p ==Constants.OneMinusDstAlphaFactor ) return All.OneMinusDstAlpha;

            if ( p ==Constants.DstColorFactor ) return All.DstColor;
            if ( p ==Constants.OneMinusDstColorFactor ) return All.OneMinusDstColor;
            if ( p ==Constants.SrcAlphaSaturateFactor ) return All.SrcAlphaSaturate;

            if ( p ==Constants.RGB_S3TC_DXT1_Format || p ==Constants.RGBA_S3TC_DXT1_Format ||
                p ==Constants.RGBA_S3TC_DXT3_Format || p ==Constants.RGBA_S3TC_DXT5_Format ) {

                if(Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_s3tc")) // 'WEBGL_compressed_texture_s3tc'
                {

                    if ( p ==Constants.RGB_S3TC_DXT1_Format ) return All.CompressedRgbS3tcDxt1Ext;
                    if ( p ==Constants.RGBA_S3TC_DXT1_Format ) return All.CompressedRgbaS3tcDxt1Ext;
                    if ( p ==Constants.RGBA_S3TC_DXT3_Format ) return All.CompressedRgbaS3tcDxt3Ext;
                    if ( p ==Constants.RGBA_S3TC_DXT5_Format ) return All.CompressedRgbaS3tcDxt5Ext;

                }

            }

            if ( p ==Constants.RGB_PVRTC_4BPPV1_Format || p ==Constants.RGB_PVRTC_2BPPV1_Format ||
                p ==Constants.RGBA_PVRTC_4BPPV1_Format || p ==Constants.RGBA_PVRTC_2BPPV1_Format ) {

                if(Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_pvrtc"))// 'WEBGL_compressed_texture_pvrtc' 
                {
                    if ( p ==Constants.RGB_PVRTC_4BPPV1_Format ) return All.CompressedRgbPvrtc4Bppv1Img;//extension.COMPRESSED_RGB_PVRTC_4BPPV1_IMG;
                    if ( p ==Constants.RGB_PVRTC_2BPPV1_Format ) return All.CompressedRgbPvrtc2Bppv1Img;
                    if ( p ==Constants.RGBA_PVRTC_4BPPV1_Format ) return All.CompressedRgbaPvrtc4Bppv1Img;
                    if ( p ==Constants.RGBA_PVRTC_2BPPV1_Format ) return All.CompressedRgbaPvrtc2Bppv1Img;

                }

            }

            if ( p ==Constants.RGB_ETC1_Format ) {

                if(Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_etc1"))
                    return All.CompressedRgb;
            }

            if ( p ==Constants.RGBA_ASTC_4x4_Format || p ==Constants.RGBA_ASTC_5x4_Format || p ==Constants.RGBA_ASTC_5x5_Format ||
                p ==Constants.RGBA_ASTC_6x5_Format || p ==Constants.RGBA_ASTC_6x6_Format || p ==Constants.RGBA_ASTC_8x5_Format ||
                p ==Constants.RGBA_ASTC_8x6_Format || p ==Constants.RGBA_ASTC_8x8_Format || p ==Constants.RGBA_ASTC_10x5_Format ||
                p ==Constants.RGBA_ASTC_10x6_Format || p ==Constants.RGBA_ASTC_10x8_Format || p ==Constants.RGBA_ASTC_10x10_Format ||
                p ==Constants.RGBA_ASTC_12x10_Format || p ==Constants.RGBA_ASTC_12x12_Format ) {

                if(Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_astc"))
                    return All.CompressedTextureFormats;
               

            }

            if ( p == Constants.MinEquation || p == Constants.MaxEquation ) {

                if ( Capabilities.IsGL2 ) {

                    if ( p == Constants.MinEquation ) return All.Min;
                    if ( p == Constants.MaxEquation ) return All.Max;

                }

                if(Extensions.ExtensionsName.Contains( "GL_EXT_blend_minmax" ))
                {

                    if ( p == Constants.MinEquation ) return All.MinExt;
                    if ( p == Constants.MaxEquation ) return All.MaxExt;

                }

            }

            if ( p == Constants.UnsignedInt248Type ) {

                if (Capabilities.IsGL2 ) return All.UnsignedInt248;

                if(Extensions.ExtensionsName.Contains("GL_ARB_depth_texture"))
                    return All.UnsignedInt248;
            }
		    return 0;
        }
    }
}
