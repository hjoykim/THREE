using DrawingGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using THREE;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace THREEExample.Three.CustomAttributes
{
    [Example("Lines",ExampleCategory.ThreeJs,"custom attributes")]
    internal class CustomAttributesLines : ExampleTemplate
    {
        string vertexShader = @"
			uniform float amplitude;

			attribute vec3 displacement;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {

				vec3 newPosition = position + amplitude * displacement;

				vColor = customColor;

				gl_Position = projectionMatrix * modelViewMatrix * vec4( newPosition, 1.0 );

			}
";
        string fragmentShader = @"
			uniform vec3 color;
			uniform float opacity;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( vColor * color, opacity );

			}
";
		Line line;
		GLUniforms uniforms;
        public CustomAttributesLines() :base()
		{
			scene.Background = THREE.Color.Hex(0x050505);
		}
		public override void InitCamera()
		{
			camera = new THREE.PerspectiveCamera(30, glControl.AspectRatio, 1, 10000);
			camera.Position.Z = 400;
        }
		public override void Init()
		{
			base.Init();
			BuildScene();
		}
		public void BuildScene()
		{
			uniforms = new GLUniforms
			{
				{ "amplitude", new GLUniform { { "value", 5.0f } } },
				{ "opacity", new GLUniform { { "value", 0.3f } } },
				{ "color", new GLUniform { { "value", THREE.Color.Hex(0xffffff) } } }
			};
			var shaderMaterial = new THREE.ShaderMaterial()
			{

				Uniforms = uniforms,
				VertexShader = vertexShader,
				FragmentShader = fragmentShader,
				Blending = Constants.AdditiveBlending,
				DepthTest = false,
				Transparent = true
			};

			var font = THREE.FontLoader.Load("../../../../assets/fonts/helvetiker_bold.typeface.json");

			var parameter = new Hashtable
			{
				{"font", font },

					{"size", 50 },
					{"height", 15 },
					{"curveSegments", 10 },

					{"bevelThickness", 5 },
					{"bevelSize", 1.5f },
					{"bevelEnabled", true },
					{"bevelSegments", 10 }
			};
			var geometry = new TextBufferGeometry("three.js", parameter);
			geometry.Center();
            var count = (geometry.Attributes["position"] as BufferAttribute<float>).count;

            var displacement = new BufferAttribute<float>(new float[count * 3], 3);
            geometry.SetAttribute("displacement", displacement);

            var customColor = new BufferAttribute<float>(new float[count * 3], 3);
            geometry.SetAttribute("customColor", customColor);

            var color = new THREE.Color(0xffffff);

            for (var i = 0;i< customColor.count; i++)
            {

                color.SetHSL(1.0f*i / customColor.count, 0.5f, 0.5f);
                color.ToArray(customColor.Array, i * customColor.ItemSize);

            }

            line = new THREE.Line(geometry, shaderMaterial);
            line.Rotation.X = 0.2f;
            scene.Add(line);
        }
		public override void Render()
		{
            float time = (float)stopWatch.Elapsed.TotalMilliseconds * 0.001f;

            line.Rotation.Y = 0.25f * time;

			(uniforms["amplitude"] as GLUniform)["value"] = (float)Math.Sin(0.5 * time);
			THREE.Color color = (THREE.Color)(uniforms["color"] as GLUniform)["value"];
			color.OffsetHSL(0.0005f, 0, 0);
			(uniforms["color"] as GLUniform)["value"] = color;

            var attributes = (line.Geometry as TextBufferGeometry).Attributes;
            var array = (attributes["displacement"] as BufferAttribute<float>).Array;

            for (var i = 0;i< array.Length;  i += 3)
            {

                array[i] += 0.3f * (0.5f - MathUtils.NextFloat());
                array[i + 1] += 0.3f * (0.5f - MathUtils.NextFloat());
                array[i + 2] += 0.3f * (0.5f- MathUtils.NextFloat());

            }

			(attributes["displacement"] as BufferAttribute<float>).NeedsUpdate = true;
            base.Render();
		}
	}
}
