using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class PolyhedronGeometry : Geometry
    {
		public PolyhedronGeometry(List<float> vertices, List<int> indices, float radius, float detail) : base() 
		{
			this.FromBufferGeometry(new PolyhedronBufferGeometry(vertices, indices, radius, detail));
			this.MergeVertices();
		}
	}
    public class PolyhedronBufferGeometry : BufferGeometry
    {
        List<float> vertices;

        List<int> indices;

        List<float> vertexBuffer = new List<float>();

        List<float> uvBuffer = new List<float>();

		public Hashtable parameters;

		public PolyhedronBufferGeometry() : base() 
		{
		}

		public PolyhedronBufferGeometry(List<float> vertices,List<int> indices,float? radius=null,float? detail=null) : this() 
        {
			parameters = new Hashtable()
			{
				{"vertices",vertices },
				{"indices",indices },
				{"radius",radius },
				{"detail",detail }
			};

			this.vertices = vertices;
			this.indices = indices;
			radius = radius != null ? radius : 1;
			detail = detail != null ? (float)detail : 0;

			if (radius == 0) radius = 1;

			Subdivide(detail.Value);

			ApplyRadius(radius.Value);

			GenerateUVs();

			this.SetAttribute("position", new BufferAttribute<float>(vertexBuffer.ToArray(), 3));
			this.SetAttribute("normal", new BufferAttribute<float>(vertexBuffer.ToArray(), 3));
			this.SetAttribute("uv", new BufferAttribute<float>(uvBuffer.ToArray(), 2));

			if (detail == 0)
			{
				this.ComputeVertexNormals();
			}
			else
			{
				this.NormalizeNormals();
			}
		}

        // helper functions

        private void  Subdivide(float detail )
        {

            var a = new Vector3();
            var b = new Vector3();
            var c = new Vector3();

            // iterate over all faces and apply a subdivison with the given detail value

            for (var i = 0; i < indices.Count; i += 3)
            {

                // get the vertices of the face

                GetVertexByIndex(indices[i + 0], a);
                GetVertexByIndex(indices[i + 1], b);
                GetVertexByIndex(indices[i + 2], c);

                // perform subdivision

                SubdivideFace(a, b, c, detail);

            }

        }
		private void SubdivideFace(Vector3 a, Vector3 b, Vector3 c, float detail )
		{

			float cols = (float)System.Math.Pow(2, detail);

			// we use this multidimensional array as a data structure for creating the subdivision

			var v = new List<List<Vector3>>();

			int i, j;

			// construct all of the vertices for this subdivision

			for (i = 0; i <= cols; i++)
			{

				v.Add(new List<Vector3>());

				var aj = (a.Clone() as Vector3).Lerp(c, i / cols);
				var bj = (b.Clone() as Vector3).Lerp(c, i / cols);

				float rows = cols - i;

				for (j = 0; j <= rows; j++)
				{

					if (j == 0 && i == cols)
					{

						v[i].Add(aj);

					}
					else
					{

						v[i].Add((aj.Clone() as Vector3).Lerp(bj, j / rows));

					}

				}

			}

			// construct all of the faces

			for (i = 0; i < cols; i++)
			{

				for (j = 0; j < 2 * (cols - i) - 1; j++)
				{

					int k = (int)System.Math.Floor((decimal)(j / 2));

					if (j % 2 == 0)
					{

						PushVertex(v[i][k + 1]);
						PushVertex(v[i + 1][k]);
						PushVertex(v[i][k]);

					}
					else
					{

						PushVertex(v[i][k + 1]);
						PushVertex(v[i + 1][k + 1]);
						PushVertex(v[i + 1][k]);

					}

				}

			}

		}

		private void ApplyRadius(float radius )
		{

			var vertex = new Vector3();

			// iterate over the entire buffer and apply the radius to each vertex

			for (var i = 0; i < vertexBuffer.Count; i += 3)
			{

				vertex.X = vertexBuffer[i + 0];
				vertex.Y = vertexBuffer[i + 1];
				vertex.Z = vertexBuffer[i + 2];

				vertex.Normalize().MultiplyScalar(radius);

				vertexBuffer[i + 0] = vertex.X;
				vertexBuffer[i + 1] = vertex.Y;
				vertexBuffer[i + 2] = vertex.Z;

			}

		}

		private void GenerateUVs()
		{

			var vertex = new Vector3();

			for (var i = 0; i < vertexBuffer.Count; i += 3)
			{

				vertex.X = vertexBuffer[i + 0];
				vertex.Y = vertexBuffer[i + 1];
				vertex.Z = vertexBuffer[i + 2];

				float u = Azimuth(vertex) / 2 / (float)System.Math.PI + 0.5f;
				float v = Inclination(vertex) / (float)System.Math.PI + 0.5f;
				uvBuffer.Add(u);
				uvBuffer.Add(1 - v);

			}

			CorrectUVs();

			CorrectSeam();

		}

		private void  CorrectSeam()
		{

			// handle case when face straddles the seam, see #3269

			for (var i = 0; i < uvBuffer.Count; i += 6)
			{

				// uv data of a single face

				var x0 = uvBuffer[i + 0];
				var x1 = uvBuffer[i + 2];
				var x2 = uvBuffer[i + 4];

				var max = System.Math.Max(x0,System.Math.Max(x1, x2));
				var min = System.Math.Min(x0,System.Math.Min(x1, x2));

				// 0.9 is somewhat arbitrary

				if (max > 0.9 && min < 0.1)
				{

					if (x0 < 0.2) uvBuffer[i + 0] += 1;
					if (x1 < 0.2) uvBuffer[i + 2] += 1;
					if (x2 < 0.2) uvBuffer[i + 4] += 1;

				}

			}

		}

		private void PushVertex(Vector3 vertex )
		{

			vertexBuffer.Add(vertex.X);
			vertexBuffer.Add(vertex.Y);
			vertexBuffer.Add(vertex.Z);

		}

		private void GetVertexByIndex(int index, Vector3 vertex )
		{

			var stride = index * 3;

			vertex.X = vertices[stride + 0];
			vertex.Y = vertices[stride + 1];
			vertex.Z = vertices[stride + 2];

		}

		private void CorrectUVs()
		{

			var a = new Vector3();
			var b = new Vector3();
			var c = new Vector3();

			var centroid = new Vector3();

			var uvA = new Vector2();
			var uvB = new Vector2();
			var uvC = new Vector2();

			for (int i = 0, j = 0; i < vertexBuffer.Count; i += 9, j += 6)
			{

				a.Set(vertexBuffer[i + 0], vertexBuffer[i + 1], vertexBuffer[i + 2]);
				b.Set(vertexBuffer[i + 3], vertexBuffer[i + 4], vertexBuffer[i + 5]);
				c.Set(vertexBuffer[i + 6], vertexBuffer[i + 7], vertexBuffer[i + 8]);

				uvA.Set(uvBuffer[j + 0], uvBuffer[j + 1]);
				uvB.Set(uvBuffer[j + 2], uvBuffer[j + 3]);
				uvC.Set(uvBuffer[j + 4], uvBuffer[j + 5]);

				centroid.Copy(a).Add(b).Add(c).DivideScalar(3);

				var azi = Azimuth(centroid);

				CorrectUV(uvA, j + 0, a, azi);
				CorrectUV(uvB, j + 2, b, azi);
				CorrectUV(uvC, j + 4, c, azi);

			}

		}

		private void CorrectUV(Vector2 uv, int stride, Vector3 vector, float azimuth )
		{

			if ((azimuth < 0) && (uv.X == 1))
			{

				uvBuffer[stride] = uv.X - 1;

			}

			if ((vector.X == 0) && (vector.Z == 0))
			{

				uvBuffer[stride] =(float)(azimuth / 2 / System.Math.PI + 0.5);

			}

		}

		// Angle around the Y axis, counter-clockwise when looking from above.

		private float Azimuth(Vector3 vector)
		{

			return (float)System.Math.Atan2(vector.Z, -vector.X);

		}


		// Angle above the XZ plane.

		private float Inclination(Vector3 vector)
		{

			return (float)System.Math.Atan2(-vector.Y, System.Math.Sqrt((vector.X * vector.X) + (vector.Z * vector.Z)));

		}
	}
}
