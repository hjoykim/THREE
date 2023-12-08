using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace THREE
{
    public interface IColorBuffer
    {
        void SetMask(bool colorMask);
        void SetLocked(bool locked);
        void SetClear(float r, float g, float b, float a, bool premultipliedAlpha = false);
        void Reset();       
    }
    public interface IGLDepthBuffer
    {
        void SetTest(bool depthTest);
        void SetMask(bool depthMask);
        void SetFunc(int depthFunc);
        void SetLocked(bool locked);
        void SetClear(float depth);
        void Reset();
   
    }
    public interface IGLStencilBuffer
    {
        void SetTest(bool stencilTest);
        void SetMask(int stencilMask);
        void SetFunc(int stencilFunc, int stencilRef, int stencilMask);
        void SetOp(int stencilFail, int stencilZFail, int stencilZPass);

        void SetLocked(bool locked);
        void SetClear(int stencil);
        void Reset();
    }

    public interface IGLStateBuffer
    {
        IColorBuffer color { get; set; }
        IGLDepthBuffer depth { get; set; }
        IGLStencilBuffer stencil { get; set; }
    }
    public interface IGLState
    {

        IGLStateBuffer buffers { get; set; }
        public int? currentProgram { get; set; }
        void InitAttributes();
        void EnableAttribute(int attribute);
        void EnableAttributeAndDivisor(int attribute, int meshPerAttribute);
        void DisableUnusedAttributes();
        void Enable(int enableCap);
        void Disable(int enableCap);
        List<int> GetCompressedTextureFormats();
        bool UseProgram(int program);
        void SetBlending(int blending, int? blendEquation = null, int? blendSrc = null, int? blendDst = null, int? blendEquationAlpha = null, int? blendSrcAlpha = null, int? blendDstAlpha = null, bool? premultipliedAlpha = null);
        void SetMaterial(Material material, bool? frontFaceCW = null);
        void SetFlipSided(bool flipSided);
        void SetCullFace(int cullFace);
        void SetLineWidth(float width);
        void SetPolygonOffset(bool polygonoffset, float? factor = null, float? units = null);
        void SetScissorTest(bool scissorTest);
        void ActiveTexture(int? glSlot = null);
        void BindTexture(int glType, int? glTexture);
        void UnbindTexture();
        void CompressedTexImage2D(int target, int level, int internalFormat, int width, int height, int border, byte[] data);
        void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, PixelFormat format, int type, byte[] pixels);
        void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, PixelFormat format, int type, IntPtr pixels);
        void TexImage3D(int target, int level, int internalFormat, int width, int height, int depth, int border, int format, int type, byte[] pixels);
        void Scissor(Vector4 scissor);
        void Viewport(Vector4 viewport);
        void Reset();
    }
}
