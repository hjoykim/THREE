using System;
using System.Collections;
using THREE.Renderers.Shaders;

namespace THREE
{
    [Serializable]
    public class HalftonePass : Pass
    {
        public Uniforms uniforms;
        public ShaderMaterial material;
        public HalftonePass(float? width = null, float? height = null, Hashtable parameter = null) : base()
        {
            var halftoneShader = new HalftoneShader();
            uniforms = UniformsUtils.CloneUniforms(halftoneShader.Uniforms);
            this.material = new ShaderMaterial
            {
                Uniforms = this.uniforms,
                FragmentShader = halftoneShader.FragmentShader,
                VertexShader = halftoneShader.VertexShader
            };

            // set params

            (this.uniforms["width"] as Uniform)["value"] = width;
            (this.uniforms["height"] as Uniform)["value"] = height;

            if (parameter != null)
            {


                foreach (DictionaryEntry key in parameter)
                {

                    if (key.Value != null && this.uniforms.ContainsKey((string)key.Key))
                    {
                        (this.uniforms[(string)key.Key] as Uniform)["value"] = key.Value;
                    }
                }
            }
            this.fullScreenQuad = new Pass.FullScreenQuad(this.material);
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            (this.material.Uniforms["tDiffuse"] as Uniform)["value"] = readBuffer.Texture;

            if (this.RenderToScreen)
            {

                renderer.SetRenderTarget(null);
                this.fullScreenQuad.Render(renderer);

            }
            else
            {

                renderer.SetRenderTarget(writeBuffer);
                if (this.Clear) renderer.Clear();
                this.fullScreenQuad.Render(renderer);

            }
        }

        public override void SetSize(float width, float height)
        {
            (this.uniforms["width"] as Uniform)["value"] = width;
            (this.uniforms["height"] as Uniform)["value"] = height;
        }
    }
}
