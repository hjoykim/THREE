using System.Collections;

namespace THREE
{
    public class BloomPass : Pass
    {
        private float strength;
        private int kernelSize;
        private float sigma;
        private int resolution;

        private GLRenderTarget renderTargetX;
        private GLRenderTarget renderTargetY;
        private ShaderMaterial materialCopy;
        private ShaderMaterial materialConvolution;
        private GLUniforms convolutionUniforms;
        private GLUniforms uniforms;
        private CopyShader copyShader;

        public static Vector2 BlurX = new Vector2(0.001953125f, 0.0f);
        public static Vector2 BlurY = new Vector2(0.0f, 0.001953125f);

        public BloomPass(float? strength=null,int? kernelSize=null,float? sigma=null,int? resolution=null) : base()
        {
            this.strength = strength != null ? strength.Value : 1.0f;
            this.kernelSize = kernelSize != null ? kernelSize.Value : 25;
            this.sigma = sigma != null ? sigma.Value : 4.0f;
            this.resolution = resolution != null ? resolution.Value : 256;

            Hashtable pars = new Hashtable();
            pars.Add("minFilter", Constants.LinearFilter);
            pars.Add("magFilter", Constants.LinearFilter);
            pars.Add("format", Constants.RGBAFormat);

            renderTargetX = new GLRenderTarget(this.resolution, this.resolution, pars);
            renderTargetX.Texture.Name = "BloomPass.x";

            renderTargetY = new GLRenderTarget(this.resolution, this.resolution, pars);
            renderTargetY.Texture.Name = "BloomPass.y";


            copyShader = new CopyShader();

            uniforms = UniformsUtils.CloneUniforms(copyShader.Uniforms);

            (uniforms["opacity"] as GLUniform)["value"] = this.strength;

            materialCopy = new ShaderMaterial { 
                Uniforms = uniforms,
                VertexShader = copyShader.VertexShader,
                FragmentShader = copyShader.FragmentShader,
                Blending = Constants.AdditiveBlending,
                Transparent = true            
            };

            ConvolutionShader convolutionShader = new ConvolutionShader();

            convolutionUniforms = UniformsUtils.CloneUniforms(convolutionShader.Uniforms);

            (convolutionUniforms["uImageIncrement"] as GLUniform)["value"] = BloomPass.BlurX;
            (convolutionUniforms["cKernel"] as GLUniform)["value"] = convolutionShader.BuildKernel(this.sigma);

            materialConvolution = new ShaderMaterial
            {
                Uniforms = convolutionUniforms,
                VertexShader = convolutionShader.VertexShader,
                FragmentShader = convolutionShader.FragmentShader
            };
            materialConvolution.Defines.Add("KERNEL_SIZE_FLOAT", this.kernelSize.ToString()+".0");
            materialConvolution.Defines.Add("KERNEL_SIZE_INT", this.kernelSize.ToString());

            this.NeedsSwap = false;

            this.fullScreenQuad = new Pass.FullScreenQuad();
        }
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            if (maskActive != null && maskActive.Value == true) renderer.state.buffers.stencil.SetTest(false);

            // Render quad with blured scene into texture (convolution pass 1)

            this.fullScreenQuad.material = this.materialConvolution;

            (this.convolutionUniforms["tDiffuse"] as GLUniform)["value"] = readBuffer.Texture;
            (this.convolutionUniforms["uImageIncrement"] as GLUniform)["value"] = BloomPass.BlurX;

            renderer.SetRenderTarget(this.renderTargetX);
            renderer.Clear();
            this.fullScreenQuad.Render(renderer);


            // Render quad with blured scene into texture (convolution pass 2)

            (this.convolutionUniforms["tDiffuse"] as GLUniform)["value"] = this.renderTargetX.Texture;
            (this.convolutionUniforms["uImageIncrement"] as GLUniform)["value"] = BloomPass.BlurY;

            renderer.SetRenderTarget(this.renderTargetY);
            renderer.Clear();
            this.fullScreenQuad.Render(renderer);

            // Render original scene with superimposed blur to texture

            this.fullScreenQuad.material = this.materialCopy;

            (this.uniforms["tDiffuse"] as GLUniform)["value"] = this.renderTargetY.Texture;

            if (maskActive!=null && maskActive.Value==true) renderer.state.buffers.stencil.SetTest(true);

            renderer.SetRenderTarget(readBuffer);
            if (this.Clear) renderer.Clear();
            this.fullScreenQuad.Render(renderer);

        }

        public override void SetSize(float width, float height)
        {
           
        }
    }
}
