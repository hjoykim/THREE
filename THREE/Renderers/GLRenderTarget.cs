using System;
using System.Collections;

namespace THREE
{
    public class GLRenderTarget : Texture,ICloneable
    {
        //protected static int RenderTargetIdCount;

        public bool IsGLMultiviewRenderTarget = false;

        public int NumViews = 0;

        public Texture Texture;

        //public int Id = RenderTargetIdCount++;

        //public Guid Uuid = Guid.NewGuid();

        public Hashtable Options;

        public int Width;

        public int Height;

        public Vector4 Scissor;

        public bool ScissorTest = false;

        public Vector4 Viewport;

        public bool depthBuffer;

        public bool stencilBuffer;

        public DepthTexture depthTexture;

        public GLRenderTarget(int width, int height, Hashtable options=null)
        {
            this.Width = width;

            this.Height = height;

            if (options != null)
            {
                this.Options = (Hashtable)options;
            }
            else
            {
                this.Options = new Hashtable();
            }

            Scissor = new Vector4(0, 0, width, height);
            ScissorTest = false;
            Viewport = new Vector4(0, 0, width, height);

            this.Texture = new Texture(null, null, (int?)Options["wrapS"], (int?)Options["wrapT"], (int?)Options["magFilter"], (int?)Options["minFilter"], (int?)Options["format"], (int?)Options["type"], (int?)Options["anisotropy"], (int?)Options["encoding"]);

            this.Texture.ImageSize.Width = width;
            this.Texture.ImageSize.Height = height;

            this.Texture.GenerateMipmaps = Options["generateMipmaps"] != null ? (bool)Options["generateMipmaps"] : false;
            this.Texture.MinFilter = Options["minFilter"] != null ? (int)Options["minFilter"] : Constants.LinearFilter;

            this.depthBuffer = Options["depthBuffer"] != null ? (bool)Options["depthBuffer"] : true;
            this.stencilBuffer = Options["stencilBuffer"] != null ? (bool)Options["stencilBuffer"] : true;
            this.depthTexture = Options["depthTexture"] != null ? (DepthTexture)Options["depthTexture"] : null;
        }

        protected GLRenderTarget(GLRenderTarget source)
        {
            this.Width = source.Width;
            this.Height = source.Height;

            Scissor = source.Scissor;
            ScissorTest = source.ScissorTest;
            this.Viewport = source.Viewport;


            this.Texture = (Texture)source.Texture.Clone();

            this.depthBuffer = source.depthBuffer;
            this.stencilBuffer = source.stencilBuffer;
            this.depthTexture = source.depthTexture;
        }

        public new object Clone() 
        {
            return new GLRenderTarget(this);
        }

        public void SetSize(int width, int height)
        {
            if (this.Width != width || this.Height != height)
            {
                this.Width = width;
                this.Height = height;

                this.Texture.ImageSize.Width = width;
                this.Texture.ImageSize.Height = height;

            }
        }
    }
}
