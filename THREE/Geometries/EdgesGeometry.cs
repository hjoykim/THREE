using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class Edge
    {
        public int index0;
        public int index1;
        public Vector3 normal;
    }
    public class EdgesGeometry : BufferGeometry
    {
        Vector3 _v0 = new Vector3();
        Vector3 _v1 = new Vector3();
        Vector3 _normal = new Vector3();
        Triangle _triangle = new Triangle();
        public Hashtable parameters = new Hashtable();

        private Vector3 GetVector3FromTriangleWithVertexKey(int index)
        {
            if (index == 0) return _triangle.a;
            else if (index == 1) return _triangle.b;
            else return _triangle.c;
        }

        public EdgesGeometry(Geometry geometry, float? thresholdAngle = null) : base()
        {
            type = "EdgeGeometry";

            this.parameters.Add("thresholdAngle", thresholdAngle);


            thresholdAngle = (thresholdAngle != null) ? thresholdAngle : 1;

            BufferGeometry bufferGeometry;
            if (!(geometry is BufferGeometry))
            {

                bufferGeometry = new BufferGeometry().FromGeometry(geometry);

            }
            else
            {
                bufferGeometry = geometry as BufferGeometry;
            }

            float precisionPoints = 4;
            float precision = (float)System.Math.Pow(10, precisionPoints);
            float thresholdDot = (float)System.Math.Cos(MathUtils.DEG2RAD * thresholdAngle.Value);

            var indexAttr = bufferGeometry.GetIndex();
            var positionAttr = bufferGeometry.Attributes["position"] as BufferAttribute<float>;
            var indexCount = indexAttr!=null ? indexAttr.count : positionAttr.count;

            var indexArr = new int[3] { 0, 0, 0};
            var hashes = new string[3];

            var edgeData = new Dictionary<string, Edge>();
            var vertices = new List<float>();

			for (int i = 0; i < indexCount; i += 3)
			{

				if (indexAttr != null)
				{

					indexArr[0] = indexAttr.getX(i);
					indexArr[1] = indexAttr.getX(i + 1);
					indexArr[2] = indexAttr.getX(i + 2);

				}
				else
				{

					indexArr[0] = i;
					indexArr[1] = i + 1;
					indexArr[2] = i + 2;

				}

				Vector3 a, b, c;
				a = _triangle.a;
				b = _triangle.b;
				c = _triangle.c;

				a.FromBufferAttribute(positionAttr, indexArr[0]);
				b.FromBufferAttribute(positionAttr, indexArr[1]);
				c.FromBufferAttribute(positionAttr, indexArr[2]);
				_triangle.GetNormal(_normal);

				// create hashes for the edge from the vertices
				hashes[0] = string.Format("{0},{1},{2}", (int)System.Math.Round(a.X * precision), (int)System.Math.Round(a.Y * precision), (int)System.Math.Round(a.Z * precision));
				hashes[1] = string.Format("{0},{1},{2}", (int)System.Math.Round(b.X * precision), (int)System.Math.Round(b.Y * precision), (int)System.Math.Round(b.Z * precision));
				hashes[2] = string.Format("{0},{1},{2}", (int)System.Math.Round(c.X * precision), (int)System.Math.Round(c.Y * precision), (int)System.Math.Round(c.Z * precision));

				// skip degenerate triangles
				if (hashes[0] == hashes[1] || hashes[1] == hashes[2] || hashes[2] == hashes[0])
				{

					continue;

				}

				// iterate over every edge
				for (int j = 0; j < 3; j++)
				{

					// get the first and next vertex making up the edge
					var jNext = (j + 1) % 3;
					var vecHash0 = hashes[j];
					var vecHash1 = hashes[jNext];
					var v0 = GetVector3FromTriangleWithVertexKey(j);// _triangle[vertKeys[j]];
					var v1 = GetVector3FromTriangleWithVertexKey(jNext);// _triangle[vertKeys[jNext]];

					var hash = string.Format("{0}_{1}", vecHash0, vecHash1);
					var reverseHash = string.Format("{0}_{1}", vecHash1, vecHash0);

					if (edgeData.ContainsKey(reverseHash) && edgeData[reverseHash] != null) {

						// if we found a sibling edge add it into the vertex array if
						// it meets the angle threshold and delete the edge from the map.
						if (_normal.Dot(edgeData[reverseHash].normal) <= thresholdDot)
						{

							vertices.Add(v0.X, v0.Y, v0.Z);
							vertices.Add(v1.X, v1.Y, v1.Z);

						}

						edgeData[reverseHash] = null;

					} else if (!edgeData.ContainsKey(hash))
					{

						// if we've already got an edge here then skip adding a new one
						edgeData[hash] = new Edge()
						{

							index0 = indexArr[j],
							index1 = indexArr[jNext],
							normal = _normal.Clone(),

						};

					}

				}

			}

			// iterate over all remaining, unmatched edges and add them to the vertex array
			foreach (var entry in edgeData) {

				if (edgeData[entry.Key] != null) {

					var index0 = edgeData[entry.Key].index0;
					var index1 = edgeData[entry.Key].index1;
					_v0.FromBufferAttribute(positionAttr, index0);
					_v1.FromBufferAttribute(positionAttr, index1);

					vertices.Add(_v0.X, _v0.Y, _v0.Z);
					vertices.Add(_v1.X, _v1.Y, _v1.Z);

				}

			}

		this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));

        }
    }
}
