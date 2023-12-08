using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    public class Constants
    {
        public static string REVISION = "121";
        public enum MOUSE  { LEFT= 0, MIDDLE= 1, RIGHT= 2, ROTATE= 0, DOLLY= 1, PAN= 2 };
        public enum TOUCH  { ROTATE= 0, PAN= 1, DOLLY_PAN= 2, DOLLY_ROTATE= 3 };
        public static int LineStrip = 0;
        public static int LinePieces = 1;
        public static int CullFaceNone = 0;
        public static int CullFaceBack = 1;
        public static int CullFaceFront = 2;
        public static int CullFaceFrontBack = 3;
        public static int FrontFaceDirectionCW = 0;
        public static int FrontFaceDirectionCCW = 1;
        public static int BasicShadowMap = 0;
        public static int PCFShadowMap = 1;
        public static int PCFSoftShadowMap = 2;
        public static int VSMShadowMap = 3;
        public static int FrontSide = 0;
        public static int BackSide = 1;
        public static int DoubleSide = 2;
        public static int FlatShading = 1;
        public static int SmoothShading = 2;
        public static int NoColors = 0;
        public static int FaceColors = 1;
        public static int VertexColors = 2;
        public static int NoBlending = 0;
        public static int NormalBlending = 1;
        public static int AdditiveBlending = 2;
        public static int SubtractiveBlending = 3;
        public static int MultiplyBlending = 4;
        public static int CustomBlending = 5;
        public static int AddEquation = 100;
        public static int SubtractEquation = 101;
        public static int ReverseSubtractEquation = 102;
        public static int MinEquation = 103;
        public static int MaxEquation = 104;
        public static int ZeroFactor = 200;
        public static int OneFactor = 201;
        public static int SrcColorFactor = 202;
        public static int OneMinusSrcColorFactor = 203;
        public static int SrcAlphaFactor = 204;
        public static int OneMinusSrcAlphaFactor = 205;
        public static int DstAlphaFactor = 206;
        public static int OneMinusDstAlphaFactor = 207;
        public static int DstColorFactor = 208;
        public static int OneMinusDstColorFactor = 209;
        public static int SrcAlphaSaturateFactor = 210;
        public static int NeverDepth = 0;
        public static int AlwaysDepth = 1;
        public static int LessDepth = 2;
        public static int LessEqualDepth = 3;
        public static int EqualDepth = 4;
        public static int GreaterEqualDepth = 5;
        public static int GreaterDepth = 6;
        public static int NotEqualDepth = 7;
        public static int MultiplyOperation = 0;
        public static int MixOperation = 1;
        public static int AddOperation = 2;
        public static int NoToneMapping = 0;
        public static int LinearToneMapping = 1;
        public static int ReinhardToneMapping = 2;
        public static int CineonToneMapping = 3;
        public static int ACESFilmicToneMapping = 4;
        public static int CustomToneMapping = 5;

        public static int UVMapping = 300;
        public static int CubeReflectionMapping = 301;
        public static int CubeRefractionMapping = 302;
        public static int EquirectangularReflectionMapping = 303;
        public static int EquirectangularRefractionMapping = 304;
        public static int SphericalReflectionMapping = 305;
        public static int CubeUVReflectionMapping = 306;
        public static int CubeUVRefractionMapping = 307;
        public static int RepeatWrapping = 1000;
        public static int ClampToEdgeWrapping = 1001;
        public static int MirroredRepeatWrapping = 1002;
        public static int NearestFilter = 1003;
        public static int NearestMipmapNearestFilter = 1004;
        public static int NearestMipMapNearestFilter = 1004;
        public static int NearestMipmapLinearFilter = 1005;
        public static int NearestMipMapLinearFilter = 1005;
        public static int LinearFilter = 1006;
        public static int LinearMipmapNearestFilter = 1007;
        public static int LinearMipMapNearestFilter = 1007;
        public static int LinearMipmapLinearFilter = 1008;
        public static int LinearMipMapLinearFilter = 1008;
        public static int UnsignedByteType = 1009;
        public static int ByteType = 1010;
        public static int ShortType = 1011;
        public static int UnsignedShortType = 1012;
        public static int IntType = 1013;
        public static int UnsignedIntType = 1014;
        public static int FloatType = 1015;
        public static int HalfFloatType = 1016;
        public static int UnsignedShort4444Type = 1017;
        public static int UnsignedShort5551Type = 1018;
        public static int UnsignedShort565Type = 1019;
        public static int UnsignedInt248Type = 1020;
        public static int AlphaFormat = 1021;
        public static int RGBFormat = 1022;
        public static int RGBAFormat = 1023;
        public static int LuminanceFormat = 1024;
        public static int LuminanceAlphaFormat = 1025;
        public static int RGBEFormat = RGBAFormat;
        public static int DepthFormat = 1026;
        public static int DepthStencilFormat = 1027;
        public static int RedFormat = 1028;
        public static int RedIntegerFormat = 1029;
        public static int RGFormat = 1030;
        public static int RGIntegerFormat = 1031;
        public static int RGBIntegerFormat = 1032;
        public static int RGBAIntegerFormat = 1033;

        public static int RGB_S3TC_DXT1_Format = 33776;
        public static int RGBA_S3TC_DXT1_Format = 33777;
        public static int RGBA_S3TC_DXT3_Format = 33778;
        public static int RGBA_S3TC_DXT5_Format = 33779;
        public static int RGB_PVRTC_4BPPV1_Format = 35840;
        public static int RGB_PVRTC_2BPPV1_Format = 35841;
        public static int RGBA_PVRTC_4BPPV1_Format = 35842;
        public static int RGBA_PVRTC_2BPPV1_Format = 35843;
        public static int RGB_ETC1_Format = 36196;
        public static int RGBA_ASTC_4x4_Format = 37808;
        public static int RGBA_ASTC_5x4_Format = 37809;
        public static int RGBA_ASTC_5x5_Format = 37810;
        public static int RGBA_ASTC_6x5_Format = 37811;
        public static int RGBA_ASTC_6x6_Format = 37812;
        public static int RGBA_ASTC_8x5_Format = 37813;
        public static int RGBA_ASTC_8x6_Format = 37814;
        public static int RGBA_ASTC_8x8_Format = 37815;
        public static int RGBA_ASTC_10x5_Format = 37816;
        public static int RGBA_ASTC_10x6_Format = 37817;
        public static int RGBA_ASTC_10x8_Format = 37818;
        public static int RGBA_ASTC_10x10_Format = 37819;
        public static int RGBA_ASTC_12x10_Format = 37820;
        public static int RGBA_ASTC_12x12_Format = 37821;
        public static int LoopOnce = 2200;
        public static int LoopRepeat = 2201;
        public static int LoopPingPong = 2202;
        public static int InterpolateDiscrete = 2300;
        public static int InterpolateLinear = 2301;
        public static int InterpolateSmooth = 2302;
        public static int ZeroCurvatureEnding = 2400;
        public static int ZeroSlopeEnding = 2401;
        public static int WrapAroundEnding = 2402;
        public static int TrianglesDrawMode = 0;
        public static int TriangleStripDrawMode = 1;
        public static int TriangleFanDrawMode = 2;
        public static int LinearEncoding = 3000;
        public static int sRGBEncoding = 3001;
        public static int GammaEncoding = 3007;
        public static int RGBEEncoding = 3002;
        public static int LogLuvEncoding = 3003;
        public static int RGBM7Encoding = 3004;
        public static int RGBM16Encoding = 3005;
        public static int RGBDEncoding = 3006;
        public static int BasicDepthPacking = 3200;
        public static int RGBADepthPacking = 3201;
        public static int TangentSpaceNormalMap = 0;
        public static int ObjectSpaceNormalMap = 1;

        public static int ZeroStencilOp = 0;
        public static int KeepStencilOp = 7680;
        public static int ReplaceStencilOp = 7681;
        public static int IncrementStencilOp = 7682;
        public static int DecrementStencilOp = 7683;
        public static int IncrementWrapStencilOp = 34055;
        public static int DecrementWrapStencilOp = 34056;
        public static int InvertStencilOp = 5386;

        public static int NeverStencilFunc = 512;
        public static int LessStencilFunc = 513;
        public static int EqualStencilFunc = 514;
        public static int LessEqualStencilFunc = 515;
        public static int GreaterStencilFunc = 516;
        public static int NotEqualStencilFunc = 517;
        public static int GreaterEqualStencilFunc = 518;
        public static int AlwaysStencilFunc = 519;

        public static int StaticDrawUsage = 35044;
        public static int DynamicDrawUsage = 35048;
        public static int StreamDrawUsage = 35040;
        public static int StaticReadUsage = 35045;
        public static int DynamicReadUsage = 35049;
        public static int StreamReadUsage = 35041;
        public static int StaticCopyUsage = 35046;
        public static int DynamicCopyUsage = 35050;
        public static int StreamCopyUsage = 35042;
              
        public enum GLComtibility
        {
            ES2,
            ES3,
            ES3_1,
            ES3_2
        }
    }
}
