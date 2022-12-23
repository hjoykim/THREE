using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Color = THREE.Color;

namespace THREE
{
	

	public class SSAOPass : Pass, IDisposable
	{
        Scene scene;
        Camera camera;
		int width;
		int height;
		float kernelRadius;
		int kernelSize;
		List<Vector3> kernel;
		DataTexture noiseTexture;
		int output;
		float minDistance;
		float maxDistance;
		GLRenderTarget beautyRenderTarget;
		GLRenderTarget normalRenderTarget;
		GLRenderTarget ssaoRenderTarget;
		GLRenderTarget blurRenderTarget;
		ShaderMaterial ssaoMaterial;
		MeshNormalMaterial normalMaterial;
		ShaderMaterial blurMaterial;
		ShaderMaterial depthRenderMaterial;
		ShaderMaterial copyMaterial;
		Color originalClearColor;

		public enum OUTPUT
        {
			Default=0,
			SSAO=1,
			Blur=2,
			Beauty=3,
			Depth=4,
			Normal=5
        };

		public SSAOPass(Scene scene, Camera camera, int? width=null, int? height=null) : base()
		{
			this.width = (width != null) ? width.Value : 512;
			this.height = (height != null) ? height.Value : 512;

			this.Clear = true;

			this.camera = camera;
			this.scene = scene;

			this.kernelRadius = 8;
			this.kernelSize = 32;
			this.kernel = new List<Vector3>();
			this.noiseTexture = null;
			this.output = 0;

			this.minDistance = 0.005f;
			this.maxDistance = 0.1f;

			GenerateSampleKernel();
			GenerateRandomKernelRotations();

			// beauty render target with depth buffer

			var depthTexture = new DepthTexture(0, 0, 0);
			depthTexture.Type = Constants.UnsignedShortType;
			depthTexture.MinFilter = Constants.NearestFilter;
			depthTexture.MaxFilter = Constants.NearestFilter;

			this.beautyRenderTarget = new GLRenderTarget(this.width, this.height, new Hashtable(){
				{ "minFilter", Constants.LinearFilter },
				{ "magFilter", Constants.LinearFilter },
				{ "format", Constants.RGBAFormat },
				{ "depthTexture", depthTexture },
				{ "depthBuffer", true }
			});

			// normal render target

			this.normalRenderTarget = new GLRenderTarget(this.width, this.height, new Hashtable(){
				{ "minFilter",  Constants.NearestFilter },
				{ "magFilter",  Constants.NearestFilter },
				{ "format",  Constants.RGBAFormat}
			} );

			// ssao render target

			this.ssaoRenderTarget = new GLRenderTarget(this.width, this.height, new Hashtable(){
				{ "minFilter",  Constants.LinearFilter},
				{ "magFilter",  Constants.LinearFilter},
				{ "format",  Constants.RGBAFormat}
			} );

			this.blurRenderTarget = (GLRenderTarget)this.ssaoRenderTarget.Clone();

			SSAOShader ssaoShader = new SSAOShader();
			this.ssaoMaterial = new ShaderMaterial( new Hashtable(){
					{ "defines", ssaoShader.Defines },
					{ "uniforms", UniformsUtils.CloneUniforms(ssaoShader.Uniforms ) },
					{ "vertexShader", ssaoShader.VertexShader },
					{ "fragmentShader", ssaoShader.FragmentShader },
					{ "blending", Constants.NoBlending }
				} );

			(this.ssaoMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.beautyRenderTarget.Texture;
			(this.ssaoMaterial.Uniforms["tNormal"] as GLUniform)["value"] = this.normalRenderTarget.Texture;
			(this.ssaoMaterial.Uniforms["tDepth"] as GLUniform)["value"] = this.beautyRenderTarget.depthTexture;
			(this.ssaoMaterial.Uniforms["tNoise"] as GLUniform)["value"] = this.noiseTexture;
			(this.ssaoMaterial.Uniforms["kernel"] as GLUniform)["value"] = this.kernel;
			(this.ssaoMaterial.Uniforms["cameraNear"] as GLUniform)["value"] = this.camera.Near;
			(this.ssaoMaterial.Uniforms["cameraFar"] as GLUniform)["value"] = this.camera.Far;
			((this.ssaoMaterial.Uniforms["resolution"] as GLUniform)["value"] as Vector2).Set(this.width, this.height);
			((this.ssaoMaterial.Uniforms["cameraProjectionMatrix"] as GLUniform)["value"] as Matrix4).Copy(this.camera.ProjectionMatrix);
			((this.ssaoMaterial.Uniforms["cameraInverseProjectionMatrix"] as GLUniform)["value"] as Matrix4).GetInverse(this.camera.ProjectionMatrix);

			// normal material

			this.normalMaterial = new MeshNormalMaterial();
			this.normalMaterial.Blending = Constants.NoBlending;

			// blur material
			SSAOBlurShader ssaoBlurShader = new SSAOBlurShader();
			this.blurMaterial = new ShaderMaterial( new Hashtable(){
				{ "defines", ssaoBlurShader.Defines },
				{ "uniforms", UniformsUtils.CloneUniforms(ssaoBlurShader.Uniforms ) },
				{ "vertexShader", ssaoBlurShader.VertexShader },
				{ "fragmentShader", ssaoBlurShader.FragmentShader }
				} );

			(this.blurMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.ssaoRenderTarget.Texture;
			((this.blurMaterial.Uniforms["resolution"] as GLUniform)["value"] as Vector2).Set(this.width, this.height);

			// material for rendering the depth
			SSAODepthShader ssaoDepthShader = new SSAODepthShader();
			this.depthRenderMaterial = new ShaderMaterial( new Hashtable(){
					{ "defines", ssaoDepthShader.Defines },
					{ "uniforms", UniformsUtils.CloneUniforms(ssaoDepthShader.Uniforms ) },
					{ "vertexShader", ssaoDepthShader.VertexShader },
					{ "fragmentShader", ssaoDepthShader.FragmentShader },
					{ "blending", Constants.NoBlending }
				} );
			(this.depthRenderMaterial.Uniforms["tDepth"]as GLUniform)["value"] = this.beautyRenderTarget.depthTexture;
			(this.depthRenderMaterial.Uniforms["cameraNear"]as GLUniform)["value"] = this.camera.Near;
			(this.depthRenderMaterial.Uniforms["cameraFar"]as GLUniform)["value"] = this.camera.Far;

			// material for rendering the content of a render target
			CopyShader copyShader = new CopyShader();
			this.copyMaterial = new ShaderMaterial( new Hashtable(){
				{ "uniforms", UniformsUtils.CloneUniforms(copyShader.Uniforms ) },
				{ "vertexShader", copyShader.VertexShader },
				{ "fragmentShader", copyShader.FragmentShader },
				{ "transparent", true },
				{ "depthTest", false },
				{ "depthWrite", false },
				{ "blendSrc", Constants.DstColorFactor },
				{ "blendDst", Constants.ZeroFactor },
				{ "blendEquation", Constants.AddEquation },
				{ "blendSrcAlpha", Constants.DstAlphaFactor },
				{ "blendDstAlpha", Constants.ZeroFactor },
				{ "blendEquationAlpha", Constants.AddEquation }
			} );

			this.fullScreenQuad = new FullScreenQuad();

			this.originalClearColor = new Color();
		}

		
        private void GenerateRandomKernelRotations()
        {
			int width = 4;
			int height = 4;
			

			var simplex = new SimplexNoise();

			var size = width * height;
			float[] data = new float[size * 4];

			for (var i = 0; i < size; i++)
			{

				var stride = i * 4;

				float x = (float)(MathUtils.random.NextDouble()* 2) - 1;
				float y = (float)(MathUtils.random.NextDouble() * 2) - 1;
				var z = 0;

				float noise = simplex.Noise3d(x, y, z);

				data[stride] = noise;
				data[stride + 1] = noise;
				data[stride + 2] = noise;
				data[stride + 3] = 1;

			}
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
			IntPtr iptr = bitmapData.Scan0;

			Marshal.Copy(iptr, data, 0, data.Length);

			bitmap.UnlockBits(bitmapData);

			this.noiseTexture = new DataTexture(bitmap, width, height, Constants.RGBAFormat, Constants.FloatType);
			this.noiseTexture.WrapS = Constants.RepeatWrapping;
			this.noiseTexture.WrapT = Constants.RepeatWrapping;
		}

        private void GenerateSampleKernel()
        {

			for (var i = 0; i < kernelSize; i++)
			{

				var sample = new Vector3();
				sample.X = (float)(MathUtils.random.NextDouble() * 2) - 1;
				sample.Y = (float)(MathUtils.random.NextDouble() * 2) - 1;
				sample.Z = (float)(MathUtils.random.NextDouble());

				sample.Normalize();

				float scale = i / kernelSize;
				scale = MathUtils.Lerp(0.1f, 1, scale * scale);
				sample.MultiplyScalar(scale);

				kernel.Add(sample);

			}
		}
				
        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
			// render beauty and depth

			renderer.SetRenderTarget(this.beautyRenderTarget);
			renderer.Clear();
			renderer.Render(this.scene, this.camera);

			// render normals

			this.RenderOverride(renderer, this.normalMaterial, this.normalRenderTarget, Color.Hex(0x7777ff), 1.0f);

			// render SSAO

			(this.ssaoMaterial.Uniforms["kernelRadius"] as GLUniform)["value"] = this.kernelRadius;
			(this.ssaoMaterial.Uniforms["minDistance"] as GLUniform)["value"] = this.minDistance;
			(this.ssaoMaterial.Uniforms["maxDistance"] as GLUniform)["value"] = this.maxDistance;
			this.RenderPass(renderer, this.ssaoMaterial, this.ssaoRenderTarget);

			// render blur

			this.RenderPass(renderer, this.blurMaterial, this.blurRenderTarget);

			// output result to screen

			switch (this.output)
			{

				case (int)OUTPUT.SSAO:

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.ssaoRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.NoBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				case (int)OUTPUT.Blur:

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.blurRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.NoBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				case (int)OUTPUT.Beauty:

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.beautyRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.NoBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				case (int)OUTPUT.Depth:

					this.RenderPass(renderer, this.depthRenderMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				case (int)OUTPUT.Normal:

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.normalRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.NoBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				case (int)OUTPUT.Default:

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.beautyRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.NoBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					(this.copyMaterial.Uniforms["tDiffuse"] as GLUniform)["value"] = this.blurRenderTarget.Texture;
					this.copyMaterial.Blending = Constants.CustomBlending;
					this.RenderPass(renderer, this.copyMaterial, this.RenderToScreen ? null : writeBuffer);

					break;

				

			}
		}

        public override void SetSize(float width, float height)
        {
			this.width = (int)width;
			this.height = (int)height;

			this.beautyRenderTarget.SetSize((int)width, (int)height);
			this.ssaoRenderTarget.SetSize((int)width, (int)height);
			this.normalRenderTarget.SetSize((int)width, (int)height);
			this.blurRenderTarget.SetSize((int)width, (int)height);

			((this.ssaoMaterial.Uniforms["resolution"] as GLUniform)["value"] as Vector2).Set(width, height);
			((this.ssaoMaterial.Uniforms["cameraProjectionMatrix"] as GLUniform)["value"] as Matrix4).Copy(this.camera.ProjectionMatrix);
			((this.ssaoMaterial.Uniforms["cameraInverseProjectionMatrix"] as GLUniform)["value"] as Matrix4).GetInverse(this.camera.ProjectionMatrix);

			((this.blurMaterial.Uniforms["resolution"] as GLUniform)["value"] as Vector2).Set(width, height);

		}

		private void RenderPass(GLRenderer renderer,Material passMaterial,GLRenderTarget renderTarget,Color? clearColor=null,float? clearAlpha=0.0f)
        {
			// save original state
			this.originalClearColor.Copy(renderer.GetClearColor());
			var originalClearAlpha = renderer.GetClearAlpha();
			var originalAutoClear = renderer.AutoClear;

			renderer.SetRenderTarget(renderTarget);

			// setup pass state
			renderer.AutoClear = false;
			if ((clearColor != null) && (clearColor != null))
			{

				renderer.SetClearColor(clearColor.Value);
				renderer.SetClearAlpha(clearAlpha.Value);
				renderer.Clear();

			}

			this.fullScreenQuad.material = passMaterial;
			this.fullScreenQuad.Render(renderer);

			// restore original state
			renderer.AutoClear = originalAutoClear;
			renderer.SetClearColor(this.originalClearColor);
			renderer.SetClearAlpha(originalClearAlpha);
		}

		private void RenderOverride(GLRenderer renderer,Material overrideMaterial,GLRenderTarget renderTarget,Color? clearColor=null, float? clearAlpha=0.0f)
        {
			this.originalClearColor.Copy(renderer.GetClearColor());
			var originalClearAlpha = renderer.GetClearAlpha();
			var originalAutoClear = renderer.AutoClear;

			if (clearAlpha == null) clearAlpha = 0.0f;

			renderer.SetRenderTarget(renderTarget);
			renderer.AutoClear = false;

			if ((clearColor != null) && (clearAlpha != null))
			{

				renderer.SetClearColor(clearColor.Value);
				renderer.SetClearAlpha(clearAlpha.Value);
				renderer.Clear();

			}

			this.scene.OverrideMaterial = overrideMaterial;
			renderer.Render(this.scene, this.camera);
			this.scene.OverrideMaterial = null;

			// restore original state

			renderer.AutoClear = originalAutoClear;
			renderer.SetClearColor(this.originalClearColor);
			renderer.SetClearAlpha(originalClearAlpha);
		}
		public event EventHandler<EventArgs> Disposed;
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
				// dispose render targets

				this.beautyRenderTarget.Dispose();
                this.normalRenderTarget.Dispose();

				this.ssaoRenderTarget.Dispose();
				this.blurRenderTarget.Dispose();

				// dispose materials

				this.normalMaterial.Dispose();
				this.blurMaterial.Dispose();
				this.copyMaterial.Dispose();
				this.depthRenderMaterial.Dispose();

				// dipsose full screen quad

				this.fullScreenQuad.Dispose();

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
