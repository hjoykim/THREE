using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class OutlinePass : Pass,IDisposable
	{
		Scene renderScene;
		Camera renderCamera;
		List<Object3D> selectedObjects = new List<Object3D>();
		public Color visibleEdgeColor;
		public Color hiddenEdgeColor;
		public float edgeGlow;
		public bool usePatternTexture;
		public float edgeThickness;
		public float edgeStrength;
		float downSampleRatio;
		public float pulsePeriod;
		Vector2 resolution;
		Texture patternTexture;
		MeshBasicMaterial maskBufferMaterial;
		GLRenderTarget renderTargetMaskBuffer;
		MeshDepthMaterial depthMaterial;
		ShaderMaterial prepareMaskMaterial;
		GLRenderTarget renderTargetDepthBuffer;
		GLRenderTarget renderTargetMaskDownSampleBuffer;
		GLRenderTarget renderTargetBlurBuffer1;
		GLRenderTarget renderTargetBlurBuffer2;
		ShaderMaterial edgeDetectionMaterial;
		GLRenderTarget renderTargetEdgeBuffer1;
		GLRenderTarget renderTargetEdgeBuffer2;
		ShaderMaterial separableBlurMaterial1;
		ShaderMaterial separableBlurMaterial2;
		ShaderMaterial overlayMaterial;

		GLUniforms copyUniforms;
		ShaderMaterial materialCopy;
		Color oldClearColor;
		float oldClearAlpha;
		Color tempPulseColor1;
		Color tempPulseColor2;
		Matrix4 textureMatrix;

		public static Vector2 BlurDirectionX = new Vector2(1.0f, 0.0f);
		public static Vector2 BlurDirectionY = new Vector2(0.0f, 1.0f);
		public event EventHandler<EventArgs> Disposed;

		public OutlinePass(Vector2 resolution, Scene scene, Camera camera, List<Object3D> selectedObjects = null) : base()
		{
			this.renderScene = scene;
			this.renderCamera = camera;
			if (selectedObjects != null) this.selectedObjects = selectedObjects;
			this.visibleEdgeColor = new Color(1, 1, 1);
			this.hiddenEdgeColor = new Color(0.1f, 0.04f, 0.02f);
			this.edgeGlow = 0.0f;
			this.usePatternTexture = false;
			this.edgeThickness = 1.0f;
			this.edgeStrength = 3.0f;
			this.downSampleRatio = 2.0f;
			this.pulsePeriod = 0.0f;


			this.resolution = (resolution != null) ? new Vector2(resolution.X, resolution.Y) : new Vector2(256, 256);

			var pars = new Hashtable { { "minFilter", Constants.LinearFilter }, { "magFilter", Constants.LinearFilter },{"format",Constants.RGBAFormat } };

			var resx = System.Math.Round(this.resolution.X / this.downSampleRatio);
			var resy = System.Math.Round(this.resolution.Y / this.downSampleRatio);

			this.maskBufferMaterial = new MeshBasicMaterial { Color = new Color(0xffffff) };
			this.maskBufferMaterial.Side = Constants.DoubleSide;
			this.renderTargetMaskBuffer = new GLRenderTarget((int)this.resolution.X, (int)this.resolution.Y, pars );
			this.renderTargetMaskBuffer.Texture.Name = "OutlinePass.mask";
			this.renderTargetMaskBuffer.Texture.GenerateMipmaps = false;

			this.depthMaterial = new MeshDepthMaterial();
			this.depthMaterial.Side = Constants.DoubleSide;
			this.depthMaterial.DepthPacking = Constants.RGBADepthPacking;
			this.depthMaterial.Blending = Constants.NoBlending;

			this.prepareMaskMaterial = this.GetPrepareMaskMaterial();
			this.prepareMaskMaterial.Side = Constants.DoubleSide;
			this.prepareMaskMaterial.FragmentShader = ReplaceDepthToViewZ(this.prepareMaskMaterial.FragmentShader, this.renderCamera );

			this.renderTargetDepthBuffer = new GLRenderTarget((int)this.resolution.X, (int)this.resolution.Y, pars );
			this.renderTargetDepthBuffer.Texture.Name = "OutlinePass.depth";
			this.renderTargetDepthBuffer.Texture.GenerateMipmaps = false;

			this.renderTargetMaskDownSampleBuffer = new GLRenderTarget((int)resx, (int)resy, pars );
			this.renderTargetMaskDownSampleBuffer.Texture.Name = "OutlinePass.depthDownSample";
			this.renderTargetMaskDownSampleBuffer.Texture.GenerateMipmaps = false;

			this.renderTargetBlurBuffer1 = new GLRenderTarget((int)resx, (int)resy, pars );
			this.renderTargetBlurBuffer1.Texture.Name = "OutlinePass.blur1";
			this.renderTargetBlurBuffer1.Texture.GenerateMipmaps = false;
			this.renderTargetBlurBuffer2 = new GLRenderTarget((int)System.Math.Round(resx / 2 ), (int)System.Math.Round(resy / 2 ), pars );
			this.renderTargetBlurBuffer2.Texture.Name = "OutlinePass.blur2";
			this.renderTargetBlurBuffer2.Texture.GenerateMipmaps = false;

			this.edgeDetectionMaterial = this.GetEdgeDetectionMaterial();
			this.renderTargetEdgeBuffer1 = new GLRenderTarget((int)resx, (int)resy, pars );
			this.renderTargetEdgeBuffer1.Texture.Name = "OutlinePass.edge1";
			this.renderTargetEdgeBuffer1.Texture.GenerateMipmaps = false;
			this.renderTargetEdgeBuffer2 = new GLRenderTarget((int)System.Math.Round(resx / 2 ),(int)System.Math.Round(resy / 2 ), pars );			
			this.renderTargetEdgeBuffer2.Texture.Name = "OutlinePass.edge2";
			this.renderTargetEdgeBuffer2.Texture.GenerateMipmaps = false;

			var MAX_EDGE_THICKNESS = 4;
			var MAX_EDGE_GLOW = 4;

			this.separableBlurMaterial1 = this.GetSeperableBlurMaterial(MAX_EDGE_THICKNESS );
			((this.separableBlurMaterial1.Uniforms["texSize"] as GLUniform)["value"] as Vector2).Set((float)resx,(float)resy);
			(this.separableBlurMaterial1.Uniforms["kernelRadius"] as GLUniform)["value"] = 1;
			this.separableBlurMaterial2 = this.GetSeperableBlurMaterial(MAX_EDGE_GLOW );
			((this.separableBlurMaterial2.Uniforms["texSize"] as GLUniform)["value"] as Vector2).Set((float)System.Math.Round(resx / 2),(float)System.Math.Round(resy / 2 ));
			(this.separableBlurMaterial2.Uniforms["kernelRadius"] as GLUniform)["value"] = MAX_EDGE_GLOW;

			// Overlay material
			this.overlayMaterial = this.GetOverlayMaterial();

			// copy material
			
			var copyShader = new CopyShader();

			this.copyUniforms = UniformsUtils.CloneUniforms(copyShader.Uniforms );
			(this.copyUniforms["opacity"] as GLUniform)["value"] = 1.0f;

			this.materialCopy = new ShaderMaterial {
				Uniforms = this.copyUniforms,
				VertexShader = copyShader.VertexShader,
				FragmentShader = copyShader.FragmentShader,
				Blending = Constants.NoBlending,
				DepthTest = false,
				DepthWrite = false,
				Transparent = true
			};

			this.Enabled = true;
			this.NeedsSwap = false;

			this.oldClearColor = new Color();
			this.oldClearAlpha = 1;

			this.fullScreenQuad = new Pass.FullScreenQuad( null );

			this.tempPulseColor1 = new Color();
			this.tempPulseColor2 = new Color();
			this.textureMatrix = new Matrix4();
		}
		private string ReplaceDepthToViewZ(string s, Camera camera )
		{

			var type = camera is PerspectiveCamera ? "perspective" : "orthographic";

			return s.Replace("DEPTH_TO_VIEW_Z", type + "DepthToViewZ");

		}
		private void UpdateTextureMatrix()
		{

			this.textureMatrix.Set(0.5f, 0.0f, 0.0f, 0.5f,
				0.0f, 0.5f, 0.0f, 0.5f,
				0.0f, 0.0f, 0.5f, 0.5f,
				0.0f, 0.0f, 0.0f, 1.0f);
			this.textureMatrix.Multiply(this.renderCamera.ProjectionMatrix);
			this.textureMatrix.Multiply(this.renderCamera.MatrixWorldInverse);

		}
		private void ChangeVisibilityOfSelectedObjects(bool bVisible)
        {
			for (var i = 0; i < this.selectedObjects.Count; i++)
			{

				var selectedObject = this.selectedObjects[i];
				selectedObject.Traverse(obj=> 
				{
					if(obj is Mesh)
                    {
						if (bVisible)
						{

							obj.Visible = (bool)obj.UserData["oldVisible"];
							obj.UserData.Remove("oldVisible");							
						}
						else
						{

							obj.UserData["oldVisible"] = obj.Visible;
							obj.Visible = bVisible;

						}
					}
				});

			}
		}
		private void ChangeVisibilityOfNonSelectedObjects(bool bVisible)
        {
			List<Object3D> selectedMeshes = new List<Object3D>();

			for(int i = 0; i < this.selectedObjects.Count; i++)
            {
				var selectedObject = this.selectedObjects[i];
				selectedObject.Traverse(obj => 
				{
					if (obj is Mesh) selectedMeshes.Add(obj);
				});
            }

			this.renderScene.Traverse(obj => 
			{
				if (obj is Mesh || obj is Line || obj is Sprite)
				{

					var bFound = false;

					for (var i = 0; i < selectedMeshes.Count; i++)
					{

						var selectedObjectId = selectedMeshes[i].Id;

						if (selectedObjectId == obj.Id)
						{

							bFound = true;
							break;

						}

					}

					if (!bFound)
					{

						var visibility = obj.Visible;

						if (!bVisible || obj.bVisible) obj.Visible = bVisible;

						obj.bVisible = visibility;

					}

				}
			});
        }
		public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
			if (this.selectedObjects.Count > 0)
			{

				this.oldClearColor.Copy(renderer.GetClearColor());
				this.oldClearAlpha = renderer.GetClearAlpha();
				var oldAutoClear = renderer.AutoClear;

				renderer.AutoClear = false;

				if (maskActive!=null && maskActive.Value) renderer.state.buffers.stencil.SetTest(false);

				renderer.SetClearColor(Color.Hex(0xffffff), 1);

				// Make selected objects invisible
				this.ChangeVisibilityOfSelectedObjects(false);

				var currentBackground = this.renderScene.Background;
				this.renderScene.Background = null;

				// 1. Draw Non Selected objects in the depth buffer
				this.renderScene.OverrideMaterial = this.depthMaterial;
				renderer.SetRenderTarget(this.renderTargetDepthBuffer);
				renderer.Clear();
				renderer.Render(this.renderScene, this.renderCamera);

				// Make selected objects visible
				this.ChangeVisibilityOfSelectedObjects(true);

				// Update Texture Matrix for Depth compare
				this.UpdateTextureMatrix();

				// Make non selected objects invisible, and draw only the selected objects, by comparing the depth buffer of non selected objects
				this.ChangeVisibilityOfNonSelectedObjects(false);
				this.renderScene.OverrideMaterial = this.prepareMaskMaterial;
				((this.prepareMaskMaterial.Uniforms["cameraNearFar"] as GLUniform)["value"] as Vector2).Set(this.renderCamera.Near, this.renderCamera.Far);
				(this.prepareMaskMaterial.Uniforms["depthTexture"] as GLUniform)["value"] = this.renderTargetDepthBuffer.Texture;
				(this.prepareMaskMaterial.Uniforms["textureMatrix"] as GLUniform)["value"] = this.textureMatrix;
				renderer.SetRenderTarget(this.renderTargetMaskBuffer);
				renderer.Clear();
				renderer.Render(this.renderScene, this.renderCamera);
				this.renderScene.OverrideMaterial = null;
				this.ChangeVisibilityOfNonSelectedObjects(true);

				this.renderScene.Background = currentBackground;

				// 2. Downsample to Half resolution
				this.fullScreenQuad.material = this.materialCopy;
				(this.copyUniforms["tDiffuse"] as GLUniform)["value"] = this.renderTargetMaskBuffer.Texture;
				renderer.SetRenderTarget(this.renderTargetMaskDownSampleBuffer);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				this.tempPulseColor1.Copy(this.visibleEdgeColor);
				this.tempPulseColor2.Copy(this.hiddenEdgeColor);

				if (this.pulsePeriod > 0)
				{

					var scalar = (1 + 0.25f) / 2 + (float)System.Math.Cos(DateTime.Now.Ticks * 0.01f / this.pulsePeriod) * (1.0f - 0.25f) / 2;
					this.tempPulseColor1.MultiplyScalar(scalar);
					this.tempPulseColor2.MultiplyScalar(scalar);

				}

				// 3. Apply Edge Detection Pass
				this.fullScreenQuad.material = this.edgeDetectionMaterial;
				(this.edgeDetectionMaterial.Uniforms["maskTexture"] as GLUniform)["value"] = this.renderTargetMaskDownSampleBuffer.Texture;
				((this.edgeDetectionMaterial.Uniforms["texSize"] as GLUniform)["value"] as Vector2).Set(this.renderTargetMaskDownSampleBuffer.Width, this.renderTargetMaskDownSampleBuffer.Height);
				(this.edgeDetectionMaterial.Uniforms["visibleEdgeColor"] as GLUniform)["value"] = this.tempPulseColor1;
				(this.edgeDetectionMaterial.Uniforms["hiddenEdgeColor"] as GLUniform)["value"] = this.tempPulseColor2;
				renderer.SetRenderTarget(this.renderTargetEdgeBuffer1);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				// 4. Apply Blur on Half res
				this.fullScreenQuad.material = this.separableBlurMaterial1;
				(this.separableBlurMaterial1.Uniforms["colorTexture"] as GLUniform)["value"] = this.renderTargetEdgeBuffer1.Texture;
				(this.separableBlurMaterial1.Uniforms["direction"] as GLUniform)["value"] = OutlinePass.BlurDirectionX;
				(this.separableBlurMaterial1.Uniforms["kernelRadius"] as GLUniform)["value"] = this.edgeThickness;
				renderer.SetRenderTarget(this.renderTargetBlurBuffer1);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);
				(this.separableBlurMaterial1.Uniforms["colorTexture"] as GLUniform)["value"] = this.renderTargetBlurBuffer1.Texture;
				(this.separableBlurMaterial1.Uniforms["direction"] as GLUniform)["value"] = OutlinePass.BlurDirectionY;
				renderer.SetRenderTarget(this.renderTargetEdgeBuffer1);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				// Apply Blur on quarter res
				this.fullScreenQuad.material = this.separableBlurMaterial2;
				(this.separableBlurMaterial2.Uniforms["colorTexture"] as GLUniform)["value"] = this.renderTargetEdgeBuffer1.Texture;
				(this.separableBlurMaterial2.Uniforms["direction"] as GLUniform)["value"] = OutlinePass.BlurDirectionX;
				renderer.SetRenderTarget(this.renderTargetBlurBuffer2);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);
				(this.separableBlurMaterial2.Uniforms["colorTexture"] as GLUniform)["value"] = this.renderTargetBlurBuffer2.Texture;
				(this.separableBlurMaterial2.Uniforms["direction"] as GLUniform)["value"] = OutlinePass.BlurDirectionY;
				renderer.SetRenderTarget(this.renderTargetEdgeBuffer2);
				renderer.Clear();
				this.fullScreenQuad.Render(renderer);

				// Blend it additively over the input texture
				this.fullScreenQuad.material = this.overlayMaterial;
				(this.overlayMaterial.Uniforms["maskTexture"]as GLUniform)["value"] = this.renderTargetMaskBuffer.Texture;
				(this.overlayMaterial.Uniforms["edgeTexture1"]as GLUniform)["value"] = this.renderTargetEdgeBuffer1.Texture;
				(this.overlayMaterial.Uniforms["edgeTexture2"]as GLUniform)["value"] = this.renderTargetEdgeBuffer2.Texture;
				(this.overlayMaterial.Uniforms["patternTexture"]as GLUniform)["value"] = this.patternTexture;
                (this.overlayMaterial.Uniforms["edgeStrength"]as GLUniform)["value"] = this.edgeStrength;
				(this.overlayMaterial.Uniforms["edgeGlow"]as GLUniform)["value"] = this.edgeGlow;
				(this.overlayMaterial.Uniforms["usePatternTexture"]as GLUniform)["value"] = this.usePatternTexture;


				if (maskActive!=null && maskActive.Value) renderer.state.buffers.stencil.SetTest(true);

				renderer.SetRenderTarget(readBuffer);
				this.fullScreenQuad.Render(renderer);

				renderer.SetClearColor(this.oldClearColor, this.oldClearAlpha);
				renderer.AutoClear = oldAutoClear;

			}

			if (this.RenderToScreen)
			{

				this.fullScreenQuad.material = this.materialCopy;
				(this.copyUniforms["tDiffuse"] as GLUniform)["value"] = readBuffer.Texture;
				renderer.SetRenderTarget(null);
				this.fullScreenQuad.Render(renderer);

			}
		}

        public override void SetSize(float width, float height)
        {
			this.renderTargetMaskBuffer.SetSize((int)width, (int)height);

			var resx = (int)System.Math.Round(width / this.downSampleRatio);
			var resy = (int)System.Math.Round(height / this.downSampleRatio);
			this.renderTargetMaskDownSampleBuffer.SetSize(resx, resy);
			this.renderTargetBlurBuffer1.SetSize(resx, resy);
			this.renderTargetEdgeBuffer1.SetSize(resx, resy);
			var texSize = (this.separableBlurMaterial1.Uniforms["texSize"] as GLUniform)["value"] as Vector2;
			texSize.X = resx;texSize.Y=resy;

			resx = (int)System.Math.Round(resx / 2.0f);
			resy = (int)System.Math.Round(resy / 2.0f);

			this.renderTargetBlurBuffer2.SetSize(resx, resy);
			this.renderTargetEdgeBuffer2.SetSize(resx, resy);

			texSize = (this.separableBlurMaterial2.Uniforms["texSize"] as GLUniform)["value"] as Vector2;
			texSize.X = resx; texSize.Y = resy;
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
				this.renderTargetMaskBuffer.Dispose();
				this.renderTargetDepthBuffer.Dispose();
				this.renderTargetMaskDownSampleBuffer.Dispose();
				this.renderTargetBlurBuffer1.Dispose();
				this.renderTargetBlurBuffer2.Dispose();
				this.renderTargetEdgeBuffer1.Dispose();
				this.renderTargetEdgeBuffer2.Dispose();
				this.RaiseDisposed();
				this.disposed = true;
			}
			finally
			{

			}
			this.disposed = true;
		}

		private ShaderMaterial GetPrepareMaskMaterial()
        {
			return new ShaderMaterial
			{

				Uniforms = new GLUniforms {
					{ "depthTexture", new GLUniform { { "value", null } } },
					{ "cameraNearFar", new GLUniform { { "value", new Vector2(0.5f, 0.5f) } } },
					{ "textureMatrix", new GLUniform { { "value", null } } }
				},

				VertexShader = @"
				#include <morphtarget_pars_vertex>
				#include <skinning_pars_vertex>

				varying vec4 projTexCoord;
				varying vec4 vPosition;
				uniform mat4 textureMatrix;

				void main() {

					#include <skinbase_vertex>
					#include <begin_vertex>
					#include <morphtarget_vertex>
					#include <skinning_vertex>
					#include <project_vertex>

					vPosition = mvPosition;
					vec4 worldPosition = modelMatrix * vec4( position, 1.0 );
					projTexCoord = textureMatrix * worldPosition;

				}
				",


				FragmentShader = @"
				#include <packing>
				varying vec4 vPosition;
				varying vec4 projTexCoord;
				uniform sampler2D depthTexture;
				uniform vec2 cameraNearFar;

				void main() {

					float depth = unpackRGBAToDepth(texture2DProj( depthTexture, projTexCoord ));
					float viewZ = - DEPTH_TO_VIEW_Z( depth, cameraNearFar.x, cameraNearFar.y );
					float depthTest = (-vPosition.z > viewZ) ? 1.0 : 0.0;
					gl_FragColor = vec4(0.0, depthTest, 1.0, 1.0);

				}
				"
			}; 
        }
		private ShaderMaterial GetEdgeDetectionMaterial()
        {
			return new ShaderMaterial
			{

				Uniforms = new GLUniforms {
				{ "maskTexture",new GLUniform { {"value", null } } },
				{ "texSize",new GLUniform { {"value", new Vector2(0.5f, 0.5f) } } },
				{ "visibleEdgeColor",new GLUniform { {"value", new Vector3(1.0f, 1.0f, 1.0f) } } },
				{ "hiddenEdgeColor",new GLUniform { {"value", new Vector3(1.0f, 1.0f, 1.0f) } } }
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
			uniform sampler2D maskTexture;
			uniform vec2 texSize;
			uniform vec3 visibleEdgeColor;
			uniform vec3 hiddenEdgeColor;
				
			void main()
			{
					vec2 invSize = 1.0 / texSize;
					vec4 uvOffset = vec4(1.0, 0.0, 0.0, 1.0) * vec4(invSize, invSize);
					vec4 c1 = texture2D(maskTexture, vUv + uvOffset.xy);
					vec4 c2 = texture2D(maskTexture, vUv - uvOffset.xy);
					vec4 c3 = texture2D(maskTexture, vUv + uvOffset.yw);
					vec4 c4 = texture2D(maskTexture, vUv - uvOffset.yw);
					float diff1 = (c1.r - c2.r) * 0.5;
					float diff2 = (c3.r - c4.r) * 0.5;
					float d = length(vec2(diff1, diff2));
					float a1 = min(c1.g, c2.g);
					float a2 = min(c3.g, c4.g);
					float visibilityFactor = min(a1, a2);
					vec3 edgeColor = 1.0 - visibilityFactor > 0.001 ? visibleEdgeColor : hiddenEdgeColor;
					gl_FragColor = vec4(edgeColor, 1.0) * vec4(d);
				}
			"
			};
        }
		private ShaderMaterial GetSeperableBlurMaterial(float maxRadius)
        {
			return new ShaderMaterial
			{

				Defines = new Hashtable {
				{ "MAX_RADIUS", maxRadius.ToString() }
			},

				Uniforms = new GLUniforms
			{
				{ "colorTexture",new GLUniform { {"value", null } } },
				{ "texSize", new GLUniform{ {"value", new Vector2(0.5f, 0.5f) } } },
				{ "direction", new GLUniform{ {"value", new Vector2(0.5f, 0.5f) } } },
				{ "kernelRadius", new GLUniform{ {"value", 1.0f } } }
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
			#include <common>
			varying vec2 vUv;
			uniform sampler2D colorTexture;
			uniform vec2 texSize;
			uniform vec2 direction;
			uniform float kernelRadius;
			
			float gaussianPdf(in float x, in float sigma)
			{
				return 0.39894 * exp(-0.5 * x * x / (sigma * sigma)) / sigma;
			}
			void main()
			{
				vec2 invSize = 1.0 / texSize;
				float weightSum = gaussianPdf(0.0, kernelRadius);
				vec4 diffuseSum = texture2D(colorTexture, vUv) * weightSum;
				vec2 delta = direction * invSize * kernelRadius / float(MAX_RADIUS);
				vec2 uvOffset = delta;
				for (int i = 1; i <= MAX_RADIUS; i++)
				{
					float w = gaussianPdf(uvOffset.x, kernelRadius);
					vec4 sample1 = texture2D(colorTexture, vUv + uvOffset);
					vec4 sample2 = texture2D(colorTexture, vUv - uvOffset);
					diffuseSum += ((sample1 + sample2) * w);
					weightSum += (2.0 * w);
					uvOffset += delta;
				}
				gl_FragColor = diffuseSum / weightSum;
			}
			"
			}; 
        }
		private ShaderMaterial GetOverlayMaterial()
        {
			return new ShaderMaterial {

				Uniforms = new GLUniforms {
					{ "maskTexture", new GLUniform{{ "value", null } } },
					{ "edgeTexture1",new GLUniform{{ "value", null } } },
					{ "edgeTexture2",new GLUniform {{ "value", null } } },
					{ "patternTexture", new GLUniform{{ "value", null } } },
					{ "edgeStrength",  new GLUniform{{"value", 1.0 } } },
					{ "edgeGlow",  new GLUniform {{"value", 1.0 } } },
					{ "usePatternTexture", new GLUniform {{ "value", 0.0 } } }
				},

				VertexShader=@"
				varying vec2 vUv;
				void main()
				{
						vUv = uv;
						gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
					}
				",

				FragmentShader =@"
				varying vec2 vUv;
				uniform sampler2D maskTexture;
				uniform sampler2D edgeTexture1;
				uniform sampler2D edgeTexture2;
				uniform sampler2D patternTexture;
				uniform float edgeStrength;
				uniform float edgeGlow;
				uniform bool usePatternTexture;
				
				void main()
				{
					vec4 edgeValue1 = texture2D(edgeTexture1, vUv);
					vec4 edgeValue2 = texture2D(edgeTexture2, vUv);
					vec4 maskColor = texture2D(maskTexture, vUv);
					vec4 patternColor = texture2D(patternTexture, 6.0 * vUv);
					float visibilityFactor = 1.0 - maskColor.g > 0.0 ? 1.0 : 0.5;
					vec4 edgeValue = edgeValue1 + edgeValue2 * edgeGlow;
					vec4 finalColor = edgeStrength * maskColor.r * edgeValue;
					if (usePatternTexture)
						finalColor += +visibilityFactor * (1.0 - maskColor.r) * (1.0 - patternColor.r);
					gl_FragColor = finalColor;
				}
				",
				Blending= Constants.AdditiveBlending,
				DepthTest= false,
				DepthWrite= false,
				Transparent= true
			};
        }
	}
}
