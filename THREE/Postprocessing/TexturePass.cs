using System;
using THREE.Renderers.Shaders;

namespace THREE
{
    [Serializable]
    public class TexturePass : Pass
    {
        private Texture map;
        private float opacity;
        private Uniforms uniforms;
        private ShaderMaterial material;

        public TexturePass(Texture map, float? opacity = null) : base()
        {
            var shader = new CopyShader();

            this.map = map;
            this.opacity = opacity != null ? opacity.Value : 1.0f;
            this.uniforms = UniformsUtils.CloneUniforms(shader.Uniforms);

            this.material = new ShaderMaterial
            {
                Uniforms = this.uniforms,
                VertexShader = shader.VertexShader,
                FragmentShader = shader.FragmentShader,
                DepthTest = false,
                DepthWrite = false
            };

            this.NeedsSwap = false;

            this.fullScreenQuad = new FullScreenQuad();
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            var oldAutoClear = renderer.AutoClear;
            renderer.AutoClear = false;

            this.fullScreenQuad.material = this.material;

            (this.uniforms["opacity"] as Uniform)["value"] = this.opacity;
            (this.uniforms["tDiffuse"] as Uniform)["value"] = this.map;
            this.material.Transparent = (this.opacity < 1.0);

            renderer.SetRenderTarget(this.RenderToScreen ? null : readBuffer);
            if (this.Clear) renderer.Clear();
            this.fullScreenQuad.Render(renderer);

            renderer.AutoClear = oldAutoClear;
        }

        public override void SetSize(float width, float height)
        {

        }
    }
}
