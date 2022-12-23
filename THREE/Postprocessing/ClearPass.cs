
namespace THREE
{
    public class ClearPass : Pass
    {
        Color? clearColor=null;
        float clearAlpha = 0;

        public ClearPass(Color? clearColor=null,float? clearAlpha=null)
        {
            this.NeedsSwap = false;

            this.clearColor = clearColor != null ? clearColor.Value : Color.Hex(0x000000);
            this.clearAlpha = clearAlpha != null ? clearAlpha.Value : 0.0f;
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            Color oldClearColor  = Color.Hex(0x000000);
            float oldClearAlpha = 0.0f;

            if (this.clearColor!=null)
            {

                oldClearColor = renderer.GetClearColor();
                oldClearAlpha = renderer.GetClearAlpha();

                renderer.SetClearColor(this.clearColor.Value, this.clearAlpha);

            }

            renderer.SetRenderTarget(this.RenderToScreen ? null : readBuffer);
            renderer.Clear();

            if (this.clearColor!=null)
            {

                renderer.SetClearColor(oldClearColor, oldClearAlpha);

            }
        }

        public override void SetSize(float width, float height)
        {
           
        }
    }
}
