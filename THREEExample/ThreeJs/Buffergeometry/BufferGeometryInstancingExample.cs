using ImGuiNET;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Three.Buffergeometry
{
    [Example("instancing", ExampleCategory.ThreeJs, "Buffergeometry")]
    public class BufferGeometryInstancingExample : Example
    {
		string vertexShader = @"
		precision highp float;

		uniform float sineTime;

		uniform mat4 modelViewMatrix;
		uniform mat4 projectionMatrix;

		attribute vec3 position;
		attribute vec3 offset;
		attribute vec4 color;
		attribute vec4 orientationStart;
		attribute vec4 orientationEnd;

		varying vec3 vPosition;
		varying vec4 vColor;

		void main(){

			vPosition = offset * max( abs( sineTime * 2.0 + 1.0 ), 0.5 ) + position;
			vec4 orientation = normalize( mix( orientationStart, orientationEnd, sineTime ) );
			vec3 vcV = cross( orientation.xyz, vPosition );
			vPosition = vcV * ( 2.0 * orientation.w ) + ( cross( orientation.xyz, vcV ) * 2.0 + vPosition );

			vColor = color;

			gl_Position = projectionMatrix * modelViewMatrix * vec4( vPosition, 1.0 );

		}
";
		string fragmentShader = @"
		precision highp float;

		uniform float time;

		varying vec3 vPosition;
		varying vec4 vColor;

		void main() {

			vec4 color = vec4( vColor );
			color.r += sin( vPosition.x * 10.0 + time ) * 0.5;

			gl_FragColor = color;

		}
";
		Mesh mesh;
		InstancedBufferGeometry geometry;
		int instances = 50000;
		public BufferGeometryInstancingExample() : base()
        {
			stopWatch.Start();
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(50, glControl.AspectRatio, 1, 10);
            camera.Position.Z = 2;
        }
		public override void Init()
		{
			base.Init();

			this.renderer.SetClearColor(Color.Hex(0x000000));
			var vector = new Vector4();

			

			var positions = new List<float>();
			var offsets = new List<float>();
			var colors = new List<float>();
			var orientationsStart = new List<float>();
			var orientationsEnd = new List<float>();

			positions.Add(0.025f, -0.025f, 0);
			positions.Add(-0.025f, 0.025f, 0);
			positions.Add(0, 0, 0.025f);

			// instanced attributes

			for (var i = 0; i < instances; i++)
			{

				// offsets

				offsets.Add(MathUtils.NextFloat() - 0.5f, MathUtils.NextFloat() - 0.5f, MathUtils.NextFloat() - 0.5f);

				// colors

				colors.Add(MathUtils.NextFloat(0, 1), MathUtils.NextFloat(0, 1), MathUtils.NextFloat(0, 1), MathUtils.NextFloat(0, 1));

				// orientation start

				vector.Set(MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1);
				vector.Normalize();

				orientationsStart.Add(vector.X, vector.Y, vector.Z, vector.W);

				// orientation end

				vector.Set(MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1, MathUtils.NextFloat() * 2 - 1);
				vector.Normalize();

				orientationsEnd.Add(vector.X, vector.Y, vector.Z, vector.W);

			}

			geometry = new InstancedBufferGeometry();
			geometry.InstanceCount = instances; // set so its initalized for dat.GUI, will be set in first draw otherwise

			geometry.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));

			geometry.SetAttribute("offset", new InstancedBufferAttribute<float>(offsets.ToArray(), 3));
			geometry.SetAttribute("color", new InstancedBufferAttribute<float>(colors.ToArray(), 4));
			geometry.SetAttribute("orientationStart", new InstancedBufferAttribute<float>(orientationsStart.ToArray(), 4));
			geometry.SetAttribute("orientationEnd", new InstancedBufferAttribute<float>(orientationsEnd.ToArray(), 4));

			var material = new RawShaderMaterial() {

				Uniforms = new GLUniforms {
					{ "time",new GLUniform{ { "value", 1.0f } } },
					{ "sineTime",new GLUniform{{"value", 1.0f } } }
				},
				VertexShader = vertexShader,
				FragmentShader = fragmentShader,
				Side = Constants.DoubleSide,
				Transparent = true

			};

			mesh = new Mesh(geometry, material);
			scene.Add(mesh);

			AddGuiControlsAction = AddControls;
		}
        
        public override void Render()
        {
			var time = stopWatch.ElapsedMilliseconds;

			mesh.Rotation.Y = time * 0.0005f;
			((mesh.Material as RawShaderMaterial).Uniforms["time"] as GLUniform)["value"] = time * 0.005f;
			var timeValue = (float)((mesh.Material as RawShaderMaterial).Uniforms["time"] as GLUniform)["value"];
			((mesh.Material as RawShaderMaterial).Uniforms["sineTime"] as GLUniform)["value"] = (float)System.Math.Sin(timeValue * 0.05);

			renderer.Render(scene, camera);
			
			ShowGUIControls();
		}
		private void AddControls()
        {
			ImGui.SliderInt("instanceCount", ref geometry.InstanceCount, 0, instances);
           
        }
    }
}
