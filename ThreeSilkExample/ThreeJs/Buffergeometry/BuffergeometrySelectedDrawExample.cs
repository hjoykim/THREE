using ImGuiNET;
using System;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("buffergeometry_selective_draw", ExampleCategory.ThreeJs, "Buffergeometry")]
    public class BuffergeometrySelectedDrawExample : Example
    {
        LineSegments mesh;
        BufferGeometry geometry;

        string vertex = @"
		attribute float visible;
		varying float vVisible;
		attribute vec3 vertColor;
		varying vec3 vColor;

		void main() {

			vColor = vertColor;
			vVisible = visible;
			gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

		}
";
        string fragment = @"
		varying float vVisible;
		varying vec3 vColor;

		void main() {

			if ( vVisible > 0.0 ) {

				gl_FragColor = vec4( vColor, 1.0 );

			} else {

				discard;

			}

		}
";
		int numLat = 100;
		int numLng = 200;
		int numLinesCulled = 0;
		float time;
		public BuffergeometrySelectedDrawExample() : base() { }

        public override void InitCamera()
        {
			camera = new PerspectiveCamera(45, this.AspectRatio, 0.01f, 10);
			camera.Position.Z = 3.5f;
		}

        public override void InitLighting()
        {
			scene.Add(new AmbientLight(0x444444));
		}

        public override void Init()
        {
            base.Init();

			AddLines(1.0f);

        }
		private void AddLines(float radius)
		{
			geometry = new BufferGeometry();
			var linePositions = new float[numLat * numLng * 3 * 2];
			var lineColors = new float[numLat * numLng * 3 * 2];
			var visible = new float[numLat * numLng * 2];

			for (var i = 0; i < numLat; ++i)
			{

				for (var j = 0; j < numLng; ++j)
				{

					var lat = (MathUtils.NextFloat(0,1000) * Math.PI) / 50.0 + i / numLat * Math.PI;
					var lng = (MathUtils.NextFloat(0,1000) * Math.PI) / 50.0 + j / numLng * 2 * Math.PI;

					var index = i * numLng + j;

					linePositions[index * 6 + 0] = 0;
					linePositions[index * 6 + 1] = 0;
					linePositions[index * 6 + 2] = 0;
					linePositions[index * 6 + 3] = (float)(radius * Math.Sin(lat) * Math.Cos(lng));
					linePositions[index * 6 + 4] = (float)(radius * Math.Cos(lat));
					linePositions[index * 6 + 5] = (float)(radius * Math.Sin(lat) * Math.Sin(lng));

					var color = Color.Hex(0xffffff);

					color.SetHSL((float)(lat / Math.PI), 1.0f, 0.2f);
					lineColors[index * 6 + 0] = color.R;
					lineColors[index * 6 + 1] = color.G;
					lineColors[index * 6 + 2] = color.B;

					color.SetHSL((float)(lat / Math.PI), 1.0f, 0.7f);
					lineColors[index * 6 + 3] = color.R;
					lineColors[index * 6 + 4] = color.G;
					lineColors[index * 6 + 5] = color.B;

					// non-0 is visible
					visible[index * 2 + 0] = 1.0f;
					visible[index * 2 + 1] = 1.0f;

				}

			}

			geometry.SetAttribute("position", new BufferAttribute<float>(linePositions, 3));
			geometry.SetAttribute("vertColor", new BufferAttribute<float>(lineColors, 3));
			geometry.SetAttribute("visible", new BufferAttribute<float>(visible, 1));

			geometry.ComputeBoundingSphere();

			var shaderMaterial = new ShaderMaterial()
			{

				VertexShader = vertex,
				FragmentShader = fragment
			};

			mesh = new LineSegments(geometry, shaderMaterial);
			scene.Add(mesh);

			AddGuiControlsAction = UpdateCount;
		}
        public override void Render()
        {
			time = time + 0.01f;
			mesh.Rotation.X = time * 0.025f;
			mesh.Rotation.Y = time * 0.05f;

            base.Render();
        }

		private void HideLines()
        {
			for (var i = 0; i < (geometry.Attributes["visible"] as BufferAttribute<float>).Array.Length; i += 2)
			{

				if (MathUtils.NextFloat() > 0.75f)
				{

					if ((geometry.Attributes["visible"] as BufferAttribute<float>).Array[i + 0]!=0)
					{

						++numLinesCulled;

					}

					(geometry.Attributes["visible"] as BufferAttribute<float>).Array[i + 0] = 0;
					(geometry.Attributes["visible"] as BufferAttribute<float>).Array[i + 1] = 0;

				}

			}

			(geometry.Attributes["visible"] as BufferAttribute<float>).NeedsUpdate = true;
		}
		private void ShowAllLines()
        {
			numLinesCulled = 0;

			for (var i = 0; i < (geometry.Attributes["visible"] as BufferAttribute<float>).Array.Length; i += 2)
			{

				(geometry.Attributes["visible"] as BufferAttribute<float>).Array[i + 0] = 1;
				(geometry.Attributes["visible"] as BufferAttribute<float>).Array[i + 1] = 1;

			}

			(geometry.Attributes["visible"] as BufferAttribute<float>).NeedsUpdate = true;
		}

		private void UpdateCount()
        {
			ImGui.Text("1 draw call, " + numLat * numLng + " lines, " + numLinesCulled + " culled ");
			if (ImGui.Button("hideLines"))
				HideLines();
			if (ImGui.Button("showAllLines"))
				ShowAllLines();

		}
    }
}
