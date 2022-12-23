using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    public class MaskPass : Pass
    {
        public Scene scene;

        public Camera camera;

        public bool Inverse;

        public MaskPass(Scene scene,Camera camera) : base()
        {
            this.scene = scene;
            this.camera = camera;
            this.Clear = true;
            this.NeedsSwap = false;
            this.Inverse = false;
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer,float? deltaTime=null,bool? maskActive=null)
        {
            var state = renderer.state;

            // don't update color or depth
            state.buffers.color.SetMask(false);
            state.buffers.depth.SetMask(false);

            // lock buffers

            state.buffers.color.SetLocked(true);
            state.buffers.depth.SetLocked(true);

            // set up stencil

            int writeValue,clearValue;

            if (this.Inverse)
            {

                writeValue = 0;
                clearValue = 1;

            }
            else
            {

                writeValue = 1;
                clearValue = 0;

            }

            state.buffers.stencil.SetTest(true);
            state.buffers.stencil.SetOp(Constants.ReplaceStencilOp, Constants.ReplaceStencilOp, Constants.ReplaceStencilOp);
            unchecked
            {
                state.buffers.stencil.SetFunc(Constants.AlwaysStencilFunc, writeValue, (int)0xffffffff);
            }
            state.buffers.stencil.SetClear(clearValue);
            state.buffers.stencil.SetLocked(true);

            // draw into the stencil buffer

            renderer.SetRenderTarget(readBuffer);
            if (this.Clear) renderer.Clear();
            renderer.Render(this.scene, this.camera);

            renderer.SetRenderTarget(writeBuffer);
            if (this.Clear) renderer.Clear();
            renderer.Render(this.scene, this.camera);

            // unlock color and depth buffer for subsequent rendering

            state.buffers.color.SetLocked(false);
            state.buffers.depth.SetLocked(false);

            // only render where stencil is set to 1

            state.buffers.stencil.SetLocked(false);
            unchecked
            {
                state.buffers.stencil.SetFunc(Constants.EqualStencilFunc, 1, (int)0xffffffff); // draw if == 1
            }
            state.buffers.stencil.SetOp(Constants.KeepStencilOp, Constants.KeepStencilOp, Constants.KeepStencilOp);
            state.buffers.stencil.SetLocked(true);
        }

        public override void SetSize(float width, float height)
        {
           
        }
    }

    public class ClearMaskPass : Pass
    {
        public ClearMaskPass() : base()
        {
            this.NeedsSwap = false;
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            renderer.state.buffers.stencil.SetLocked(false);
            renderer.state.buffers.stencil.SetTest(false);
        }

        public override void SetSize(float width, float height)
        {
            
        }
    }
}
