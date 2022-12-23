using System.Collections;

namespace THREE
{
    public class BokehPass : Pass
    {
        public Scene scene;
        public Camera camera;

        private GLRenderTarget renderTargetDepth;
        private MeshDepthMaterial materialDepth;
        private ShaderMaterial materialBokeh;
        public GLUniforms uniforms;
		Color oldClearColor;
        public BokehPass(Scene scene,Camera camera,Hashtable parameter) : base()
        {
            this.scene = scene;
            this.camera = camera;

			var focus = parameter.ContainsKey("focus")   ? parameter["focus"] : 1.0f;
			var aspect = parameter.ContainsKey("aspect") ? parameter["aspect"]: camera.Aspect;
			var aperture = parameter.ContainsKey("aperture") ? parameter["aperture"]: 0.025f;
			var maxblur = parameter.ContainsKey("maxblur")? parameter["maxblur"]: 1.0f;

			// render targets

			var width = parameter.ContainsKey("width") ? (int)parameter["width"] : 1;
			var height = parameter.ContainsKey("height") ? (int)parameter["height"] : 1;

			this.renderTargetDepth = new GLRenderTarget(width, height,new Hashtable {
				{ "minFilter", Constants.NearestFilter },
				{ "magFilter", Constants.NearestFilter }
			} );

			this.renderTargetDepth.Texture.Name = "BokehPass.depth";

			// depth material

			this.materialDepth = new MeshDepthMaterial();
			this.materialDepth.DepthPacking = Constants.RGBADepthPacking;
			this.materialDepth.Blending = Constants.NoBlending;

			// bokeh material

			

			var bokehShader = new BokehShader();
			var bokehUniforms = UniformsUtils.CloneUniforms(bokehShader.Uniforms);

			(bokehUniforms["tDepth"] as GLUniform)["value"] = this.renderTargetDepth.Texture;

			(bokehUniforms["focus"] as GLUniform)["value"] = focus;
			(bokehUniforms["aspect"] as GLUniform)["value"] = aspect;
			(bokehUniforms["aperture"] as GLUniform)["value"] = aperture;
			(bokehUniforms["maxblur"] as GLUniform)["value"] = maxblur;
			(bokehUniforms["nearClip"] as GLUniform)["value"] = camera.Near;
			(bokehUniforms["farClip"] as GLUniform)["value"] = camera.Far;

			this.materialBokeh = new ShaderMaterial {
				Defines =bokehShader.Defines ,
				Uniforms = bokehUniforms,
			    VertexShader = bokehShader.VertexShader,
				FragmentShader = bokehShader.FragmentShader
			};

			this.uniforms = bokehUniforms;
			this.NeedsSwap = false;

			this.fullScreenQuad = new Pass.FullScreenQuad(this.materialBokeh);

			this.oldClearColor = new Color();

		}
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
			// Render depth into texture

			this.scene.OverrideMaterial = this.materialDepth;

			this.oldClearColor.Copy(renderer.GetClearColor());
			var oldClearAlpha = renderer.GetClearAlpha();
			var oldAutoClear = renderer.AutoClear;
			renderer.AutoClear = false;

			renderer.SetClearColor(Color.Hex(0xffffff));
			renderer.SetClearAlpha(1.0f);
			renderer.SetRenderTarget(this.renderTargetDepth);
			renderer.Clear();
			renderer.Render(this.scene, this.camera);

			// Render bokeh composite

			(this.uniforms["tColor"] as GLUniform)["value"] = readBuffer.Texture;
			(this.uniforms["nearClip"] as GLUniform)["value"] = this.camera.Near;
			(this.uniforms["farClip"] as GLUniform)["value"] = this.camera.Far;

			if (this.RenderToScreen)
			{

				renderer.SetRenderTarget(null);
				this.fullScreenQuad.Render(renderer);

			}
			else
			{

				renderer.SetRenderTarget(writeBuffer);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

			}

			this.scene.OverrideMaterial = null;
			renderer.SetClearColor(this.oldClearColor);
			renderer.SetClearAlpha(oldClearAlpha);
			renderer.AutoClear = oldAutoClear;
		}

        public override void SetSize(float width, float height)
        {
            
        }
    }
}
