using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class TorusKnotGeometry : Geometry
	{
		public Hashtable parameters;
		public TorusKnotGeometry(float? radius = null, float? tube = null, float? tubularSegments = null, float? radialSegments = null, float? p = null, float? q = null) : base()
		{
			parameters = new Hashtable()
			{
				{"radius",radius },
				{"tube",radius },
				{"radialSegments",radius },
				{"tubularSegments",radius },
				{"p",p},
				 {"q",q}
			};

			this.FromBufferGeometry(new TorusKnotBufferGeometry(radius, tube, tubularSegments, radialSegments, p, q));
			this.MergeVertices();
		}
	}

	public class TorusKnotBufferGeometry : BufferGeometry
	{
		public Hashtable parameters;

		public TorusKnotBufferGeometry(float? radius = null, float? tube = null, float? tubularSegments = null, float? radialSegments = null, float? p = null, float? q = null) : base()
		{
			radius = radius != null ? radius : 1;
			tube = tube != null ? tube : 0.4f;
			radialSegments = radialSegments != null ? (float)Math.Floor(radialSegments.Value) : 64;
			tubularSegments = tubularSegments != null ? (float)Math.Floor(tubularSegments.Value) : 8;
			p = p != null ? p : 2;
			q = q != null ? q : 3;

			parameters = new Hashtable()
			{
				{"radius",radius },
				{"tube",radius },
				{"radialSegments",radius },
				{"tubularSegments",radius },
				{"p",p},
				 {"q",q}
			};

			int i, j;

			List<int> indices = new List<int>();
			List<float> vertices = new List<float>();
			List<float> normals = new List<float>();
			List<float> uvs = new List<float>();

			var vertex = new Vector3();
			var normal = new Vector3();

			var P1 = new Vector3();
			var P2 = new Vector3();

			var B = new Vector3();
			var T = new Vector3();
			var N = new Vector3();
			// generate vertices, normals and uvs

			for (i = 0; i <= tubularSegments; ++i)
			{

				// the radian "u" is used to calculate the position on the torus curve of the current tubular segement

				float u = i / (float)tubularSegments * (float)p * (float)Math.PI * 2;

				// now we calculate two points. P1 is our current position on the curve, P2 is a little farther ahead.
				// these points are used to create a special "coordinate space", which is necessary to calculate the correct vertex positions

				CalculatePositionOnCurve(u, (float)p, (float)q, (float)radius, P1);
				CalculatePositionOnCurve(u + 0.01f, (float)p, (float)q, (float)radius, P2);

				// calculate orthonormal basis

				T.SubVectors(P2, P1);
				N.AddVectors(P2, P1);
				B.CrossVectors(T, N);
				N.CrossVectors(B, T);

				// normalize B, N. T can be ignored, we don't use it

				B.Normalize();
				N.Normalize();

				for (j = 0; j <= radialSegments; ++j)
				{

					// now calculate the vertices. they are nothing more than an extrusion of the torus curve.
					// because we extrude a shape in the xy-plane, there is no need to calculate a z-value.

					var v = (float)(j / radialSegments * Math.PI * 2);
					var cx = (float)(-tube * Math.Cos(v));
					var cy = (float)(tube * Math.Sin(v));

					// now calculate the final vertex position.
					// first we orient the extrusion with our basis vectos, then we add it to the current position on the curve

					vertex.X = P1.X + (cx * N.X + cy * B.X);
					vertex.Y = P1.Y + (cx * N.Y + cy * B.Y);
					vertex.Z = P1.Z + (cx * N.Z + cy * B.Z);

					vertices.Add(vertex.X, vertex.Y, vertex.Z);

					// normal (P1 is always the center/origin of the extrusion, thus we can use it to calculate the normal)

					normal.SubVectors(vertex, P1).Normalize();

					normals.Add(normal.X, normal.Y, normal.Z);

					// uv

					uvs.Add(i / (float)tubularSegments);
					uvs.Add(j / (float)radialSegments);

				}

			}

			// generate indices

			for (j = 1; j <= tubularSegments; j++)
			{

				for (i = 1; i <= radialSegments; i++)
				{

					// indices

					var a = ((int)radialSegments + 1) * (j - 1) + (i - 1);
					var b = ((int)radialSegments + 1) * j + (i - 1);
					var c = ((int)radialSegments + 1) * j + i;
					var d = ((int)radialSegments + 1) * (j - 1) + i;

					// faces

					indices.Add(a, b, d);
					indices.Add(b, c, d);

				}

			}

			// build geometry

			this.SetIndex(indices);
			this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
			this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
			this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));

			// this function calculates the current position on the torus curve


		}
		private void CalculatePositionOnCurve(float u, float p, float q, float radius, Vector3 position )
		{

			var cu = (float)Math.Cos(u);
			var su = (float)Math.Sin(u);
			var quOverP = q / p * u;
			var cs = (float)Math.Cos(quOverP);

			position.X = radius * (2 + cs) * 0.5f * cu;
			position.Y = radius * (2 + cs) * su * 0.5f;
			position.Z = radius * (float)Math.Sin(quOverP) * 0.5f;

		}
	}
}