using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Core;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;

namespace THREEExample.Three.Buffergeometry
{
    [Example("BuffergeometryUint", ExampleCategory.ThreeJs, "Buffergeometry")]
    public class BuffergeometryUintExample : BuffergeometryExample
    {

        public BuffergeometryUintExample() : base() { }

       

       
		public override void CreateObject()
		{

			var triangles = 500000;

			var geometry = new BufferGeometry();

			var positions = new List<float>();
			var normals = new List<uint>();
			var colors = new List<byte>();

			var color = new Color();

			var n = 800;
			var n2 = n / 2;    // triangles spread in the cube
			var d = 12;
			var d2 = d / 2; // individual triangle size

			var pA = new Vector3();
			var pB = new Vector3();
			var pC = new Vector3();

			var cb = new Vector3();
			var ab = new Vector3();

			for (var i = 0; i < triangles; i++)
			{

				// positions

				var x = (float)MathUtils.random.NextDouble() * n - n2;
				var y = (float)MathUtils.random.NextDouble() * n - n2;
				var z = (float)MathUtils.random.NextDouble() * n - n2;

				var ax = x + (float)MathUtils.random.NextDouble() * d - d2;
				var ay = y + (float)MathUtils.random.NextDouble() * d - d2;
				var az = z + (float)MathUtils.random.NextDouble() * d - d2;

				var bx = x + (float)MathUtils.random.NextDouble() * d - d2;
				var by = y + (float)MathUtils.random.NextDouble() * d - d2;
				var bz = z + (float)MathUtils.random.NextDouble() * d - d2;

				var cx = x + (float)MathUtils.random.NextDouble() * d - d2;
				var cy = y + (float)MathUtils.random.NextDouble() * d - d2;
				var cz = z + (float)MathUtils.random.NextDouble() * d - d2;

				positions.Add(ax, ay, az);
				positions.Add(bx, by, bz);
				positions.Add(cx, cy, cz);

				// flat face normals

				pA.Set(ax, ay, az);
				pB.Set(bx, by, bz);
				pC.Set(cx, cy, cz);

				cb.SubVectors(pC, pB);
				ab.SubVectors(pA, pB);
				cb.Cross(ab);

				cb.Normalize();

                //var nx = cb.X;
                //var ny = cb.Y;
                //var nz = cb.Z;

                //normals.Add(nx, ny, nz);
                //normals.Add(nx, ny, nz);
                //normals.Add(nx, ny, nz);

                var nx = (uint)cb.X;
                var ny = (uint)cb.Y;
                var nz = (uint)cb.Z;



                normals.Add(nx * 32767, ny * 32767, nz * 32767);
                normals.Add(nx * 32767, ny * 32767, nz * 32767);
                normals.Add(nx * 32767, ny * 32767, nz * 32767);

                // colors

                var vx = (x / n) + 0.5f;
				var vy = (y / n) + 0.5f;
				var vz = (z / n) + 0.5f;

				color.SetRGB(vx, vy, vz);

				colors.Add((byte)(color.R*255),(byte)(color.G*255),(byte)(color.B*255));
				colors.Add((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
				colors.Add((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
				//colors.Add(color.R, color.G, color.B);
				//colors.Add(color.R, color.G, color.B);


			}



			geometry.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));
			geometry.SetAttribute("normal", new BufferAttribute<uint>(normals.ToArray(), 3));
			geometry.SetAttribute("color", new BufferAttribute<byte>(colors.ToArray(), 3));

			geometry.ComputeBoundingSphere();

			var material = new MeshPhongMaterial()
			{
				Color = Color.Hex(0xaaaaaa),
				Specular = Color.Hex(0xffffff),
				Shininess = 250,
				Side = Constants.DoubleSide,
				VertexColors = true
			};

			mesh = new Mesh(geometry, material);
			scene.Add(mesh);
		}

        public override void Render()
        {
			time = time + 0.001f;
			mesh.Rotation.X = time * 0.25f;
			mesh.Rotation.Y = time * 0.5f;
            base.Render();
        }
    }
}
