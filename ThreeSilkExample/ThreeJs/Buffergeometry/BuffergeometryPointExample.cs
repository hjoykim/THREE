using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("buffergeometry_point",ExampleCategory.ThreeJs,"Buffergeometry")]
    public class BuffergeometryPointExample :Example
    {
        public Points points;
        public float time;

        public BuffergeometryPointExample() : base() { }

        public override void InitCamera()
        {
            camera = new PerspectiveCamera(27, this.AspectRatio, 5, 3500);
            camera.Position.Z = 2750;
        }
        public override void Init()
        {
            base.Init();

            scene.Background = Color.Hex(0x050505);
            scene.Fog = new Fog(0x050505, 2000, 3500);
            CreateObject();

			float time = DateTime.Now.Millisecond * 0.001f;
		}
		public virtual void CreateObject()
		{
			var particles = 500000;

			var geometry = new BufferGeometry();

			var positions = new List<float>();
			var colors = new List<float>();

			var color = new Color();

			var n = 1000;
			var n2 = n / 2; // particles spread in the cube

			for (var i = 0; i < particles; i++)
			{

				// positions

				var x = MathUtils.NextFloat() * n - n2;
				var y = MathUtils.NextFloat() * n - n2;
				var z = MathUtils.NextFloat() * n - n2;

				positions.Add(x, y, z);

				// colors

				var vx = (x / n) + 0.5f;
				var vy = (y / n) + 0.5f;
				var vz = (z / n) + 0.5f;

				color.SetRGB(vx, vy, vz);

				colors.Add(color.R, color.G, color.B);

			}

			geometry.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));
			geometry.SetAttribute("color", new BufferAttribute<float>(colors.ToArray(), 3));

			geometry.ComputeBoundingSphere();

			//

			var material = new PointsMaterial() { Size = 15, VertexColors = true };

			points = new Points(geometry, material);
			scene.Add(points);
		}

        public override void Render()
        {
			time = time + 0.001f;
			points.Rotation.X = time * 0.25f;
			points.Rotation.Y = time * 0.5f;
			base.Render();
        }
    }
}
