
using Silk.NET.OpenGLES;

namespace THREE
{
    [Serializable]
    public class GLUtils
    {
        private GLExtensions Extensions;

        private GLCapabilities Capabilities;

        public GLUtils(GLExtensions extensions, GLCapabilities capabilities)
        {
            this.Extensions = extensions;

            this.Capabilities = capabilities;
        }

        public GLEnum Convert(int p)
        {
            if (p == Constants.RepeatWrapping) return GLEnum.Repeat;
            if (p == Constants.ClampToEdgeWrapping) return GLEnum.ClampToEdge;
            if (p == Constants.MirroredRepeatWrapping) return GLEnum.MirroredRepeat;

            if (p == Constants.NearestFilter) return GLEnum.Nearest;
            if (p == Constants.NearestMipmapNearestFilter) return GLEnum.NearestMipmapNearest;
            if (p == Constants.NearestMipmapLinearFilter) return GLEnum.NearestMipmapLinear;

            if (p == Constants.LinearFilter) return GLEnum.Linear;
            if (p == Constants.LinearMipmapNearestFilter) return GLEnum.LinearMipmapNearest;
            if (p == Constants.LinearMipmapLinearFilter) return GLEnum.LinearMipmapLinear;

            if (p == Constants.UnsignedByteType) return GLEnum.UnsignedByte;
            if (p == Constants.UnsignedShort4444Type) return GLEnum.UnsignedShort4444;
            if (p == Constants.UnsignedShort5551Type) return GLEnum.UnsignedShort5551;
            if (p == Constants.UnsignedShort565Type) return GLEnum.UnsignedShort565;

            if (p == Constants.ByteType) return GLEnum.Byte;
            if (p == Constants.ShortType) return GLEnum.Short;
            if (p == Constants.UnsignedShortType) return GLEnum.UnsignedShort;
            if (p == Constants.IntType) return GLEnum.Int;
            if (p == Constants.UnsignedIntType) return GLEnum.UnsignedInt;
            if (p == Constants.FloatType) return GLEnum.Float;

            if (p == Constants.HalfFloatType)
            {

                if (Capabilities.IsGL2) return GLEnum.HalfFloat;

                if (Extensions.ExtensionsName.Contains("GL_ARB_texture_half_float")) //'OES_texture_half_float'
                    return GLEnum.HalfFloat;

            }

            if (p == Constants.AlphaFormat) return GLEnum.Alpha;
            if (p == Constants.RGBFormat) return GLEnum.Rgb;
            if (p == Constants.RGBAFormat) return GLEnum.Rgba;
            if (p == Constants.LuminanceFormat) return GLEnum.Luminance;
            if (p == Constants.LuminanceAlphaFormat) return GLEnum.LuminanceAlpha;
            if (p == Constants.DepthFormat) return GLEnum.DepthComponent;
            if (p == Constants.DepthStencilFormat) return GLEnum.DepthStencil;
            if (p == Constants.RedFormat) return GLEnum.Red;

            if (p == Constants.RedIntegerFormat) return GLEnum.RedInteger;
            if (p == Constants.RGFormat) return GLEnum.RG;
            if (p == Constants.RGIntegerFormat) return GLEnum.RGInteger;
            if (p == Constants.RGBIntegerFormat) return GLEnum.RgbInteger;
            if (p == Constants.RGBAIntegerFormat) return GLEnum.RgbaInteger;

            if (p == Constants.AddEquation) return GLEnum.FuncAdd;
            if (p == Constants.SubtractEquation) return GLEnum.FuncSubtract;
            if (p == Constants.ReverseSubtractEquation) return GLEnum.FuncReverseSubtract;

            if (p == Constants.ZeroFactor) return GLEnum.Zero;
            if (p == Constants.OneFactor) return GLEnum.One;
            if (p == Constants.SrcColorFactor) return GLEnum.SrcColor;
            if (p == Constants.OneMinusSrcColorFactor) return GLEnum.OneMinusSrcColor;
            if (p == Constants.SrcAlphaFactor) return GLEnum.SrcAlpha;
            if (p == Constants.OneMinusSrcAlphaFactor) return GLEnum.OneMinusSrcAlpha;
            if (p == Constants.DstAlphaFactor) return GLEnum.DstAlpha;
            if (p == Constants.OneMinusDstAlphaFactor) return GLEnum.OneMinusDstAlpha;

            if (p == Constants.DstColorFactor) return GLEnum.DstColor;
            if (p == Constants.OneMinusDstColorFactor) return GLEnum.OneMinusDstColor;
            if (p == Constants.SrcAlphaSaturateFactor) return GLEnum.SrcAlphaSaturate;

            if (p == Constants.RGB_S3TC_DXT1_Format || p == Constants.RGBA_S3TC_DXT1_Format ||
                p == Constants.RGBA_S3TC_DXT3_Format || p == Constants.RGBA_S3TC_DXT5_Format)
            {
#if OPENGLES
                if (Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_s3tc")) // 'WEBGL_compressed_texture_s3tc'
                {

                    if (p == Constants.RGB_S3TC_DXT1_Format) return GLEnum.CompressedRgbS3tcDxt1Ext;
                    if (p == Constants.RGBA_S3TC_DXT1_Format) return GLEnum.CompressedRgbaS3tcDxt1Ext;
                    if (p == Constants.RGBA_S3TC_DXT3_Format) return GLEnum.CompressedRgbaS3tcDxt3Ext;
                    if (p == Constants.RGBA_S3TC_DXT5_Format) return GLEnum.CompressedRgbaS3tcDxt5Ext;

                }
#endif

            }

            if (p == Constants.RGB_PVRTC_4BPPV1_Format || p == Constants.RGB_PVRTC_2BPPV1_Format ||
                p == Constants.RGBA_PVRTC_4BPPV1_Format || p == Constants.RGBA_PVRTC_2BPPV1_Format)
            {
#if OPENGLES
                if (Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_pvrtc"))// 'WEBGL_compressed_texture_pvrtc' 
                {
                    if (p == Constants.RGB_PVRTC_4BPPV1_Format) return GLEnum.CompressedRgbPvrtc4Bppv1Img;//extension.COMPRESSED_RGB_PVRTC_4BPPV1_IMG;
                    if (p == Constants.RGB_PVRTC_2BPPV1_Format) return GLEnum.CompressedRgbPvrtc2Bppv1Img;
                    if (p == Constants.RGBA_PVRTC_4BPPV1_Format) return GLEnum.CompressedRgbaPvrtc4Bppv1Img;
                    if (p == Constants.RGBA_PVRTC_2BPPV1_Format) return GLEnum.CompressedRgbaPvrtc2Bppv1Img;

                }
#endif

            }

            if (p == Constants.RGB_ETC1_Format)
            {

                if (Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_etc1"))
                    return GLEnum.CompressedRgb8Etc2;
            }

            if (p == Constants.RGBA_ASTC_4x4_Format || p == Constants.RGBA_ASTC_5x4_Format || p == Constants.RGBA_ASTC_5x5_Format ||
                p == Constants.RGBA_ASTC_6x5_Format || p == Constants.RGBA_ASTC_6x6_Format || p == Constants.RGBA_ASTC_8x5_Format ||
                p == Constants.RGBA_ASTC_8x6_Format || p == Constants.RGBA_ASTC_8x8_Format || p == Constants.RGBA_ASTC_10x5_Format ||
                p == Constants.RGBA_ASTC_10x6_Format || p == Constants.RGBA_ASTC_10x8_Format || p == Constants.RGBA_ASTC_10x10_Format ||
                p == Constants.RGBA_ASTC_12x10_Format || p == Constants.RGBA_ASTC_12x12_Format)
            {

                if (Extensions.ExtensionsName.Contains("GL_ARB_compressed_texture_astc"))
                    return GLEnum.CompressedTextureFormats;


            }

            if (p == Constants.MinEquation || p == Constants.MaxEquation)
            {

                if (Capabilities.IsGL2)
                {

                    if (p == Constants.MinEquation) return GLEnum.Min;
                    if (p == Constants.MaxEquation) return GLEnum.Max;

                }

                if (Extensions.ExtensionsName.Contains("GL_EXT_blend_minmax"))
                {

                    if (p == Constants.MinEquation) return GLEnum.Min;
                    if (p == Constants.MaxEquation) return GLEnum.Max;

                }

            }

            if (p == Constants.UnsignedInt248Type)
            {

                if (Capabilities.IsGL2) return GLEnum.UnsignedInt248;

                if (Extensions.ExtensionsName.Contains("GL_ARB_depth_texture"))
                    return GLEnum.UnsignedInt248;
            }
            return 0;
        }
    }
}
