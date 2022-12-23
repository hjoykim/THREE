using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace THREE
{
    public class EffectComposer
    {
        public GLRenderer Renderer;

        public GLRenderTarget RenderTarget1;

        public GLRenderTarget RenderTarget2;

        public GLRenderTarget WriteBuffer;

        public GLRenderTarget ReadBuffer;

        public List<Pass> Passes = new List<Pass>();

        public ShaderPass CopyPass;

        public bool RenderToScreen;

        private float pixelRatio;

        private float width;
        private float height;

        private Stopwatch stopwatch = new Stopwatch();

        private CopyShader copyShader = new CopyShader();
        public EffectComposer(GLRenderer renderer,GLRenderTarget renderTarget = null)
        {
            Renderer = renderer;

            Hashtable parameters = new Hashtable();

            if(renderTarget==null)
            {
                parameters.Add("minFilter", Constants.LinearFilter);
                parameters.Add("magFilter", Constants.LinearFilter);
                parameters.Add("format", Constants.RGBAFormat);

                var size = renderer.GetSize();

                pixelRatio = renderer.GetPixelRatio();
                width = size.X;
                height = size.Y;

                renderTarget = new GLRenderTarget((int)(width * pixelRatio), (int)(height * pixelRatio), parameters);
                renderTarget.Texture.Name = "EffectComposer.rt1";
            }
            else
            {
                this.pixelRatio = 1;
                this.width = renderTarget.Width;
                this.height = renderTarget.Height;
            }

            this.RenderTarget1 = renderTarget;
            this.RenderTarget2 = (GLRenderTarget)renderTarget.Clone();
            //this.RenderTarget2 = new GLRenderTarget((int)(width * pixelRatio), (int)(height * pixelRatio), parameters);
            this.RenderTarget2.Texture.Name = "EffectComposer.rt2";

            this.WriteBuffer = this.RenderTarget1;
            this.ReadBuffer = this.RenderTarget2;

            this.RenderToScreen = true;

            CopyPass = new ShaderPass(copyShader);

            stopwatch.Start();
        }

        public float GetDelta()
        {
            return stopwatch.ElapsedMilliseconds / 1000.0f;
        }
        public void SwapBuffers()
        {
            var tmp = this.ReadBuffer;
            this.ReadBuffer = this.WriteBuffer;
            this.WriteBuffer = tmp;
        }

        public void AddPass(Pass pass)
        {
            this.Passes.Add(pass);
            pass.SetSize(width * pixelRatio, height * pixelRatio);
        }

        public void InsertPass(Pass pass,int index)
        {
            //this.Passes.Splice(index, 0, pass);
            Passes.Insert(index, pass);
            pass.SetSize(this.width * this.pixelRatio, this.height * this.pixelRatio);
        }

        public bool IsLastEnabledPass(int passIndex)
        {
            for (var i = passIndex + 1; i < this.Passes.Count; i++)
            {

                if (this.Passes[i].Enabled)
                {

                    return false;

                }

            }

            return true;
        }

        public void Render(float? deltaTime=null)
        {
            if (deltaTime == null)
            {

                deltaTime = GetDelta();

            }

            var currentRenderTarget = this.Renderer.GetRenderTarget();

            var maskActive = false;

            Pass pass; 
            int il = this.Passes.Count;

            for (int i = 0; i < il; i++)
            {

                pass = this.Passes[i];

                if (pass.Enabled == false) continue;

                pass.RenderToScreen = (this.RenderToScreen && this.IsLastEnabledPass(i));
                pass.Render(this.Renderer, this.WriteBuffer, this.ReadBuffer, deltaTime, maskActive);

                if (pass.NeedsSwap)
                {

                    if (maskActive)
                    {

                        //var context = this.renderer.getContext();
                        var stencil = this.Renderer.state.buffers.stencil;

                        //context.stencilFunc( context.NOTEQUAL, 1, 0xffffffff );
                        unchecked
                        {
                            stencil.SetFunc(Constants.NotEqualStencilFunc, 1, (int)0xffffffff);

                            this.CopyPass.Render(this.Renderer, this.WriteBuffer, this.ReadBuffer, deltaTime);

                            //context.stencilFunc( context.EQUAL, 1, 0xffffffff );
                            stencil.SetFunc(Constants.EqualStencilFunc, 1,(int)0xffffffff);
                        }

                    }

                    this.SwapBuffers();

                }              

                if (pass is MaskPass ) 
                {

                    maskActive = true;

                } 
                else if (pass is ClearMaskPass ) 
                {
                    maskActive = false;

                }

            }


            this.Renderer.SetRenderTarget(currentRenderTarget );
        }

        public void Reset(GLRenderTarget renderTarget=null)
        {
            if (renderTarget == null)
            {

                var size = this.Renderer.GetSize(new Vector2());
                this.pixelRatio = this.Renderer.GetPixelRatio();
                this.width = size.X;
                this.height = size.Y;

                renderTarget = (GLRenderTarget)this.RenderTarget1.Clone();
                renderTarget.SetSize((int)(this.width * this.pixelRatio), (int)(this.height * this.pixelRatio));

            }

            this.RenderTarget1.Dispose();
            this.RenderTarget2.Dispose();
            this.RenderTarget1 = renderTarget;
            this.RenderTarget2 = (GLRenderTarget)renderTarget.Clone();

            this.WriteBuffer = this.RenderTarget1;
            this.ReadBuffer = this.RenderTarget2;
        }

        public void SetSize(float width,float height)
        {
            this.width = width;
            this.height = height;

            var effectiveWidth = this.width * this.pixelRatio;
            var effectiveHeight = this.height * this.pixelRatio;

            this.RenderTarget1.SetSize((int)effectiveWidth, (int)effectiveHeight);
            this.RenderTarget2.SetSize((int)effectiveWidth, (int)effectiveHeight);

            for (var i = 0; i < this.Passes.Count; i++)
            {

                this.Passes[i].SetSize(effectiveWidth, effectiveHeight);

            }
        }

        public void SetPixelRatio(float pixelRatio)
        {
            this.pixelRatio = pixelRatio;

            this.SetSize(this.width, this.height);
        }
    }
}
