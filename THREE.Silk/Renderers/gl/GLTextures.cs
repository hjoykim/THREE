using Silk.NET.OpenGLES;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace THREE
{
    [Serializable]
    public class GLTextures : DisposableObject,IGLTextures
    {
        private bool IsGL2;

        private int maxTextures;

        private int maxCubemapSize;

        private int maxTextureSize;

        private int maxSample;

        private GLRenderer renderer;

        private GL gl ;
        private Hashtable videoTextures = new Hashtable();

        private int textureUnits = 0;

        GLProperties properties;

        GLCapabilities capabilities;
        GLState state;

        GLExtensions extensions;

        GLUtils utils;

        GLInfo info;

        public GLTextures(GL gl, GLExtensions extensions, GLState state, GLProperties properties, GLCapabilities capabilities, GLUtils util, GLInfo info)
        {
            this.gl = gl;

            this.IsGL2 = capabilities.IsGL2;

            this.maxTextures = capabilities.maxTextures;

            this.maxCubemapSize = capabilities.maxCubemapSize;

            this.maxTextureSize = capabilities.maxTextureSize;

            this.maxSample = capabilities.maxSamples;

            this.properties = properties;

            this.capabilities = capabilities;

            this.state = state;

            this.extensions = extensions;

            this.utils = util;

            this.info = info;
        }

        private SKBitmap ResizeImage(SKBitmap image, int width, int height)
        {
            return image.Resize(new SKImageInfo(width,height), SKFilterQuality.High);
            //var destRect = new Rectangle(0, 0, width, height);
            //var destImage = new Bitmap(width, height);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //using (var graphics = Graphics.FromImage(destImage))
            //{
            //    graphics.CompositingMode = CompositingMode.SourceCopy;
            //    graphics.CompositingQuality = CompositingQuality.HighQuality;
            //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    graphics.SmoothingMode = SmoothingMode.HighQuality;
            //    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //    using (var wrapMode = new ImageAttributes())
            //    {
            //        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            //        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            //    }
            //}

            //return destImage;
        }

        //private Bitmap ResizeImage(Bitmap image, int width, int height)
        //{
        //    Size resize = new Size(width, height);
        //    return new Bitmap(image, resize);
        //}
        private SKBitmap ResizeImage(SKBitmap image, bool needsPowerOfTwo, bool needsNewCanvas, int maxSize)
        {
            float scale = 1.0f;

            if (image == null) return image;

            if (image.Width > maxSize || image.Height > maxSize)
            {
                scale = maxSize / (float)System.Math.Max(image.Width, image.Height);
            }

            if (scale <= 1 || needsPowerOfTwo == true)
            {
                if (image is SKBitmap)
                {
                    var width = needsPowerOfTwo ? MathUtils.FloorPowerOfTwo(scale * image.Width) : (float)System.Math.Floor(scale * image.Width);
                    var height = needsPowerOfTwo ? MathUtils.FloorPowerOfTwo(scale * image.Height) : (float)System.Math.Floor(scale * image.Height);


                    image = ResizeImage(image, (int)width, (int)height);

                    return image;

                }
                else
                {
                    return image;
                }
            }
            return image;
        }

        private bool IsPowerOfTwo(SKBitmap image)
        {
            if (image == null) return false;
            return MathUtils.IsPowerOfTwo(image.Width) && MathUtils.IsPowerOfTwo(image.Height);

        }

        private bool IsPowerOfTwo(GLRenderTarget image)
        {
            if (image == null) return false;
            return MathUtils.IsPowerOfTwo(image.Width) && MathUtils.IsPowerOfTwo(image.Height);

        }

        private bool TextureNeedsPowerOfTwo(Texture texture)
        {
            if (IsGL2) return false;

            return (texture.WrapS != Constants.ClampToEdgeWrapping || texture.WrapT != Constants.ClampToEdgeWrapping) ||
                (texture.MinFilter != Constants.NearestFilter && texture.MinFilter != Constants.LinearFilter);
        }

        private bool TextureNeedsGenerateMipmaps(Texture texture, bool supportsMips)
        {
            return texture.GenerateMipmaps && supportsMips && texture.MinFilter != Constants.NearestFilter && texture.MinFilter != Constants.LinearFilter;
        }

        private void GenerateMipmap(TextureTarget target, Texture texture, int width, int height)
        {
            gl.GenerateMipmap(target);

            Hashtable textureProperties = properties.Get(texture);

            var value = System.Math.Log(System.Math.Max(width, height)) * System.Math.Log(System.Math.E, 2);
            if (textureProperties.ContainsKey("maxMipLevel"))
            {
                textureProperties["maxMipLevel"] = (int)value;
            }
            else
            {
                textureProperties.Add("maxMipLevel", (int)value);
            }
        }
        private int GetInternalFormat(string internalFormatName, int glFormat, int glType)
        {
            if (IsGL2 == false) return glFormat;

            if (internalFormatName != null)
            {
                //if (_gl[internalFormatName] != null) return _gl[internalFormatName];
                //console.warn('THREE.WebGLRenderer: Attempt to use non-existing WebGL internal format \'' + internalFormatName + '\'');
            }

            var internalFormat = glFormat;

            if (glFormat == (int)GLEnum.Red)
            {
                if (glType == (int)GLEnum.Float) internalFormat = (int)GLEnum.R32f;
                if (glType == (int)GLEnum.HalfFloat) internalFormat = (int)GLEnum.R16f;
                if (glType == (int)GLEnum.UnsignedByte) internalFormat = (int)GLEnum.R8;

            }

            if (glFormat == (int)GLEnum.Rgb)
            {

                if (glType == (int)GLEnum.Float) internalFormat = (int)GLEnum.Rgb32f;
                if (glType == (int)GLEnum.HalfFloat) internalFormat = (int)GLEnum.Rgb16f;
                if (glType == (int)GLEnum.UnsignedByte) internalFormat = (int)GLEnum.Rgb8;

            }

            if (glFormat == (int)GLEnum.Rgba)
            {

                if (glType == (int)GLEnum.Float) internalFormat = (int)GLEnum.Rgba32f;
                if (glType == (int)GLEnum.HalfFloat) internalFormat = (int)GLEnum.Rgba16f;
                if (glType == (int)GLEnum.UnsignedByte) internalFormat = (int)GLEnum.Rgba8;

            }

            if (internalFormat == (int)GLEnum.R16f || internalFormat == (int)GLEnum.R32f ||
                internalFormat == (int)GLEnum.Rgba16f || internalFormat == (int)GLEnum.Rgba32f)
            {

                extensions.Get("EXT_color_buffer_float");

            }
            else if (internalFormat == (int)GLEnum.Rgb16f || internalFormat == (int)GLEnum.Rgb32f)
            {

                Trace.TraceWarning("THREE.GLRenderer: Floating point textures with RGB format not supported. Please use RGBA instead.");

            }

            return internalFormat;
        }

        private int FilterFallback(int f)
        {
            if (f == Constants.NearestFilter || f == Constants.NearestMipmapNearestFilter || f == Constants.NearestMipmapLinearFilter)
            {
                return (int)GLEnum.Nearest;
            }
            return (int)GLEnum.Linear;
        }

        private void TextureDispose()
        {

        }

        public void DeallocateTexture(Texture texture)
        {
            var textureProperties = properties.Get(texture);

            if (!textureProperties.ContainsKey("glInit")) return;

            gl.DeleteTexture((uint)textureProperties["glTexture"]);

            properties.Remove(texture);
        }

        public void DeallocateRenderTarget(GLRenderTarget renderTarget)
        {
            //if (!this.renderer.IsCurrent) return;
            if (renderTarget == null) return;

            var renderTargetProperties = properties.Get(renderTarget);
            var textureProperties = properties.Get(renderTarget.Texture);

            if (textureProperties.ContainsKey("glTexture"))
            {
                gl.DeleteTexture((uint)textureProperties["glTexture"]);
            }

            if (renderTarget.depthTexture != null)
            {
                renderTarget.depthTexture.Dispose();
            }

            if (renderTarget is GLCubeRenderTarget)
            {
                uint[] bufferList = (uint[])renderTargetProperties["glFramebuffer"];
                uint[] depthList = (uint[])renderTargetProperties["glDepthbuffer"];
                for (int i = 0; i < 6; i++)
                {
                    if(bufferList!=null) gl.DeleteFramebuffer(bufferList[i]);
                    if (depthList != null) gl.DeleteRenderbuffer(depthList[i]);
                }
            }
            else
            {
                if(renderTargetProperties.ContainsKey("glFramebuffer"))  gl.DeleteFramebuffer((uint)renderTargetProperties["glFramebuffer"]);
                if (renderTargetProperties.ContainsKey("glDepthbuffer")) gl.DeleteRenderbuffer((uint)renderTargetProperties["glDepthbuffer"]);
            }

            if (renderTarget is GLMultiviewRenderTarget)
            {
                gl.DeleteTexture((uint)renderTargetProperties["glColorTexture"]);
                gl.DeleteTexture((uint)renderTargetProperties["glDepthStencilTexture"]);

                info.memory.Textures -= 2;

                uint[] framebuffers = (uint[])renderTargetProperties["glViewFramebuffers"];

                for (int i = 0; i < framebuffers.Length; i++)
                {
                    gl.DeleteFramebuffer(framebuffers[i]);
                }

            }

            properties.Remove(renderTarget.Texture);
            properties.Remove(renderTarget);
        }

        public void SetTexture2D(Texture texture, int slot)
        {
            var textureProperties = properties.Get(texture);

            if (texture is VideoTexture) UpdateVideoTexture((VideoTexture)texture);

            int textureVersion = textureProperties.ContainsKey("version") ? (int)textureProperties["version"] : -1;

            if (texture.version > 0 && textureVersion != texture.version)
            {

                if (texture.Image == null && texture.ImageSize.Width > 0 && texture.ImageSize.Height > 0 && !(texture is DataTexture))
                {
                    byte[] data = new byte[texture.ImageSize.Width * texture.ImageSize.Height*4];
                    //Bitmap bitmap = new Bitmap(texture.ImageSize.Width, texture.ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    //BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, texture.ImageSize.Width, texture.ImageSize.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    //IntPtr iptr = bitmapData.Scan0;
                    //Marshal.Copy(iptr, data, 0, data.Length);

                    //bitmap.UnlockBits(bitmapData);

                    texture.Image = data.ToSKBitMap(texture.ImageSize.Width,texture.ImageSize.Height);
                }
                if (texture.Image == null && !(texture is DataTexture))
                {
                    Trace.TraceWarning("THREE.Renderers.gl.GLTextures.SetTexture2D : Texture marked for update but image is undefined");
                }
                else
                {
                    UploadTexture(textureProperties, texture, slot);
                    return;
                }
            }

            state.ActiveTexture((int)TextureUnit.Texture0 + slot);

            state.BindTexture((int)TextureTarget.Texture2D, (int?)(uint)textureProperties["glTexture"]);

        }

        public void SetTexture2DArray(Texture texture, int slot)
        {
            var textureProperties = properties.Get(texture);

            if (texture.version > 0 && (int)textureProperties["version"] != texture.version)
            {
                UploadTexture(textureProperties, texture, slot);
                return;
            }

            state.ActiveTexture((int)TextureUnit.Texture0 + slot);
            state.BindTexture((int)TextureTarget.Texture2D, (int)textureProperties["glTexture"]);
        }

        public void SetTexture3D(Texture texture, int slot)
        {
            var textureProperties = properties.Get(texture);
            if (texture.version > 0 && (int)textureProperties["version"] != texture.version)
            {
                UploadTexture(textureProperties, texture, slot);
                return;
            }

            state.ActiveTexture((int)TextureUnit.Texture0 + slot);
            state.BindTexture((int)TextureTarget.Texture3D, (int)textureProperties["glTexture"]);

        }
        public void SetTextureCube(Texture texture, int slot)
        {
            if (texture.Images.Length != 6) return;

            var textureProperties = properties.Get(texture);

            int textureVersion = textureProperties.ContainsKey("version") ? (int)textureProperties["version"] : -1;

            if (texture.version > 0 && textureVersion != texture.version)
            {

                InitTexture(textureProperties, texture);

                state.ActiveTexture((int)TextureUnit.Texture0 + slot);
                int textureId = (int)(uint)textureProperties["glTexture"];
                state.BindTexture((int)TextureTarget.TextureCubeMap, textureId);

                //gl.PixelStore(_gl.UNPACK_FLIP_Y_WEBGL, texture.flipY );

                var isCompressed = (texture != null && texture is CompressedTexture);
                var isDataTexture = (texture.Images != null && texture.Images.Length > 0 && texture.Images[0] is DataTexture);

                var cubeImage = new List<object>();

                for (var i = 0; i < 6; i++)
                {

                    if (!isCompressed && !isDataTexture)
                    {

                        cubeImage.Add(ResizeImage(texture.Images[i].Image, false, true, maxCubemapSize));

                    }
                    else
                    {
                        if (isDataTexture)
                            cubeImage.Add(texture.Images[i].Image);
                        else
                            cubeImage.Add(texture.Images[i]);

                    }

                }

                var image = cubeImage[0];

                bool supportsMipsMap = image is SKBitmap ? IsPowerOfTwo((SKBitmap)image) : IsPowerOfTwo((image as Texture).Image);
                bool supportsMips = supportsMipsMap ? supportsMipsMap : IsGL2;

                GLEnum glFormat = utils.Convert(texture.Format);
                GLEnum glType = utils.Convert(texture.Type);
                int glInternalFormat = GetInternalFormat(texture.InternalFormat, (int)glFormat, (int)glType);

                SetTextureParameters(TextureTarget.TextureCubeMap, texture, supportsMips);

                List<TextureTarget> targets = new List<TextureTarget>
                    {
                    TextureTarget.TextureCubeMapPositiveX,

                    TextureTarget.TextureCubeMapNegativeX,

                    TextureTarget.TextureCubeMapPositiveY,

                    TextureTarget.TextureCubeMapNegativeY,

                    TextureTarget.TextureCubeMapPositiveZ,

                    TextureTarget.TextureCubeMapNegativeZ
                       //TextureTarget2d.TextureCubeMapNegativeX, TextureTarget2d.TextureCubeMapNegativeY,
                       //TextureTarget2d.TextureCubeMapNegativeZ, TextureTarget2d.TextureCubeMapPositiveX,
                       //TextureTarget2d.TextureCubeMapPositiveY, TextureTarget2d.TextureCubeMapPositiveZ
                    };

                if (isCompressed)
                {

                    List<MipMap> mipmaps = null;

                    for (var i = 0; i < 6; i++)
                    {

                        mipmaps = (cubeImage[i] as Texture).Mipmaps;

                        for (var j = 0; j < mipmaps.Count; j++)
                        {

                            var mipmap = mipmaps[j];

                            if (texture.Format != Constants.RGBAFormat && texture.Format != Constants.RGBFormat)
                            {

                                //if ( glFormat != null ) {

                                state.CompressedTexImage2D((int)targets[i], j, glInternalFormat, mipmap.Width, mipmap.Height, 0, mipmap.Data);

                                //} else {

                                //    console.warn( 'THREE.WebGLRenderer: Attempt to load unsupported compressed texture format in .setTextureCube()' );

                                //}

                            }
                            else
                            {

                                state.TexImage2D((int)targets[i], j,glInternalFormat, mipmap.Width, mipmap.Height, 0,(int)glFormat, (int)glType, mipmap.Data);

                            }

                        }

                    }


                    textureProperties["maxMipLevel"] = mipmaps.Count - 1;

                }
                else
                {

                    List<MipMap> mipmaps = texture.Mipmaps;

                    for (var i = 0; i < 6; i++)
                    {
                        SKBitmap localImage = cubeImage[i] as SKBitmap;
                        if (isDataTexture)
                        {
                            state.TexImage2D((int)targets[i], 0, glInternalFormat, localImage.Width, localImage.Height, 0, (int)glFormat, (int)glType, localImage.Bytes);

                            for (var j = 0; j < mipmaps.Count; j++)
                            {
                                var mipmap = mipmaps[j];
                                var mipmapImage = mipmap.Data;

                                state.TexImage2D((int)targets[i], j + 1, glInternalFormat, mipmap.Width, mipmap.Height, 0, (int)glFormat, (int)glType, mipmapImage);
                            }
                        }
                        else
                        {


                            //state.TexImage2D(targets[i], 0, (TextureComponentCount)glInternalFormat, data.Width,data.Height,0,(OpenTK.Graphics.ES30.PixelFormat)glFormat, (PixelType)glType, data.Scan0);
                            state.TexImage2D((int)targets[i], 0, glInternalFormat, localImage.Width, localImage.Height, 0,(int)PixelFormat.BgraImg, (int)glType, localImage.Bytes);

                            for (var j = 0; j < mipmaps.Count; j++)
                            {

                                var mipmap = mipmaps[j];
                                var mipmapImage = mipmap.Data;
                                state.TexImage2D((int)targets[i], j + 1, glInternalFormat, mipmap.Width, mipmap.Height, 0, (int)glFormat, (int)glType, mipmapImage);

                            }
                        }

                    }

                    textureProperties["maxMipLevel"] = mipmaps.Count;

                }

                if (TextureNeedsGenerateMipmaps(texture, supportsMips))
                {

                    // We assume images for cube map have the same size.
                    GenerateMipmap(TextureTarget.TextureCubeMap, texture, (image as SKBitmap).Width, (image as SKBitmap).Height);

                }

                textureProperties["version"] = texture.version;

                //if ( texture.onUpdate ) texture.onUpdate( texture );

            }
            else
            {

                state.ActiveTexture((int)TextureUnit.Texture0 + slot);
                state.BindTexture((int)TextureTarget.TextureCubeMap, (int)(uint)textureProperties["glTexture"]);

            }
        }

        public void SetTextureCubeDynamic(Texture texture, int slot)
        {
            state.ActiveTexture((int)TextureUnit.Texture0 + slot);
            state.BindTexture((int)TextureTarget.TextureCubeMap, (int)(uint)properties.Get(texture)["glTexture"]);
        }

        private int WrappingToGL(int wrap)
        {
            if (Constants.RepeatWrapping == wrap)
                return (int)GLEnum.Repeat;
            else if (Constants.ClampToEdgeWrapping == wrap)
                return (int)GLEnum.ClampToEdge;
            else if (Constants.MirroredRepeatWrapping == wrap)
                return (int)GLEnum.MirroredRepeat;
            else return (int)GLEnum.Repeat;
        }
        //List<int> wrappingToGL = new List<int>()
        //    {
        //         (int)GLEnum.Repeat,
        //         (int)GLEnum.ClampToEdge,
        //         (int)GLEnum.MirroredRepeat
        //        //{ "RepeatWrapping" , (int)GLEnum.Repeat},
        //        //{ "ClampToEdgeWrapping", (int)GLEnum.ClampToEdge},
        //        //{ "MirroredRepeatWrapping", (int)GLEnum.MirroredRepeat}
        //    };

        private int FilterToGL(int filter)
        {
            if (Constants.NearestFilter == filter)
                return (int)GLEnum.Nearest;

            else if (Constants.NearestMipmapNearestFilter == filter)
                return (int)GLEnum.NearestMipmapNearest;

            else if (Constants.NearestMipmapLinearFilter == filter)
                return (int)GLEnum.NearestMipmapLinear;

            else if (Constants.LinearFilter == filter)
                return (int)GLEnum.Linear;

            else if (Constants.LinearMipmapNearestFilter == filter)
                return (int)GLEnum.LinearMipmapNearest;

            else if (Constants.LinearMipmapLinearFilter == filter)
                return (int)GLEnum.LinearMipmapLinear;

            else
                return (int)GLEnum.Nearest;
        }
        List<int> filterToGL = new List<int>()
            {
                (int)GLEnum.Nearest,
                (int)GLEnum.NearestMipmapNearest,
                (int)GLEnum.NearestMipmapLinear,

                (int)GLEnum.Linear,
                (int)GLEnum.LinearMipmapLinear,
                (int)GLEnum.LinearMipmapLinear
                //{"NearestFilter", (int)GLEnum.Nearest},
                //{"NearestMipmapNearestFilter", (int)GLEnum.NearestMipmapNearest},
                //{"NearestMipmapLinearFilter", (int)GLEnum.NearestMipmapLinear},

                //{"LinearFilter", (int)GLEnum.Linear},
                //{"LinearMipmapNearestFilter", (int)GLEnum.LinearMipmapLinear},
                //{"LinearMipmapLinearFilter", (int)GLEnum.LinearMipmapLinear}
            };
        private void SetTextureParameters(TextureTarget textureType, Texture texture, bool supportsMips)
        {
            if (supportsMips)
            {
                gl.TexParameter(textureType, TextureParameterName.TextureWrapS, WrappingToGL(texture.WrapS));
                gl.TexParameter(textureType, TextureParameterName.TextureWrapT, WrappingToGL(texture.WrapT));

                if (textureType == TextureTarget.Texture3D || textureType == TextureTarget.Texture2DArray)
                {

                    gl.TexParameter(textureType, TextureParameterName.TextureWrapR, WrappingToGL(texture.WrapR));

                }

                gl.TexParameter(textureType, TextureParameterName.TextureMagFilter, FilterToGL(texture.MagFilter));
                gl.TexParameter(textureType, TextureParameterName.TextureMinFilter, FilterToGL(texture.MinFilter));
            }
            else
            {
                gl.TexParameter(textureType, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
                gl.TexParameter(textureType, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

                if (textureType == TextureTarget.Texture3D || textureType == TextureTarget.Texture2DArray)
                {

                    gl.TexParameter(textureType, TextureParameterName.TextureWrapR, (int)GLEnum.ClampToEdge);

                }

                if (texture.WrapS != Constants.ClampToEdgeWrapping || texture.WrapT != Constants.ClampToEdgeWrapping)
                {

                    Trace.TraceWarning("THREE.GLRenderer: Texture is not power of two. Texture.wrapS and Texture.wrapT should be set to THREE.ClampToEdgeWrapping.");

                }

                gl.TexParameter(textureType, TextureParameterName.TextureMagFilter, FilterFallback(texture.MagFilter));
                gl.TexParameter(textureType, TextureParameterName.TextureMinFilter, FilterFallback(texture.MinFilter));

                if (texture.MinFilter != Constants.NearestFilter && texture.MinFilter != Constants.LinearFilter)
                {

                    Trace.TraceWarning("THREE.GLRenderer: Texture is not power of two. Texture.minFilter should be set to THREE.NearestFilter or THREE.LinearFilter.");

                }
            }

            var extension = extensions.Get("GL_EXT_texture_filter_anisotropic");

            if (extension > -1)
            {

                if (texture.Type == Constants.FloatType && extensions.Get("GL_OES_texture_float_linear") == -1) return;
                if (texture.Type == Constants.HalfFloatType && (IsGL2 || extensions.Get("GL_OES_texture_half_float_linear") == -1)) return;

                if (texture.Anisotropy > 1 || (properties.Get(texture) as Hashtable)["currentAnisotropy"] != null)
                {
                    gl.TexParameter(textureType, (GLEnum)2, System.Math.Min(texture.Anisotropy, capabilities.GetMaxAnisotropy()));
                    (properties.Get(texture))["currentAnisotropy"] = texture.Anisotropy;
                }

            }
        }

        private void InitTexture(Hashtable textureProperties, Texture texture)
        {
            if (!textureProperties.ContainsKey("glInit"))
            {
                textureProperties.Add("glInit", true);
                texture.Disposed += (o, e) =>
                {
                    DeallocateTexture(texture);
                    info.memory.Textures--;
                };
                if (!textureProperties.Contains("glTexture"))
                    textureProperties.Add("glTexture", gl.GenTexture());

                info.memory.Textures++;
            }
        }


        private unsafe void UploadTexture(Hashtable textureProperties, Texture texture, int slot)
        {
            var textureType = TextureTarget.Texture2D;

            if (texture is DataTexture2DArray) textureType = TextureTarget.Texture2DArray;
            if (texture is DataTexture2DArray) textureType = TextureTarget.Texture3D;

            InitTexture(textureProperties, texture);

            state.ActiveTexture((int)TextureUnit.Texture0 + slot);
            state.BindTexture((int)textureType, (int)(uint)textureProperties["glTexture"]);

            //gl.PixelStore(PixelStoreParameter.UnPackFlipY, texture.flipY ? 1 : 0);
            //gl.PixelStore(PixelStoreParameter.UnpackPremultiplyAlpha, texture.PremultiplyAlpha?1:0);
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, texture.UnpackAlignment);

            var needsPowerOfTwo = TextureNeedsPowerOfTwo(texture) && IsPowerOfTwo(texture.Image) == false;

            var image = ResizeImage(texture.Image, needsPowerOfTwo, false, maxTextureSize);

            bool supportsMips = IsPowerOfTwo(image) ? true : IsGL2;
            GLEnum glFormat = utils.Convert(texture.Format);
            GLEnum glType = utils.Convert(texture.Type);
            int glInternalFormat = GetInternalFormat(texture.InternalFormat, (int)glFormat, (int)glType);

            SetTextureParameters(textureType, texture, supportsMips);

            MipMap mipmap;
            var mipmaps = texture.Mipmaps;

            if (texture is DepthTexture)
            {
                glInternalFormat = (int)GLEnum.DepthComponent;

                if (texture.Type == Constants.FloatType)
                {
                    if (IsGL2 == false)
                        throw new Exception("Float Depth Texture only supported in WebGL2.0");

                    glInternalFormat = (int)GLEnum.DepthComponent32f;
                }
                else if (IsGL2)
                {

                    // WebGL 2.0 requires signed internalformat for glTexImage2D
                    glInternalFormat = (int)GLEnum.DepthComponent16;
                }
                if (texture.Format == Constants.DepthFormat && glInternalFormat == (int)GLEnum.DepthComponent)
                {

                    // The error INVALID_OPERATION is generated by texImage2D if format and internalformat are
                    // DEPTH_COMPONENT and type is not UNSIGNED_SHORT or UNSIGNED_INT
                    // (https://www.khronos.org/registry/webgl/extensions/WEBGL_depth_texture/)
                    if (texture.Type != Constants.UnsignedShortType && texture.Type != Constants.UnsignedIntType)
                    {

                        Trace.TraceWarning("THREE.WebGLRenderer: Use UnsignedShortType or UnsignedIntType for DepthFormat DepthTexture.");

                        texture.Type = Constants.UnsignedShortType;
                        glType = utils.Convert(texture.Type);

                    }

                }

                // Depth stencil textures need the DEPTH_STENCIL internal format
                // (https://www.khronos.org/registry/webgl/extensions/WEBGL_depth_texture/)
                if (texture.Format == Constants.DepthStencilFormat)
                {

                    glInternalFormat = (int)GLEnum.DepthStencil;

                    // The error INVALID_OPERATION is generated by texImage2D if format and internalformat are
                    // DEPTH_STENCIL and type is not UNSIGNED_INT_24_8_WEBgl.
                    // (https://www.khronos.org/registry/webgl/extensions/WEBGL_depth_texture/)
                    if (texture.Type != Constants.UnsignedInt248Type)
                    {

                        Trace.TraceWarning("THREE.WebGLRenderer: Use UnsignedInt248Type for DepthStencilFormat DepthTexture.");

                        texture.Type = Constants.UnsignedInt248Type;
                        glType = utils.Convert(texture.Type);

                    }

                }
                state.TexImage2D((int)TextureTarget.Texture2D, 0, glInternalFormat, image.Width, image.Height, 0, (int)glFormat, (int)glType, null);
            }
            else if (texture is DataTexture)
            {

                // use manually created mipmaps if available
                // if there are no manual mipmaps
                // set 0 level mipmap and then use GL to generate other mipmap levels

                if (mipmaps.Count > 0 && supportsMips)
                {

                    for (int i = 0; i < mipmaps.Count; i++)
                    {

                        mipmap = mipmaps[i];
                        state.TexImage2D((int)TextureTarget.Texture2D, i, glInternalFormat, mipmap.Width, mipmap.Height, 0, (int)glFormat, (int)glType, mipmap.Data);

                    }

                    texture.GenerateMipmaps = false;
                    textureProperties["maxMipLevel"] = mipmaps.Count - 1;

                }
                else
                {
                    //var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);//System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    //state.TexImage2D(TextureTarget2d.Texture2D, 0, (TextureComponentCount)glInternalFormat, image.Width, image.Height, 0, (OpenTK.Graphics.ES30.PixelFormat)glFormat, (PixelType)glType, data.Scan0);
                    if (texture.Image != null)
                    {
                        //Bitmap image1 = (Bitmap)image.Clone();
                        //var data = image1.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);//System.Drawing.Imaging.PixelFormat.Format32bppArgb);                   
                        //image1.UnlockBits(data);
                        byte[] data = new byte[image.Width * image.Height * 4];
                        state.TexImage2D((int)GLEnum.Texture2D, 0, glInternalFormat, image.Width, image.Height, 0, (int)PixelFormat.Bgra, (int)glType, data.ToSKBitMap(image.Width, image.Height).Bytes);
                        textureProperties["maxMipLevel"] = 0;
                    }
                    else
                    {
                        if ((texture as DataTexture).byteData != null || (texture as DataTexture).intData != null || (texture as DataTexture).floatData != null)
                        {
                            switch ((texture as DataTexture).Type)
                            {
                                case 1015:// Constants.FloatType:
                                    fixed (float* p = &(texture as DataTexture).floatData[0])
                                    {
                                        gl.TexImage2D(GLEnum.Texture2D, 0, glInternalFormat, (uint)(texture as DataTexture).ImageSize.Width, (uint)(texture as DataTexture).ImageSize.Height, 0, GLEnum.Rgba, glType, p);
                                        textureProperties["maxMipLevel"] = 0;
                                    }
                                    break;
                                case 1013: //Constants.IntType:
                                    fixed (int* p = &(texture as DataTexture).intData[0])
                                    {
                                        gl.TexImage2D(GLEnum.Texture2D, 0, glInternalFormat, (uint)(texture as DataTexture).ImageSize.Width, (uint)(texture as DataTexture).ImageSize.Height, 0, GLEnum.Rgba, glType, p);
                                        textureProperties["maxMipLevel"] = 0;
                                    }
                                    break;
                                case 1010: //Constants.ByteType:
                                    fixed (byte* p = &(texture as DataTexture).byteData[0])
                                    {
                                        gl.TexImage2D(GLEnum.Texture2D, 0, glInternalFormat, (uint)(texture as DataTexture).ImageSize.Width, (uint)(texture as DataTexture).ImageSize.Height, 0, GLEnum.Rgba, glType, p);
                                        textureProperties["maxMipLevel"] = 0;
                                    }
                                    break;
                            }
                        }
                    }

                }

            }
            else if (texture is CompressedTexture)
            {

                for (var i = 0; i <= mipmaps.Count; i++)
                {

                    mipmap = mipmaps[i];

                    if (texture.Format != Constants.RGBAFormat && texture.Format != Constants.RGBFormat)
                    {

                        if (glFormat != null)
                        {

                            state.CompressedTexImage2D((int)GLEnum.Texture2D, i, glInternalFormat, mipmap.Width, mipmap.Height, 0, mipmap.Data);

                        }
                        else
                        {

                            Trace.TraceWarning("THREE.GLRenderer: Attempt to load unsupported compressed texture format in .uploadTexture()");

                        }

                    }
                    else
                    {

                        state.TexImage2D((int)GLEnum.Texture2D, i, glInternalFormat, mipmap.Width, mipmap.Height, 0, (int)glFormat, (int)glType, mipmap.Data);

                    }

                }

                textureProperties["maxMipLevel"] = mipmaps.Count - 1;

            }
            else if (texture is DataTexture2DArray)
            {

                state.TexImage3D((int)GLEnum.Texture2DArray, 0, glInternalFormat, (texture as DataTexture2DArray).Width, (texture as DataTexture2DArray).Height, (texture as DataTexture2DArray).Depth, 0, (int)glFormat, (int)glType, (texture as DataTexture2DArray).Data);
                textureProperties["maxMipLevel"] = 0;

            }
            else if (texture is DataTexture3D)
            {

                state.TexImage3D((int)GLEnum.Texture3D, 0, glInternalFormat, (texture as DataTexture3D).Width, (texture as DataTexture3D).Height, (texture as DataTexture3D).Depth, 0, (int)glFormat, (int)glType, (texture as DataTexture3D).Data);
                textureProperties["maxMipLevel"] = 0;

            }
            else
            {

                // regular Texture (image, video, canvas)

                // use manually created mipmaps if available
                // if there are no manual mipmaps
                // set 0 level mipmap and then use GL to generate other mipmap levels

                if (mipmaps.Count > 0 && supportsMips)
                {

                    for (var i = 0; i < mipmaps.Count; i++)
                    {

                        mipmap = mipmaps[i];
                        state.TexImage2D((int)GLEnum.Texture2D, i, glInternalFormat, mipmap.Width, mipmap.Height, 0, (int)glFormat, (int)glType, mipmap.Data);


                    }

                    texture.GenerateMipmaps = false;
                    textureProperties["maxMipLevel"] = mipmaps.Count - 1;

                }
                else
                {

                    //image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    //var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);//System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    //state.TexImage2D(TextureTarget2d.Texture2D, 0, (TextureComponentCount)glInternalFormat, image.Width, image.Height, 0, (OpenTK.Graphics.ES30.PixelFormat)glType, PixelType.UnsignedByte, data.Scan0);
                    //state.TexImage2D(TextureTarget2d.Texture2D, 0, TextureComponentCount.Rgba, image.Width, image.Height, 0, OpenTK.Graphics.ES30.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
                    //OpenTK.Graphics.Opengl.gl.TexImage2D(OpenTK.Graphics.Opengl.TextureTarget.Texture2D,
                    //    0,
                    //    OpenTK.Graphics.Opengl.PixelInternalFormat.Rgb,
                    //    data.Width,
                    //    data.Height,
                    //    0,
                    //    OpenTK.Graphics.Opengl.PixelFormat.Bgra,
                    //    OpenTK.Graphics.Opengl.PixelType.UnsignedByte,
                    //    data.Scan0);
                    byte[] bytes = image.Bytes;
                    fixed (byte* p = bytes)
                    {
                        gl.TexImage2D(GLEnum.Texture2D, 0, glInternalFormat, (uint)image.Width, (uint)image.Height, 0, (GLEnum)32993, glType, p);
                        textureProperties["maxMipLevel"] = 0;
                    }
                    //image.UnlockBits(data);

                    //byte[] pixels = image.GetTextureImage();
                    //state.TexImage2D(TextureTarget2d.Texture2D, 0, (TextureComponentCount)glInternalFormat, image.Width, image.Height, 0, (OpenTK.Graphics.ES30.PixelFormat)glType, PixelType.UnsignedByte, pixels);

                   


                }

            }

            if (TextureNeedsGenerateMipmaps(texture, supportsMips))
            {

                GenerateMipmap(textureType, texture, image.Width, image.Height);

            }

            textureProperties["version"] = texture.version;
            //if ( texture.onUpdate ) texture.onUpdate( texture );
        }

        public unsafe void SetupFrameBufferTexture(int framebuffer, GLRenderTarget renderTarget, GLEnum attachment, GLEnum textureTarget)
        {

            var glFormat = utils.Convert(renderTarget.Texture.Format);
            var glType = utils.Convert(renderTarget.Texture.Type);
            var glInternalFormat = GetInternalFormat(renderTarget.Texture.InternalFormat, (int)glFormat, (int)glType);

            int target = (int)textureTarget;
            byte[] emptyData = Array.Empty<byte>();
            fixed(byte *ptr = emptyData)
            {
                gl.TexImage2D((GLEnum)target, 0, glInternalFormat, (uint)renderTarget.Width, (uint)renderTarget.Height, 0, (PixelFormat)glFormat, (PixelType)glType, ptr);
            }
            //state.TexImage2D((int)(int)target, 0, glInternalFormat, renderTarget.Width, renderTarget.Height, 0, (int)glFormat, (int)glType, IntPtr.Zero);
            int texture = (int)(uint)properties.Get(renderTarget.Texture)["glTexture"];
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)framebuffer);
            gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, textureTarget, (uint)texture, 0);
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void SetupRenderBufferStorage(int renderbuffer, GLRenderTarget renderTarget, bool isMultisample)
        {
            gl.BindRenderbuffer(GLEnum.Renderbuffer, (uint)renderbuffer);

            if (renderTarget.depthBuffer && !renderTarget.stencilBuffer)
            {

                if (isMultisample)
                {

                    var samples = GetRenderTargetSamples(renderTarget);

                    gl.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, (uint)samples, GLEnum.DepthComponent16, (uint)renderTarget.Width, (uint)renderTarget.Height);

                }
                else
                {

                    gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent16, (uint)renderTarget.Width, (uint)renderTarget.Height);

                }

                gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, (uint)renderbuffer);

            }
            else if (renderTarget.depthBuffer && renderTarget.stencilBuffer)
            {

                if (isMultisample)
                {

                    var samples = GetRenderTargetSamples(renderTarget);


                    gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, (uint)samples, GLEnum.Depth24Stencil8, (uint)renderTarget.Width, (uint)renderTarget.Height);

                }
                else
                {
                    gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, (uint)renderTarget.Width, (uint)renderTarget.Height);

                }


                gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, (uint)renderbuffer);

            }
            else
            {

                var glFormat = utils.Convert(renderTarget.Texture.Format);
                var glType = utils.Convert(renderTarget.Texture.Type);
                var glInternalFormat = GetInternalFormat(renderTarget.Texture.InternalFormat, (int)glFormat, (int)glType);

                if (isMultisample)
                {

                    var samples = GetRenderTargetSamples(renderTarget);

                    gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer,(uint)samples, (GLEnum)glInternalFormat,(uint) renderTarget.Width, (uint)renderTarget.Height);

                }
                else
                {

                    gl.RenderbufferStorage(GLEnum.Renderbuffer, (GLEnum)glInternalFormat, (uint)renderTarget.Width, (uint)renderTarget.Height);

                }

            }

            gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void SetupDepthTexture(int framebuffer, GLRenderTarget renderTarget)
        {
            var isCube = (renderTarget != null && renderTarget is GLCubeRenderTarget);
            if (isCube) throw new Exception("Depth Texture with cube render targets is not supported");

            gl.BindFramebuffer(GLEnum.Framebuffer, (uint)framebuffer);

            if (!(renderTarget.depthTexture != null && renderTarget.depthTexture is DepthTexture))
            {

                throw new Exception("renderTarget.depthTexture must be an instance of THREE.DepthTexture");

            }

            // upload an empty depth texture with framebuffer size
            if (properties.Get(renderTarget.depthTexture)["glTexture"] != null ||
                    renderTarget.depthTexture.ImageSize.Width != renderTarget.Width ||
                    renderTarget.depthTexture.ImageSize.Height != renderTarget.Height)
            {

                renderTarget.depthTexture.ImageSize.Width = renderTarget.Width;
                renderTarget.depthTexture.ImageSize.Height = renderTarget.Height;
                renderTarget.depthTexture.NeedsUpdate = true;

            }

            SetTexture2D(renderTarget.depthTexture, 0);

            var glDepthTexture = (int)(uint)properties.Get(renderTarget.depthTexture)["glTexture"];

            if (renderTarget.depthTexture.Format == Constants.DepthFormat)
            {

                gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Texture2D, (uint)glDepthTexture, 0);

            }
            else if (renderTarget.depthTexture.Format == Constants.DepthStencilFormat)
            {

                gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Texture2D, (uint)glDepthTexture, 0);

            }
            else
            {

                throw new Exception("Unknown depthTexture format");

            }
        }

        public void SetupDepthRenderbuffer(GLRenderTarget renderTarget)
        {
            var renderTargetProperties = properties.Get(renderTarget);

            var isCube = renderTarget is GLCubeRenderTarget;

            if (renderTarget.depthTexture != null)
            {

                if (isCube) throw new Exception("target.depthTexture not supported in Cube render targets");

                SetupDepthTexture((int)(uint)renderTargetProperties["glFramebuffer"], renderTarget);

            }
            else
            {

                if (isCube)
                {

                    int[] depthbuffer = new int[6];
                    int[] framebuffer = (int[])renderTargetProperties["glFramebuffer"];
                    for (var i = 0; i < 6; i++)
                    {

                        gl.BindFramebuffer(GLEnum.Framebuffer, (uint)framebuffer[i]);
                        depthbuffer[i] =(int) gl.GenRenderbuffer();
                        SetupRenderBufferStorage(depthbuffer[i], renderTarget, false);

                    }
                    renderTargetProperties["glDepthbuffer"] = depthbuffer;

                }
                else
                {

                    gl.BindFramebuffer(GLEnum.Framebuffer, (uint)renderTargetProperties["glFramebuffer"]);
                    var buffer = gl.GenRenderbuffer();
                    renderTargetProperties["glDepthbuffer"] = buffer;
                    SetupRenderBufferStorage((int)(uint)renderTargetProperties["glDepthbuffer"], renderTarget, false);

                }

            }

            gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void SetupRenderTarget(GLRenderTarget renderTarget)
        {
            var renderTargetProperties = properties.Get(renderTarget);
            var textureProperties = properties.Get(renderTarget.Texture);

            renderTarget.Disposed += (s, e) =>
            {
                //if (!this.Context.IsDisposed)
                //{
                    DeallocateRenderTarget(renderTarget);
                //}

            };


            textureProperties["glTexture"] = gl.GenTexture();

            info.memory.Textures++;

            var isCube = renderTarget is GLCubeRenderTarget;
            var isMultisample = renderTarget is GLMultisampleRenderTarget;
            //var isMultiview =  renderTarget is GLMultiviewRenderTarget;
            var supportsMips = IsPowerOfTwo(renderTarget) || IsGL2;

            // Setup framebuffer

            if (isCube)
            {

                //renderTargetProperties.__webglFramebuffer = [];
                uint[] framebuffer = new uint[6];

                for (var i = 0; i < 6; i++)
                {

                    //renderTargetProperties.__webglFramebuffer[ i ] = _gl.createFramebuffer();
                    framebuffer[i] = gl.GenFramebuffer();

                }
                renderTargetProperties["glFramebuffer"] = framebuffer;

            }
            else
            {

                renderTargetProperties["glFramebuffer"] = gl.GenFramebuffer();

                if (isMultisample)
                {

                    if (IsGL2)
                    {

                        renderTargetProperties["glMultisampledFramebuffer"] = gl.GenFramebuffer();
                        renderTargetProperties["glColorRenderbuffer"] = gl.GenRenderbuffer();

                        gl.BindRenderbuffer(GLEnum.Renderbuffer, (uint)renderTargetProperties["glColorRenderbuffer"]);
                        var glFormat = utils.Convert(renderTarget.Texture.Format);
                        var glType = utils.Convert(renderTarget.Texture.Type);
                        var glInternalFormat = GetInternalFormat(renderTarget.Texture.InternalFormat, (int)glFormat, (int)glType);
                        var samples = GetRenderTargetSamples(renderTarget);
                        gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, (uint)samples, (GLEnum)glInternalFormat, (uint)renderTarget.Width,(uint)renderTarget.Height);

                        gl.BindFramebuffer(GLEnum.Framebuffer, (uint)renderTargetProperties["glMultisampledFramebuffer"]);
                        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Renderbuffer, (uint)renderTargetProperties["glColorRenderbuffer"]);
                        gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                        if (renderTarget.depthBuffer)
                        {

                            renderTargetProperties["glDepthRenderbuffer"] = gl.GenRenderbuffer();
                            SetupRenderBufferStorage((int)renderTargetProperties["glDepthRenderbuffer"], renderTarget, true);

                        }

                        gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


                    }
                    else
                    {

                        Trace.TraceWarning("THREE.GLRenderer: WebGLMultisampleRenderTarget can only be used with GL2.");

                    }

                }
            }

            // Setup color buffer

            if (isCube)
            {

                state.BindTexture((int)TextureTarget.TextureCubeMap, (int)(uint)textureProperties["glTexture"]);
                SetTextureParameters(TextureTarget.TextureCubeMap, renderTarget.Texture, supportsMips);

                for (var i = 0; i < 6; i++)
                {

                    SetupFrameBufferTexture(((int[])renderTargetProperties["glFramebuffer"])[i], renderTarget, GLEnum.ColorAttachment0, GLEnum.TextureCubeMapPositiveX + i);

                }

                if (TextureNeedsGenerateMipmaps(renderTarget.Texture, supportsMips))
                {

                    GenerateMipmap(TextureTarget.TextureCubeMap, renderTarget.Texture, renderTarget.Width, renderTarget.Height);

                }

                state.BindTexture((int)TextureTarget.TextureCubeMap, null);

            }
            else
            {//if{ ( ! isMultiview ) {

                state.BindTexture((int)TextureTarget.Texture2D, (int)(uint)textureProperties["glTexture"]);
                SetTextureParameters(TextureTarget.Texture2D, renderTarget.Texture, supportsMips);
                SetupFrameBufferTexture((int)(uint)renderTargetProperties["glFramebuffer"], renderTarget, GLEnum.ColorAttachment0, GLEnum.Texture2D);

                if (TextureNeedsGenerateMipmaps(renderTarget.Texture, supportsMips))
                {

                    GenerateMipmap(TextureTarget.Texture2D, renderTarget.Texture, renderTarget.Width, renderTarget.Height);

                }

                state.BindTexture((int)TextureTarget.Texture2D, null);

            }

            // Setup depth and stencil buffers

            if (renderTarget.depthBuffer)
            {

                SetupDepthRenderbuffer(renderTarget);

            }
        }
        private void OnRenderTargetDispose()
        {
            // Disposed+=(s,e) =>{} 
            //see SetupRenderTarget(renderTarget)

        }

        public void ResetTextureUnits()
        {
            this.textureUnits = 0;
        }

        public int AllocateTextureUnit()
        {
            var textureUnit = textureUnits;

            if (textureUnit >= maxTextures)
            {
                Trace.TraceWarning("THREE.GLTextures: Trying to use " + textureUnits + " texture units while this GPU supports only " + maxTextures);
            }

            textureUnits += 1;

            return textureUnit;
        }

        public void UpdateRenderTargetMipmap(GLRenderTarget renderTarget)
        {
            var texture = renderTarget.Texture;
            var supportsMips = IsPowerOfTwo(renderTarget.Image) || this.IsGL2;

            if (TextureNeedsGenerateMipmaps(texture, supportsMips))
            {
                var target = renderTarget is GLCubeRenderTarget ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D;
                int? glTexture = (int?)(properties.Get(texture)["glTexture"]);

                state.BindTexture((int)target, glTexture);

                GenerateMipmap(target, texture, renderTarget.Width, renderTarget.Height);

                state.BindTexture((int)target, null);

            }
        }

        public void UpdateMultisampleRenderTarget(GLRenderTarget renderTarget)
        {
            if (renderTarget is GLMultisampleRenderTarget)
            {
                if (IsGL2)
                {
                    var renderTargetProperties = properties.Get(renderTarget);

                    gl.BindFramebuffer(GLEnum.ReadFramebuffer, (uint)renderTargetProperties["glMultisampledFramebuffer"]);
                    gl.BindFramebuffer(GLEnum.DrawFramebuffer, (uint)renderTargetProperties["glFramebuffer"]);

                    var width = renderTarget.Width;
                    var height = renderTarget.Height;
                    var mask = ClearBufferMask.ColorBufferBit;

                    if (renderTarget.depthBuffer) mask |= ClearBufferMask.DepthBufferBit;
                    if (renderTarget.stencilBuffer) mask |= ClearBufferMask.StencilBufferBit;

                    gl.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, mask, BlitFramebufferFilter.Nearest);

                }
                else
                {
                    Trace.TraceWarning("THREE.Renderers.gl.GLTextures : GLMultisampleRenderTarget can only be used with GL2.");
                }
            }
        }
        private int GetRenderTargetSamples(GLRenderTarget renderTarget)
        {
            return IsGL2 && renderTarget is GLMultisampleRenderTarget ? System.Math.Min(maxSample, (renderTarget as GLMultisampleRenderTarget).Samples) : 0;
        }

        private void UpdateVideoTexture(VideoTexture texture)
        {
            var frame = info.render.Frame;

            if (videoTextures.Contains(texture) && (int)videoTextures[texture] != frame)
            {
                videoTextures.Add(texture, frame);
                (texture as VideoTexture).Update();
            }
        }

        public void SafeSetTexture2D(Texture texture, int slot)
        {
            SetTexture2D(texture, slot);
        }

        private bool warnedTexture2D = false;
        private bool warnedTextureCube = false;

        public void SafeSetTextureCube(Texture texture, int slot)
        {
            if (texture != null && texture is GLCubeRenderTarget)
            {

                if (warnedTextureCube == false)
                {

                    Trace.TraceWarning("THREE.WebGLTextures.safeSetTextureCube: don't use cube render targets as textures. Use their .texture property instead.");
                    warnedTextureCube = true;

                }

                texture = (texture as GLCubeRenderTarget).Texture;

            }

            // currently relying on the fact that WebGLRenderTargetCube.texture is a Texture and NOT a CubeTexture
            // TODO: unify these code paths
            if ((texture != null && texture is CubeTexture) ||
                (texture.Images != null && texture.Images.Length == 6))
            {

                // CompressedTexture can have Array in image :/

                // this function alone should take care of cube textures
                SetTextureCube(texture, slot);

            }
            else
            {

                // assumed: texture property of THREE.WebGLRenderTargetCube
                SetTextureCubeDynamic(texture, slot);

            }
        }
    }
}
