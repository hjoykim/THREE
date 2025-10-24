using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Three.Buffergeometry
{
    [Example("buffergeometry_rawshader", ExampleCategory.ThreeJs, "Buffergeometry")]
    public class BuffergeometryRawShaderExample : Example
    {
        string vertex = @"
			uniform mat4 modelViewMatrix; // optional
			uniform mat4 projectionMatrix; // optional

			attribute vec3 position;
			attribute vec4 color;

			varying vec3 vPosition;
			varying vec4 vColor;

			void main()	{

				vPosition = position;
				vColor = color;

				gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

			}

";
        string fragment = @"
			uniform float time;

			varying vec3 vPosition;
			varying vec4 vColor;

			void main()	{

				vec4 color = vec4( vColor );
				color.r += sin( vPosition.x * 10.0 + time ) * 0.5;

				gl_FragColor = color;

			}
";
		float time;
		Mesh mesh;
        public BuffergeometryRawShaderExample() : base() { }

		public override void InitCamera()
		{
			camera = new PerspectiveCamera(50, glControl.AspectRatio, 1, 10);
			camera.Position.Z = 2;
		}
		public override void Init()
		{
			base.Init();

			scene.Background = Color.Hex(0x101010);
			CreateObject();

			float time = DateTime.Now.Millisecond * 0.001f;
		}

		public void CreateObject()
		{
			var vertexCount = 200 * 3;

			var geometry = new BufferGeometry();

			var positions = new List<float>();
			var colors = new List<float>();

			for (var i = 0; i < vertexCount; i++)
			{

				// adding x,y,z
				positions.Add(MathUtils.NextFloat() - 0.5f);
				positions.Add(MathUtils.NextFloat() - 0.5f);
				positions.Add(MathUtils.NextFloat() - 0.5f);

				// adding r,g,b,a
				colors.Add(MathUtils.NextFloat(0,1));
				colors.Add(MathUtils.NextFloat(0,1));
				colors.Add(MathUtils.NextFloat(0,1));
				colors.Add(MathUtils.NextFloat(0,1));

			}

			var positionAttribute = new BufferAttribute<float>(positions.ToArray(), 3);
			var colorAttribute = new BufferAttribute<float>(colors.ToArray(), 4);

			colorAttribute.Normalized = true; // this will map the buffer values to 0.0f - +1.0f in the shader

			geometry.SetAttribute("position", positionAttribute);
			geometry.SetAttribute("color", colorAttribute);

			// material

			var material = new RawShaderMaterial()
			{

				Uniforms = new GLUniforms() {
						{ "time",new GLUniform() {{ "value", 1.0f} } }
				},
				VertexShader = vertex,
				FragmentShader = fragment,
				Side = Constants.DoubleSide,
				Transparent = true

			};

			mesh = new Mesh(geometry, material);
			scene.Add(mesh );
        }
        public override void Render()
        {
            time = time + 1f;
            mesh.Rotation.Y = time * 0.0005f;
            ((mesh.Material as RawShaderMaterial).Uniforms["time"] as GLUniform)["value"] = time * 0.005f;

            base.Render();
        }
    }
}
