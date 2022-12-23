namespace THREE
{
    public class RenderPass : Pass
    {
        public Scene scene;
        public Camera camera;

        public Material OverrideMaterial;

        public Color? ClearColor;

        public float ClearAlpha;

        public bool ClearDepth;

        public RenderPass(Scene scene,Camera camera,Material overrideMaterial=null,Color? clearColor=null,float? clearAlpha=null)
        {
            this.scene = scene;
            this.camera = camera;

            this.OverrideMaterial = overrideMaterial;

            this.ClearColor = clearColor;
            if (clearAlpha == null)
                this.ClearAlpha = 1.0f;
            else 
                this.ClearAlpha = clearAlpha.Value;

            this.Clear = true;
            this.ClearDepth = false;
            this.NeedsSwap = false;
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime=null, bool? maskActive=null)
        {
            var oldAutoClear = renderer.AutoClear;
            renderer.AutoClear = false;

            Color? oldClearColor = null; ;
            float oldClearAlpha=1;
            Material oldOverrideMaterial;

           

            oldOverrideMaterial = this.scene.OverrideMaterial;

            this.scene.OverrideMaterial = this.OverrideMaterial;

            

            if (this.ClearColor !=null)
            {

                oldClearColor = renderer.GetClearColor();
                oldClearAlpha = renderer.GetClearAlpha();

                renderer.SetClearColor(this.ClearColor.Value, this.ClearAlpha);

            }

            if (this.ClearDepth)
            {

                renderer.ClearDepth();

            }

            renderer.SetRenderTarget(this.RenderToScreen ? null : readBuffer);

            // TODO: Avoid using autoClear properties, see https://github.com/mrdoob/three.js/pull/15571#issuecomment-465669600
            if (this.Clear) renderer.Clear(renderer.AutoClearColor, renderer.AutoClearDepth, renderer.AutoClearStencil);
            renderer.Render(this.scene, this.camera);

            if (this.ClearColor!=null)
            {

                renderer.SetClearColor(oldClearColor.Value, oldClearAlpha);

            }

            if (this.OverrideMaterial != null)
            {

                this.scene.OverrideMaterial = oldOverrideMaterial;

            }

            renderer.AutoClear = oldAutoClear;
        }

        public override void SetSize(float width, float height)
        {
           
        }
    }
}
