using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class UnrealBloomPass : Pass, IDisposable
	{
		public static Vector2 BlurDirectionX = new Vector2(1.0f, 0.0f);
		public static Vector2 BlurDirectionY = new Vector2(0.0f, 1.0f);

		public Vector2 Resolution;
		public float Strength;
		public float Radius;
		public float Threshold;
		public Color ClearColor;
		public List<GLRenderTarget> RenderTargetsHorizontal = new List<GLRenderTarget>();
		public List<GLRenderTarget> RenderTargetsVertical = new List<GLRenderTarget>();
		public int nMips;
		public GLRenderTarget renderTargetBright;
		GLUniforms highPassUniforms;
		ShaderMaterial materialHighPassFilter;
		List<ShaderMaterial> separableBlurMaterials = new List<ShaderMaterial>();
		ShaderMaterial compositeMaterial;
		List<Vector3> BloomTintColors;
		GLUniforms copyUniforms;
		ShaderMaterial materialCopy;
		Color oldClearColor;
		float oldClearAlpha;
		MeshBasicMaterial basic;

		public event EventHandler<EventArgs> Disposed;

		public UnrealBloomPass(Vector2 resolution=null, float? strength=null, float? radius=null, float? threshold=null) : base()
		{
			Strength = strength != null ? strength.Value : 1;
			Radius = radius != null ? radius.Value : 0;
			Threshold = threshold != null ? threshold.Value : 0;

			Resolution = resolution != null ? new Vector2(resolution.X, resolution.Y) : new Vector2(256, 256);
			this.ClearColor = new Color(0, 0, 0);

			// render targets
			Hashtable pars = new Hashtable { { "minFilter", Constants.LinearFilter }, { "magFilter", Constants.LinearFilter }, { "format", Constants.RGBAFormat } };

			this.nMips = 5;

			var resx = (int)System.Math.Round(this.Resolution.X / 2);
			var resy = (int)System.Math.Round(this.Resolution.Y / 2);

			this.renderTargetBright = new GLRenderTarget(resx, resy, pars);
			this.renderTargetBright.Texture.Name = "UnrealBloomPass.bright";
			this.renderTargetBright.Texture.GenerateMipmaps = false;

			for (var i = 0; i < this.nMips; i++) {

				var renderTargetHorizonal = new GLRenderTarget(resx, resy, pars);

				renderTargetHorizonal.Texture.Name = "UnrealBloomPass.h" + i;
				renderTargetHorizonal.Texture.GenerateMipmaps = false;

				RenderTargetsHorizontal.Add(renderTargetHorizonal);

				var renderTargetVertical = new GLRenderTarget(resx, resy, pars);

				renderTargetVertical.Texture.Name = "UnrealBloomPass.v" + i;
				renderTargetVertical.Texture.GenerateMipmaps = false;

				this.RenderTargetsVertical.Add(renderTargetVertical);

				resx = (int)System.Math.Round(resx / 2.0f);

				resy = (int)System.Math.Round(resy / 2.0f);

			}

			// luminosity high pass material



			var highPassShader = new LuminosityHighPassShader();
			this.highPassUniforms = UniformsUtils.CloneUniforms(highPassShader.Uniforms);

			(this.highPassUniforms["luminosityThreshold"] as GLUniform)["value"] = threshold;
			(this.highPassUniforms["smoothWidth"] as GLUniform)["value"] = 0.01f;

			this.materialHighPassFilter = new ShaderMaterial {
				Uniforms = this.highPassUniforms,
				VertexShader = highPassShader.VertexShader,
				FragmentShader = highPassShader.FragmentShader,

			};

			// Gaussian Blur Materials
			List<int> kernelSizeArray = new List<int> { 3, 5, 7, 9, 11 };
			resx = (int)System.Math.Round(this.Resolution.X / 2);
			resy = (int)System.Math.Round(this.Resolution.Y / 2);
			for (var i = 0; i < this.nMips; i++)
			{

				this.separableBlurMaterials.Add(GetSeperableBlurMaterial(kernelSizeArray[i]));

				(this.separableBlurMaterials[i].Uniforms["texSize"] as GLUniform)["value"] = new Vector2(resx, resy);

				resx = (int)System.Math.Round(resx / 2.0f);

				resy = (int)System.Math.Round(resy / 2.0f);

			}

			// Composite material
			this.compositeMaterial = this.GetCompositeMaterial(this.nMips);
			(this.compositeMaterial.Uniforms["blurTexture1"] as GLUniform)["value"] = this.RenderTargetsVertical[0].Texture;
			(this.compositeMaterial.Uniforms["blurTexture2"] as GLUniform)["value"] = this.RenderTargetsVertical[1].Texture;
			(this.compositeMaterial.Uniforms["blurTexture3"] as GLUniform)["value"] = this.RenderTargetsVertical[2].Texture;
			(this.compositeMaterial.Uniforms["blurTexture4"] as GLUniform)["value"] = this.RenderTargetsVertical[3].Texture;
			(this.compositeMaterial.Uniforms["blurTexture5"] as GLUniform)["value"] = this.RenderTargetsVertical[4].Texture;
			(this.compositeMaterial.Uniforms["bloomStrength"] as GLUniform)["value"] = strength;
			(this.compositeMaterial.Uniforms["bloomRadius"] as GLUniform)["value"] = 0.1f;
			this.compositeMaterial.NeedsUpdate = true;

			List<float> bloomFactors = new List<float> { 1.0f, 0.8f, 0.6f, 0.4f, 0.2f };
			(this.compositeMaterial.Uniforms["bloomFactors"] as GLUniform)["value"] = bloomFactors;
			this.BloomTintColors = new List<Vector3>{new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1),
									 new Vector3(1, 1, 1), new Vector3(1, 1, 1) };
			(this.compositeMaterial.Uniforms["bloomTintColors"] as GLUniform)["value"] = this.BloomTintColors;


			var copyShader = new CopyShader();

			this.copyUniforms = UniformsUtils.CloneUniforms(copyShader.Uniforms);
			(this.copyUniforms["opacity"] as GLUniform)["value"] = 1.0f;

			this.materialCopy = new ShaderMaterial {
				Uniforms = this.copyUniforms,
				VertexShader = copyShader.VertexShader,
				FragmentShader = copyShader.FragmentShader,
				Blending = Constants.AdditiveBlending,
				DepthTest = false,
				DepthWrite = false,
				Transparent = true
			};

			this.Enabled = true;
			this.NeedsSwap = false;

			this.oldClearColor = new Color();
			this.oldClearAlpha = 1;

			this.basic = new MeshBasicMaterial();

			this.fullScreenQuad = new FullScreenQuad(null);

		}

		private ShaderMaterial GetCompositeMaterial(int nMips)
		{
			return new ShaderMaterial
			{

				Defines = new Hashtable {
					{ "NUM_MIPS", nMips.ToString() }
				},

				Uniforms = new GLUniforms
				{
					{ "blurTexture1", new GLUniform { { "value", null } } },
					{ "blurTexture2", new GLUniform { { "value", null } } },
					{ "blurTexture3", new GLUniform { { "value", null } } },
					{ "blurTexture4", new GLUniform { { "value", null } } },
					{ "blurTexture5", new GLUniform { { "value", null } } },
					{ "dirtTexture", new GLUniform { { "value", null } } },
					{ "bloomStrength", new GLUniform { { "value", 1.0f } } },
					{ "bloomFactors", new GLUniform { { "value", null } } },
					{ "bloomTintColors", new GLUniform { { "value", null } } },
					{ "bloomRadius", new GLUniform { { "value", 0.0f } } }
				},


				VertexShader = @"
			varying vec2 vUv;
			void main()
			{
					vUv = uv;
					gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
				}
			",

				FragmentShader = @"
			varying vec2 vUv;
			uniform sampler2D blurTexture1;
			uniform sampler2D blurTexture2;
			uniform sampler2D blurTexture3;
			uniform sampler2D blurTexture4;
			uniform sampler2D blurTexture5;
			uniform sampler2D dirtTexture;
			uniform float bloomStrength;
			uniform float bloomRadius;
			uniform float bloomFactors[NUM_MIPS];
			uniform vec3 bloomTintColors[NUM_MIPS];			
			float lerpBloomFactor(const in float factor) { 
				float mirrorFactor = 1.2 - factor;
				return mix(factor, mirrorFactor, bloomRadius);
			}
			
			void main()
			{
					gl_FragColor = bloomStrength * (lerpBloomFactor(bloomFactors[0]) * vec4(bloomTintColors[0], 1.0) * texture2D(blurTexture1, vUv) + 
													 lerpBloomFactor(bloomFactors[1]) * vec4(bloomTintColors[1], 1.0) * texture2D(blurTexture2, vUv) + 
													 lerpBloomFactor(bloomFactors[2]) * vec4(bloomTintColors[2], 1.0) * texture2D(blurTexture3, vUv) + 
													 lerpBloomFactor(bloomFactors[3]) * vec4(bloomTintColors[3], 1.0) * texture2D(blurTexture4, vUv) + 
													 lerpBloomFactor(bloomFactors[4]) * vec4(bloomTintColors[4], 1.0) * texture2D(blurTexture5, vUv) );
				}"
			}; 
        }

        private ShaderMaterial GetSeperableBlurMaterial(int kernelRadius)
        {
			return new ShaderMaterial {

			Defines = new Hashtable {
				{ "KERNEL_RADIUS", kernelRadius.ToString() },
				{ "SIGMA" , kernelRadius.ToString() }
			},

			Uniforms = new GLUniforms
			{
				{ "colorTexture", new GLUniform { {"value", null } } },
				{ "texSize", new GLUniform{ { "value", new Vector2(0.5f, 0.5f) } } },
				{ "direction", new GLUniform{{"value", new Vector2(0.5f, 0.5f) } } }
			},

			VertexShader =@"
			
			varying vec2 vUv;
			void main()
			{
					vUv = uv;
					gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
				}
			",

			FragmentShader = @"
			#include <common>
			varying vec2 vUv;
			uniform sampler2D colorTexture;
			uniform vec2 texSize;
			uniform vec2 direction;
			
			float gaussianPdf(in float x, in float sigma)
			{
					return 0.39894 * exp(-0.5 * x * x / (sigma * sigma)) / sigma;
			}
			void main()
			{
				vec2 invSize = 1.0 / texSize;
				float fSigma = float(SIGMA);
				float weightSum = gaussianPdf(0.0, fSigma);
				vec3 diffuseSum = texture2D(colorTexture, vUv).rgb * weightSum;
				for (int i = 1; i < KERNEL_RADIUS; i++)
				{
					float x = float(i);
					float w = gaussianPdf(x, fSigma);
					vec2 uvOffset = direction * invSize * x;
					vec3 sample1 = texture2D(colorTexture, vUv + uvOffset).rgb;
					vec3 sample2 = texture2D(colorTexture, vUv - uvOffset).rgb;
					diffuseSum += (sample1 + sample2) * w;
					weightSum += 2.0 * w;
				}
					gl_FragColor = vec4(diffuseSum / weightSum, 1.0);
			}
			"
			} ;
        }

        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
			this.oldClearColor = renderer.GetClearColor();
			this.oldClearAlpha = renderer.GetClearAlpha();
			var oldAutoClear = renderer.AutoClear;
			renderer.AutoClear = false;

			renderer.SetClearColor(this.ClearColor, 0);

			if (maskActive!=null && maskActive.Value) renderer.state.buffers.stencil.SetTest(false);

			// Render input to screen

			if (this.RenderToScreen)
			{

				this.fullScreenQuad.material = this.basic;
				this.basic.Map = readBuffer.Texture;

				renderer.SetRenderTarget(null);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

			}

			// 1. Extract Bright Areas

			(this.highPassUniforms["tDiffuse"] as GLUniform)["value"] = readBuffer.Texture;
			(this.highPassUniforms["luminosityThreshold"] as GLUniform)["value"] = this.Threshold;
			this.fullScreenQuad.material = this.materialHighPassFilter;

			renderer.SetRenderTarget(this.renderTargetBright);
			renderer.Clear();
			this.fullScreenQuad.Render(renderer);

			// 2. Blur All the mips progressively

			var inputRenderTarget = this.renderTargetBright;

			for (var i = 0; i < this.nMips; i++)
			{

				this.fullScreenQuad.material = this.separableBlurMaterials[i];

				(this.separableBlurMaterials[i].Uniforms["colorTexture"] as GLUniform)["value"] = inputRenderTarget.Texture;
				(this.separableBlurMaterials[i].Uniforms["direction"] as GLUniform)["value"] = UnrealBloomPass.BlurDirectionX;
				renderer.SetRenderTarget(this.RenderTargetsHorizontal[i]);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				(this.separableBlurMaterials[i].Uniforms["colorTexture"] as GLUniform)["value"] = this.RenderTargetsHorizontal[i].Texture;
				(this.separableBlurMaterials[i].Uniforms["direction"] as GLUniform)["value"] = UnrealBloomPass.BlurDirectionY;
				renderer.SetRenderTarget(this.RenderTargetsVertical[i]);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				inputRenderTarget = this.RenderTargetsVertical[i];

			}

			// Composite All the mips

			this.fullScreenQuad.material = this.compositeMaterial;
			(this.compositeMaterial.Uniforms["bloomStrength"] as GLUniform)["value"] = this.Strength;
			(this.compositeMaterial.Uniforms["bloomRadius"] as GLUniform)["value"] = this.Radius;
			(this.compositeMaterial.Uniforms["bloomTintColors"] as GLUniform)["value"] = this.BloomTintColors;

			renderer.SetRenderTarget(this.RenderTargetsHorizontal[0]);
			renderer.Clear();
			this.fullScreenQuad.Render(renderer);

			// Blend it additively over the input texture

			this.fullScreenQuad.material = this.materialCopy;
			
			(this.copyUniforms["tDiffuse"] as GLUniform)["value"] = this.RenderTargetsHorizontal[0].Texture;
			if (maskActive!=null && maskActive.Value) renderer.state.buffers.stencil.SetTest(true);

			if (this.RenderToScreen)
			{

				renderer.SetRenderTarget(null);
				this.fullScreenQuad.Render(renderer);

			}
			else
			{

				renderer.SetRenderTarget(readBuffer);
				this.fullScreenQuad.Render(renderer);

			}

			// Restore renderer settings

			renderer.SetClearColor(this.oldClearColor, this.oldClearAlpha);
			renderer.AutoClear = oldAutoClear;
		}

        public override void SetSize(float width, float height)
        {
			var resx = (int)System.Math.Round(width / 2);
			var resy = (int)System.Math.Round(height / 2);

			this.renderTargetBright.SetSize(resx, resy);

			for (var i = 0; i < this.nMips; i++)
			{

				this.RenderTargetsHorizontal[i].SetSize(resx, resy);
				this.RenderTargetsVertical[i].SetSize(resx, resy);

				(this.separableBlurMaterials[i].Uniforms["texSize"] as GLUniform)["value"] = new Vector2(resx, resy);

				resx = (int)System.Math.Round(resx / 2.0f);
				resy = (int)System.Math.Round(resy / 2.0f);

			}
		}

		public virtual void Dispose()
		{
			Dispose(disposed);
		}
		protected virtual void RaiseDisposed()
		{
			var handler = this.Disposed;
			if (handler != null)
				handler(this, new EventArgs());
		}
		private bool disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed) return;
			try
			{
				for(int i = 0; i < RenderTargetsHorizontal.Count; i++)
                {
					RenderTargetsHorizontal[i].Dispose();
                }
				for(int i = 0; i < RenderTargetsVertical.Count; i++)
                {
					RenderTargetsVertical[i].Dispose();
                }

				this.renderTargetBright.Dispose();

				this.RaiseDisposed();
				this.disposed = true;
			}
			finally
			{

			}
			this.disposed = true;
		}
	}
}
