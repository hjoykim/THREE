
namespace THREE
{
    public class FilmPass : Pass
    {
        public GLUniforms uniforms;

        public ShaderMaterial material;

        public FilmPass(float? noiseIntensity,float? scanlinesIntensity,float? scanlinesCount,bool? grayscale)
        {
            var shader = new FilmShader();

            this.uniforms = UniformsUtils.CloneUniforms(shader.Uniforms);

            material = new ShaderMaterial();
            material.Uniforms = uniforms;
            material.VertexShader = shader.VertexShader;
            material.FragmentShader = shader.FragmentShader;


            if (grayscale != null) (this.uniforms["grayscale"] as GLUniform)["value"]= grayscale.Value;
            if (noiseIntensity != null) (this.uniforms["nIntensity"] as GLUniform)["value"]=noiseIntensity.Value;
            if (scanlinesIntensity != null) (this.uniforms["sIntensity"] as GLUniform)["value"] = scanlinesIntensity.Value;
            if (scanlinesCount != null) (this.uniforms["sCount"] as GLUniform)["value"] = scanlinesCount.Value;

            this.fullScreenQuad = new Pass.FullScreenQuad(this.material);
        }

        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            (this.uniforms["tDiffuse"] as GLUniform)["value"]= readBuffer.Texture;
            float currentDeltaTime = (float)(this.uniforms["time"] as GLUniform)["value"] + deltaTime.Value;
            (this.uniforms["time"] as GLUniform)["value"] = currentDeltaTime;

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
