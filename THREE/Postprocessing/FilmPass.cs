
using System;
using THREE.Renderers.Shaders;

namespace THREE
{
    [Serializable]
    public class FilmPass : Pass
    {
        public Uniforms uniforms;

        public ShaderMaterial material;

        public FilmPass(float? noiseIntensity, float? scanlinesIntensity, float? scanlinesCount, bool? grayscale)
        {
            var shader = new FilmShader();

            this.uniforms = UniformsUtils.CloneUniforms(shader.Uniforms);

            material = new ShaderMaterial();
            material.Uniforms = uniforms;
            material.VertexShader = shader.VertexShader;
            material.FragmentShader = shader.FragmentShader;


            if (grayscale != null) (this.uniforms["grayscale"] as Uniform)["value"] = grayscale.Value;
            if (noiseIntensity != null) (this.uniforms["nIntensity"] as Uniform)["value"] = noiseIntensity.Value;
            if (scanlinesIntensity != null) (this.uniforms["sIntensity"] as Uniform)["value"] = scanlinesIntensity.Value;
            if (scanlinesCount != null) (this.uniforms["sCount"] as Uniform)["value"] = scanlinesCount.Value;

            this.fullScreenQuad = new Pass.FullScreenQuad(this.material);
        }

        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            (this.uniforms["tDiffuse"] as Uniform)["value"] = readBuffer.Texture;
            float currentDeltaTime = (float)(this.uniforms["time"] as Uniform)["value"] + deltaTime.Value;
            (this.uniforms["time"] as Uniform)["value"] = currentDeltaTime;

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

        }
    }
}
