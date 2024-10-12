using Silk.NET.OpenGLES;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace THREE
{
    [Serializable]
    public class GLColorBuffer : IGLColorBuffer
    {
        private bool locked = false;

        private Vector4 color = new Vector4();

        private bool? currentColorMask;

        private Vector4 currentColorClear = new Vector4(0, 0, 0, 0);

        private GLState State;

        private GL gl => State.gl;

        public GLColorBuffer(GLState state)
        {
            this.State = state;
        }

        public void SetMask(bool colorMask)
        {
            if (currentColorMask != colorMask && !locked)
            {
                gl.ColorMask(colorMask, colorMask, colorMask, colorMask);
                currentColorMask = colorMask;
            }
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
        }

        public void SetClear(float r, float g, float b, float a, bool premultipliedAlpha = false)
        {
            if (premultipliedAlpha == true)
            {
                r *= a;
                g *= a;
                b *= a;
            }
            color.X = r; color.Y = g; color.Z = b; color.W = a;
            if (!currentColorClear.Equals(color))
            {
                gl.ClearColor(r, g, b, a);
                currentColorClear.Copy(color);
            }


        }
        public void Reset()
        {
            this.locked = false;
            this.currentColorMask = null;
            currentColorClear = new Vector4(-1, 0, 0, 0);
        }
    }
    public class GLDepthBuffer : IGLDepthBuffer
    {
        private bool locked = false;

        private bool? currentDepthMask;

        private int? currentDepthFunc;

        private float? currentDepthClear;

        private GLState State;

        private GL gl => State.gl;
        public GLDepthBuffer(GLState state)
        {
            this.State = state;
        }

        public void SetTest(bool depthTest)
        {
            if (depthTest)
            {
                State.Enable((int)EnableCap.DepthTest);
            }
            else
            {
                State.Disable((int)EnableCap.DepthTest);
            }
        }
        public void SetMask(bool depthMask)
        {
            if (currentDepthMask != depthMask && !locked)
            {
                gl.DepthMask(depthMask);
                currentDepthMask = depthMask;
            }
        }

        public void SetFunc(int depthFunc)
        {
            //DepthFunction func = (DepthFunction)Enum.ToObject(typeof(DepthFunction), depthFunc);

            if (currentDepthFunc != depthFunc)
            {
                if (depthFunc > 0)
                {
                    if (depthFunc == Constants.NeverDepth) gl.DepthFunc(DepthFunction.Never);

                    else if (depthFunc == Constants.AlwaysDepth) gl.DepthFunc(DepthFunction.Always);

                    else if (depthFunc == Constants.LessDepth) gl.DepthFunc(DepthFunction.Less);

                    else if (depthFunc == Constants.LessEqualDepth) gl.DepthFunc(DepthFunction.Lequal);

                    else if (depthFunc == Constants.EqualDepth) gl.DepthFunc(DepthFunction.Equal);

                    else if (depthFunc == Constants.GreaterEqualDepth) gl.DepthFunc(DepthFunction.Gequal);

                    else if (depthFunc == Constants.GreaterDepth) gl.DepthFunc(DepthFunction.Greater);

                    else if (depthFunc == Constants.NotEqualDepth) gl.DepthFunc(DepthFunction.Notequal);

                    else gl.DepthFunc(DepthFunction.Lequal);
                }
                else
                {
                    gl.DepthFunc(DepthFunction.Lequal);
                }
                currentDepthFunc = depthFunc;
            }
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
        }

        public void SetClear(float depth)
        {
            if (this.currentDepthClear != depth)
            {
                gl.ClearDepth(depth);
                currentDepthClear = depth;
            }
        }
        public void Reset()
        {
            this.locked = false;

            currentDepthMask = null;
            currentDepthFunc = null;
            currentDepthClear = null;

        }
    }
    public class GLStencilBuffer : IGLStencilBuffer
    {
        private bool locked = false;

        private int? currentStencilMask;

        private int? currentStencilFunc;

        private int? currentStencilRef;

        private int? currentStencilFuncMask;

        private int? currentStencilFail;

        private int? currentStencilZFail;

        private int? currentStencilZPass;

        private int? currentStencilClear;

        private GLState State;

        private GL gl => State.gl;
        public GLStencilBuffer(GLState state)
        {
            this.State = state;
        }

        public void SetTest(bool stencilTest)
        {
            if (!locked)
            {
                if (stencilTest)
                {
                    State.Enable((int)EnableCap.StencilTest);
                }
                else
                {
                    State.Disable((int)EnableCap.StencilTest);
                }
            }
        }

        public void SetMask(int stencilMask)
        {
            if (currentStencilMask != stencilMask && !locked)
            {
                gl.StencilMask((uint)stencilMask);
                currentStencilMask = stencilMask;
            }
        }

        public void SetFunc(int stencilFunc, int stencilRef, int stencilMask)
        {
            if (currentStencilFunc != stencilFunc ||
                currentStencilRef != stencilRef ||
                currentStencilFuncMask != stencilMask)
            {
                //All localTarget = (All)Enum.ToObject(typeof(All), (int)target + i);
                StencilFunction func = (StencilFunction)Enum.ToObject(typeof(StencilFunction), stencilFunc);
                gl.StencilFunc(func, stencilRef, (uint)stencilMask);

                currentStencilFunc = stencilFunc;
                currentStencilRef = stencilRef;
                currentStencilFuncMask = stencilMask;
            }
        }

        public void SetOp(int stencilFail, int stencilZFail, int stencilZPass)
        {
            if (currentStencilFail != stencilFail ||
                currentStencilZFail != stencilZFail ||
                currentStencilZPass != stencilZPass)
            {
                StencilOp fail = (StencilOp)Enum.ToObject(typeof(StencilOp), stencilFail);
                StencilOp zfail = (StencilOp)Enum.ToObject(typeof(StencilOp), stencilZFail);
                StencilOp zpass = (StencilOp)Enum.ToObject(typeof(StencilOp), stencilZPass);
                gl.StencilOp(fail, zfail, zpass);

                currentStencilFail = stencilFail;
                currentStencilZFail = stencilZFail;
                currentStencilZPass = stencilZPass;
            }
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
        }

        public void SetClear(int stencil)
        {
            if (currentStencilClear != stencil)
            {
                gl.ClearStencil(stencil);
                currentStencilClear = stencil;
            }
        }

        public void Reset()
        {
            locked = false;

            currentStencilMask = null;
            currentStencilFunc = null;
            currentStencilRef = null;
            currentStencilFuncMask = null;
            currentStencilFail = null;
            currentStencilZFail = null;
            currentStencilZPass = null;
            currentStencilClear = null;
        }
    }

    [Serializable]
    public struct BoundTexture
    {
        public int type;

        public int? texture;
    }

    [Serializable]
    public class GLStateBuffer : IGLStateBuffer
    {
        public IGLColorBuffer color { get; set; }

        public IGLDepthBuffer depth { get; set; }

        public IGLStencilBuffer stencil { get; set; }
    }
    public class GLState : DisposableObject, IGLState
    {
        
        public IGLStateBuffer buffers { get; set; } = new GLStateBuffer();

        private static Hashtable enabledCapabilities = new Hashtable();

        private GLColorBuffer colorBuffer;

        private GLDepthBuffer depthBuffer;

        private GLStencilBuffer stencilBuffer;

        private int maxVertexAttributes;// = GL.GetInteger(GetPName.MaxVertexAttribs);

        private byte[] newAttributes;

        private byte[] enabledAttributes;

        private byte[] attributeDivisors;



        private GLExtensions Extensions;

        private GLUtils Utils;

        private GLCapabilities Capabilities;

        private float version;

        private bool lineWidthAvailable = false;

        private int? currentTexturesSlot = null;

        private Vector4 currentScissor = new Vector4();

        private Vector4 currentViewport = new Vector4();

        private Dictionary<int, BoundTexture> currentBoundTextures = new Dictionary<int, BoundTexture>();

        private Dictionary<int, int> emptyTextures = new Dictionary<int, int>();

        private List<int> compressedTextureFormats;

        public int? currentProgram { get; set; }

        private bool currentBlendingEnabled;

        private int? currentBlending;

        private int? currentBlendEquation;

        private int? currentBlendEquationAlpha;

        private int? currentBlendSrc;

        private int? currentBlendDst;

        private int? currentBlendSrcAlpha;

        private int? currentBlendDstAlpha;

        private bool? currentPremultipliedAlpha;

        private bool? currentFlipSided;

        private int? currentCullFace;

        private float? currentLineWidth;

        private float? currentPolygonOffsetFactor;

        private float? currentPolygonOffsetUnits;

        private int maxTextures;

        public GL gl;
        public GLState(GL gl,GLExtensions extensions, GLUtils utils, GLCapabilities capabilities)
        {
            this.gl = gl;
            this.maxVertexAttributes = gl.GetInteger(GetPName.MaxVertexAttribs);
            this.Extensions = extensions;
            this.Utils = utils;
            this.Capabilities = capabilities;

            this.newAttributes = new byte[maxVertexAttributes];
            this.enabledAttributes = new byte[maxVertexAttributes];
            this.attributeDivisors = new byte[maxVertexAttributes];

            colorBuffer = new GLColorBuffer(this);

            depthBuffer = new GLDepthBuffer(this);

            stencilBuffer = new GLStencilBuffer(this);

            string[] glversion = gl.GetStringS(StringName.Version).Split(' ');

            if (glversion[1].IndexOf("OpenGL ES") > -1)
            {
                float.TryParse(glversion[0].Split('.')[0], out this.version);
                this.lineWidthAvailable = (this.version >= 2.0);
            }
            else
            {
                float.TryParse(glversion[0].Split('.')[0], out this.version);
                this.lineWidthAvailable = (this.version >= 1.0);
            }

            emptyTextures.Add((int)TextureTarget.Texture2D, this.CreateTexture(TextureTarget.Texture2D, TextureTarget.Texture2D, 1));
            emptyTextures.Add((int)TextureTarget.TextureCubeMap, this.CreateTexture(TextureTarget.TextureCubeMap, TextureTarget.TextureCubeMapPositiveX, 6));

            colorBuffer.SetClear(0, 0, 0, 1);
            depthBuffer.SetClear(1);
            stencilBuffer.SetClear(0);

            Enable((int)EnableCap.DepthTest);
            depthBuffer.SetFunc(Constants.LessEqualDepth);

            SetFlipSided(false);
            SetCullFace(Constants.CullFaceBack);
            Enable((int)EnableCap.CullFace);

            maxTextures = gl.GetInteger(GetPName.MaxCombinedTextureImageUnits);

            buffers.color = colorBuffer;
            buffers.depth = depthBuffer;
            buffers.stencil = stencilBuffer;
        }

        private unsafe int CreateTexture(TextureTarget type, TextureTarget target, int count)
        {
            byte[] data = new byte[4] { 0, 0, 0, 0 };

            uint texture;
            gl.GenTextures(1, out texture);
            gl.BindTexture(type, texture);
            gl.TexParameter(type, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameter(type, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            for (int i = 0; i < count; i++)
            {
                //IntPtr byteArrayPtr = IntPtr.Zero;
                //Marshal.Copy(byteArrayPtr, data, 0, data.Length);
                TextureTarget localTarget = (TextureTarget)Enum.ToObject(typeof(TextureTarget), (int)target + i);
                fixed (byte* ptr = data)
                {
                    gl.TexImage2D(localTarget, 0, InternalFormat.Rgba, (uint)1, (uint)1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }

            }
            return (int)texture;
        }
        public void InitAttributes()
        {
            for (int i = 0; i < newAttributes.Length; i++)
            {
                newAttributes[i] = 0;
            }
        }

        public void EnableAttribute(int attribute)
        {
            EnableAttributeAndDivisor(attribute, 0);
        }

        public void EnableAttributeAndDivisor(int attribute, int meshPerAttribute)
        {
            newAttributes[attribute] = 1;

            if (enabledAttributes[attribute] == 0)
            {
                gl.EnableVertexAttribArray((uint)attribute);
                enabledAttributes[attribute] = 1;
            }

            if (attributeDivisors[attribute] != meshPerAttribute)
            {
                attributeDivisors[attribute] = (byte)meshPerAttribute;
            }
        }

        public void DisableUnusedAttributes()
        {
            for (uint i = 0; i < enabledAttributes.Length; i++)
            {
                if (enabledAttributes[i] != newAttributes[i])
                {
                    gl.DisableVertexAttribArray(i);
                    enabledAttributes[i] = 0;
                }
            }
        }

        public void Enable(int enableCap)
        {
            if (enabledCapabilities.Contains((int)enableCap))
            {
                bool value = (bool)enabledCapabilities[(int)enableCap];
                if (value != true)
                {
                    gl.Enable((EnableCap)enableCap);
                    enabledCapabilities[(int)enableCap] = true;
                }
            }
            else
            {
                gl.Enable((EnableCap)enableCap);
                enabledCapabilities.Add((int)enableCap, true);
            }

        }

        public void Disable(int enableCap)
        {
            if (enabledCapabilities.Contains((int)enableCap))
            {
                bool value = (bool)enabledCapabilities[(int)enableCap];
                if (value != false)
                {
                    gl.Disable((EnableCap)enableCap);
                    enabledCapabilities[(int)enableCap] = false;
                }

            }
            else
            {
                gl.Disable((EnableCap)enableCap);
                enabledCapabilities.Add((int)enableCap, false);
            }
        }

        public List<int> GetCompressedTextureFormats()
        {
            if (this.compressedTextureFormats == null)
            {
                compressedTextureFormats = new List<int>();

                int[] format = new int[100];
                gl.GetInteger(GetPName.CompressedTextureFormats, format);

                for (int i = 0; i < format.Length; i++)
                {
                    if (format[i] != 0)
                    {
                        compressedTextureFormats[i] = format[i];
                    }
                }

            }
            return compressedTextureFormats;
        }

        public bool UseProgram(int program)
        {
            if (currentProgram != program)
            {
                gl.UseProgram((uint)program);
                currentProgram = program;
                return true;
            }
            return false;
        }

        public void SetBlending(int blending, int? blendEquation = null, int? blendSrc = null, int? blendDst = null, int? blendEquationAlpha = null, int? blendSrcAlpha = null, int? blendDstAlpha = null, bool? premultipliedAlpha = null)
        {
            if (blending == Constants.NoBlending)
            {
                if (currentBlendingEnabled)
                {
                    Disable((int)EnableCap.Blend);
                    currentBlendingEnabled = false;
                }
                return;
            }

            if (!currentBlendingEnabled)
            {
                Enable((int)EnableCap.Blend);
                currentBlendingEnabled = true;
            }

            if (blending != Constants.CustomBlending)
            {
                if (blending != currentBlending || premultipliedAlpha != currentPremultipliedAlpha)
                {
                    if (currentBlendEquation != Constants.AddEquation || currentBlendEquationAlpha != Constants.AddEquation)
                    {
                        gl.BlendEquation(GLEnum.FuncAdd);

                        currentBlendEquation = Constants.AddEquation;
                        currentBlendEquationAlpha = Constants.AddEquation;
                    }

                    if (premultipliedAlpha == true)
                    {
                        if (blending == Constants.NormalBlending)
                            gl.BlendFuncSeparate(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        else if (blending == Constants.AdditiveBlending)
                            gl.BlendFunc(BlendingFactor.One, BlendingFactor.One);
                        else if (blending == Constants.SubtractiveBlending)
                            gl.BlendFuncSeparate(BlendingFactor.Zero, BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor, BlendingFactor.OneMinusSrcAlpha);
                        else if (blending == Constants.MultiplyBlending)
                            gl.BlendFuncSeparate(BlendingFactor.Zero, BlendingFactor.SrcColor, BlendingFactor.Zero, BlendingFactor.SrcAlpha);
                        else
                            Trace.TraceError("THREE.gl.State : Invalid blending:" + blending);
                    }
                    else
                    {
                        if (blending == Constants.NormalBlending)
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        else if (blending == Constants.AdditiveBlending)
                            gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                        else if (blending == Constants.SubtractiveBlending)
                            gl.BlendFunc(BlendingFactor.Zero, BlendingFactor.OneMinusSrcAlpha);
                        else if (blending == Constants.MultiplyBlending)
                            gl.BlendFunc(BlendingFactor.Zero, BlendingFactor.SrcColor);
                        else
                            Trace.TraceError("THREE.gl.State : Invalid blending:" + blending);
                    }
                    currentBlendSrc = null;
                    currentBlendDst = null;
                    currentBlendSrcAlpha = null;
                    currentBlendDstAlpha = null;

                    currentBlending = blending;
                    currentPremultipliedAlpha = premultipliedAlpha;
                }

                return;

            }

            // custom blending

            blendEquationAlpha = blendEquationAlpha != null ? blendEquationAlpha : blendEquation;
            blendSrcAlpha = blendSrcAlpha != null ? blendSrcAlpha : blendSrc;
            blendDstAlpha = blendDstAlpha != null ? blendDstAlpha : blendDst;

            if (blendEquation != currentBlendEquation || blendEquationAlpha != currentBlendEquationAlpha)
            {
                gl.BlendEquationSeparate((BlendEquationModeEXT)Utils.Convert(blendEquation.Value), (BlendEquationModeEXT)Utils.Convert(blendEquationAlpha.Value));

                currentBlendEquation = blendEquation;
                currentBlendEquationAlpha = blendEquationAlpha;
            }

            if (blendSrc != currentBlendSrc || blendDst != currentBlendDst || blendSrcAlpha != currentBlendSrcAlpha || blendDstAlpha != currentBlendDstAlpha)
            {
                gl.BlendFuncSeparate((BlendingFactor)Utils.Convert(blendSrc.Value), (BlendingFactor)Utils.Convert(blendDst.Value), (BlendingFactor)Utils.Convert(blendSrcAlpha.Value), (BlendingFactor)Utils.Convert(blendDstAlpha.Value));

                currentBlendSrc = blendSrc;
                currentBlendDst = blendDst;
                currentBlendSrcAlpha = blendSrcAlpha;
                currentBlendDstAlpha = blendDstAlpha;
            }

            currentBlending = blending;
            currentPremultipliedAlpha = null;
        }

        public void SetMaterial(Material material, bool? frontFaceCW = null)
        {
            if (material.Side == Constants.DoubleSide)
            {
                //Disable(EnableCap.CullFace);
                gl.Disable(EnableCap.CullFace);
            }
            else
            {
                //Enable(EnableCap.CullFace);
                gl.Enable(EnableCap.CullFace);
            }

            var flipSided = material.Side == Constants.BackSide;

            if (frontFaceCW != null && frontFaceCW.Value) flipSided = !flipSided;

            SetFlipSided(flipSided);

            if (material.Blending == Constants.NormalBlending && material.Transparent == false)
            {
                SetBlending(Constants.NoBlending);
            }
            else
            {
                SetBlending(material.Blending, material.BlendEquation, material.BlendSrc, material.BlendDst, material.BlendEquationAlpha, material.BlendSrcAlpha, material.BlendDstAlpha, material.PremultipliedAlpha);
            }

            depthBuffer.SetFunc(material.DepthFunc);
            depthBuffer.SetTest(material.DepthTest);
            depthBuffer.SetMask(material.DepthWrite);
            colorBuffer.SetMask(material.ColorWrite);


            //if (material.DepthTest)
            gl.Enable(EnableCap.DepthTest);
            //else
            //    GL.Disable(EnableCap.DepthTest);

            var stencilWrite = material.StencilWrite;

            stencilBuffer.SetTest(stencilWrite);
            //if (stencilWrite)
            //{
            //    GL.Enable(EnableCap.StencilTest);
            //}
            //else
            //{
            //    GL.Disable(EnableCap.StencilTest);
            //}

            if (stencilWrite)
            {
                stencilBuffer.SetMask(material.StencilWriteMask);
                stencilBuffer.SetFunc(material.StencilFunc, material.StencilRef, material.StencilFuncMask);
                stencilBuffer.SetOp(material.StencilFail, material.StencilZFail, material.StencilZPass);
            }

            SetPolygonOffset(material.PolygonOffset, material.PolygonOffsetFactor, material.PolygonOffsetUnits);
        }

        public void SetFlipSided(bool flipSided)
        {
            if (currentFlipSided != flipSided)
            {
                if (flipSided)
                    gl.FrontFace(FrontFaceDirection.CW);
                else
                    gl.FrontFace(FrontFaceDirection.Ccw);

                currentFlipSided = flipSided;
            }
        }

        public void SetCullFace(int cullFace)
        {
            if (cullFace != Constants.CullFaceNone)
            {
                Enable((int)EnableCap.CullFace);
                if (cullFace != currentCullFace)
                {
                    if (cullFace == Constants.CullFaceBack)
                    {
                        gl.CullFace(GLEnum.Back);
                    }
                    else if (cullFace == Constants.CullFaceFront)
                    {
                        gl.CullFace(GLEnum.Front);
                    }
                    else
                    {
                        gl.CullFace(GLEnum.FrontAndBack);
                    }
                }
            }
            else
            {
                Disable((int)EnableCap.CullFace);
            }

            this.currentCullFace = cullFace;
        }

        public void SetLineWidth(float width)
        {
            if (width != currentLineWidth)
            {
                if (lineWidthAvailable) gl.LineWidth(width);

                currentLineWidth = width;

            }
        }

        public void SetPolygonOffset(bool polygonoffset, float? factor = null, float? units = null)
        {
            if (polygonoffset)
            {
                Enable((int)EnableCap.PolygonOffsetFill);

                if ((factor != null && currentPolygonOffsetFactor != factor) || (units != null && currentPolygonOffsetUnits != units))
                {
                    gl.PolygonOffset(factor.Value, units.Value);

                    currentPolygonOffsetFactor = factor;
                    currentPolygonOffsetUnits = units;
                }
            }
            else
            {
                Disable((int)EnableCap.PolygonOffsetFill);
            }
        }

        public void SetScissorTest(bool scissorTest)
        {
            if (scissorTest)
                Enable((int)EnableCap.ScissorTest);
            else
                Disable((int)EnableCap.ScissorTest);
        }

        public void ActiveTexture(int? glSlot = null)
        {
            if (glSlot == null) glSlot = (int)TextureUnit.Texture0 + maxTextures - 1;

            if (currentTexturesSlot != glSlot)
            {
                TextureUnit unit = (TextureUnit)Enum.ToObject(typeof(TextureUnit), glSlot);
                //if (glSlot > 34015)
                //{
                //    unit = TextureUnit.Texture31;
                //    glSlot = 34015;
                //}
                gl.ActiveTexture(unit);

                currentTexturesSlot = glSlot;
            }
        }

        public void BindTexture(int glType, int? glTexture)
        {
            if (currentTexturesSlot == null)
            {
                ActiveTexture();
            }

            BoundTexture? boundTexture = currentBoundTextures.ContainsKey(currentTexturesSlot.Value) ? (BoundTexture?)currentBoundTextures[currentTexturesSlot.Value] : null;

            if (boundTexture == null)
            {
                boundTexture = new BoundTexture();
                currentBoundTextures.Add(currentTexturesSlot.Value, boundTexture.Value);
            }

            if (boundTexture.Value.type != glType || boundTexture.Value.texture != glTexture)
            {
                TextureTarget target = (TextureTarget)Enum.ToObject(typeof(TextureTarget), glType);
                gl.BindTexture(target, glTexture != null ? (uint)glTexture.Value : (uint)emptyTextures[glType]);

                BoundTexture newBoundTexture = new BoundTexture();
                newBoundTexture.type = glType;
                newBoundTexture.texture = glTexture != null ? glTexture : null;
                currentBoundTextures[currentTexturesSlot.Value] = newBoundTexture;
            }
        }

        public void UnbindTexture()
        {
            BoundTexture? boundTexture = currentBoundTextures[currentTexturesSlot.Value];

            if (boundTexture != null && boundTexture.Value.type != -1)
            {
                TextureTarget target = (TextureTarget)Enum.ToObject(typeof(TextureTarget), boundTexture.Value.type);
                gl.BindTexture(target, 0);

                BoundTexture newBoundTexture = new BoundTexture();
                newBoundTexture.type = -1;
                newBoundTexture.texture = -1;
                currentBoundTextures[currentTexturesSlot.Value] = newBoundTexture;

            }
        }
        // Same interface as https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/compressedTexImage2D
        public void CompressedTexImage2D(int target, int level, int internalFormat, int width, int height, int border, byte[] data)
        {
            //GL.CompressedTexImage2D(target, level, internalFormat, width, height, border, data.Length, data);
            //CompressedTexImage2D(TextureTarget2d target, int level, CompressedInternalFormat internalformat, int width, int height, int border, int imageSize, IntPtr data);
            gl.CompressedTexImage2D<byte>((GLEnum)target, level, (GLEnum)internalFormat, (uint)width, (uint)height, border, (uint)data.Length, data);
        }

        // Same interface as https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/texImage2D
        public void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, byte[] pixels)
        {
            //GL.TexImage2D(target, level, internalFormat, width, height, border, format, type,pixels);
            gl.TexImage2D<byte>((GLEnum)target, level, internalFormat, (uint)width, (uint)height, border, (PixelFormat)format, (PixelType)type, pixels);
        }

        public void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, IntPtr pixels)
        {
            //GL.TexImage2D(target, level, internalFormat, width, height, border, format, type, pixels);
            //TexImage2D(TextureTarget2d target, int level, TextureComponentCount internalformat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr pixels);
            //GL.TexImage2D((TextureTarget2d)target, level, (TextureComponentCount)internalFormat, width, height, border, (PixelFormat)format, (PixelType)type, pixels);
            gl.TexImage2D((GLEnum)target, level, internalFormat, (uint)width, (uint)height, border, (PixelFormat)format, (PixelType)type, pixels);
            //GL.TexImage2D(target, level, internalformat, width, height, int border, All format, All type, IntPtr pixels);
        }

        public void TexImage3D(int target, int level, int internalFormat, int width, int height, int depth, int border, int format, int type, byte[] pixels)
        {
            //GL.TexImage3D(target, level, internalFormat, width, height, depth, border, format, type, pixels);
            //TexImage3D<T9>(TextureTarget3d target, int level, TextureComponentCount internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, T9[] pixels) where T9 : struct;
            gl.TexImage3D<byte>((TextureTarget)target, level, internalFormat, (uint)width, (uint)height, (uint)depth, border, (PixelFormat)format, (PixelType)type, pixels);
        }

        public void Scissor(Vector4 scissor)
        {
            if (!currentScissor.Equals(scissor))
            {
                gl.Scissor((int)scissor.X, (int)scissor.Y, (uint)scissor.Z, (uint)scissor.W);
                currentScissor.Copy(scissor);
            }
        }
        public void Viewport(Vector4 viewport)
        {
            if (!currentViewport.Equals(viewport))
            {
                gl.Viewport((int)viewport.X, (int)viewport.Y, (uint)viewport.Z, (uint)viewport.W);
                currentViewport.Copy(viewport);
            }
        }
        public void Reset()
        {
            for (uint i = 0; i < enabledAttributes.Length; i++)
            {
                gl.DisableVertexAttribArray(i);
                enabledAttributes[i] = 0;
            }

            enabledCapabilities.Clear();

            compressedTextureFormats = null;

            currentTexturesSlot = null;

            currentBoundTextures.Clear();

            currentProgram = null;

            currentBlending = null;

            currentFlipSided = null;

            currentCullFace = null;

            colorBuffer.Reset();
            depthBuffer.Reset();
            stencilBuffer.Reset();
        }
        public override void Dispose()
        {
            Reset();
            base.Dispose();
        }
    }
}
