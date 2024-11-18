using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static THREE.ConvexHull;
using THREE.Objects;
using System.Reflection;

namespace THREE.Objects
{
    [Serializable]
    struct IEdge
    {
        public int a;
        public int b;
        public Vector3 n0;
        public Vector3 n1;
    }
    [Serializable]
    struct IEdgeArrays
    {
        public List<float> vArray;
        public List<float> n0Array;
        public List<float> n1Array;
        public List<float> otherVertArray;
    }
    [Serializable]
    public class OutlineMesh : LineSegments
    {
        private Vector3 NULL_VECTOR = new Vector3();
        private float WELD_FACTOR = 1000;

        public OutlineMesh(Mesh mesh,OutlineMaterial material) : base(new BufferGeometry(), material)
        {
            ExtractGeometry(mesh.Geometry as BufferGeometry);
        }
        private void ExtractGeometry(BufferGeometry geometry)
        {
            IEdgeArrays edge = geometry.Index != null ? ExtractIndexed(geometry) : ExtractSoup(geometry);
            var g = this.Geometry as BufferGeometry;
            g.SetAttribute("position", new BufferAttribute<float>(edge.vArray.ToArray(), 3));
            g.SetAttribute("aN0", new BufferAttribute<float>(edge.n0Array.ToArray(), 3));
            g.SetAttribute("aN1", new BufferAttribute<float>(edge.n1Array.ToArray(), 3));
            g.SetAttribute("aOtherVert", new BufferAttribute<float>(edge.otherVertArray.ToArray(), 4));
        }
        private IEdgeArrays ExtractSoup(BufferGeometry geometry)
        {

            List<float> vArray = new List<float>();
            List<float> n0Array = new List<float>();
            List<float> n1Array = new List<float>();
            List<float> otherVertArray = new List<float>();
            // const weldedVertices = new List<float>();

            return new IEdgeArrays { vArray = vArray, n0Array = n0Array, n1Array = n1Array, otherVertArray = otherVertArray };
        }
        private List<IEdge> ExtractEdgesFromIndex(int[] indexBuffer,float[] positionBuffer)
        {
            var faceNormals = new List<Vector3>();
            var av = new Vector3();
            var bv = new Vector3();
            var cv = new Vector3();

            for (int t = 0; t < indexBuffer.Length;) {
                var normal = new Vector3();
                av.FromArray(positionBuffer, indexBuffer[t++] * 3);
                bv.FromArray(positionBuffer, indexBuffer[t++] * 3);
                cv.FromArray(positionBuffer, indexBuffer[t++] * 3);
                normal.CrossVectors(bv.Sub(av), cv.Sub(av));
                faceNormals.Add(normal.Normalize());
            }

            var edgeFaceMap = new Dictionary<int, Dictionary<int, int>>();
            var halfEdges = new List<List<int>>();

            for (int t = 0; t < indexBuffer.Length / 3; t++)
            {
                var offset = t * 3;
                for (int curr = 0; curr < 3; curr++)
                {
                    var next = (curr + 1) % 3;
                    var a = indexBuffer[offset + curr];
                    var b = indexBuffer[offset + next];
                    if (!edgeFaceMap.ContainsKey(a)) edgeFaceMap[a] = new Dictionary<int, int>();
                    edgeFaceMap[a][b] = t;
                    halfEdges.Add(new List<int> { a, b });
                }
            }

            var edges = new List<IEdge>();
            var duplicateMap = new Dictionary<int, Dictionary<int, bool>>();



            halfEdges.ForEach((item) => {
                var a = item[0];
                var b = item[1];
                if (!duplicateMap.ContainsKey(a)) duplicateMap[a] = new Dictionary<int, bool>();
                if (!duplicateMap.ContainsKey(b)) duplicateMap[b] = new Dictionary<int, bool>();

                if (duplicateMap[a].ContainsKey(b) && duplicateMap[a][b]) return;

                
                var f0 = edgeFaceMap[a].ContainsKey(b);
                var f1 = edgeFaceMap[b].ContainsKey(a);
                var isOutline = f0 != false && f1 != false;
                var n0 = isOutline ? faceNormals[edgeFaceMap[a][b]] : NULL_VECTOR;
                var n1 = isOutline ? faceNormals[edgeFaceMap[b][a]] : NULL_VECTOR;

                edges.Add(new IEdge { a = a, b = b, n0 = n0, n1 = n1 });
                duplicateMap[a][b] = true;
                duplicateMap[b][a] = true;
            });

            return edges;
        }
        private (float[], int[]) WeldIndexed(BufferAttribute<int> indexBuffer, BufferAttribute<float> positionBuffer)
        {
            var map = new Dictionary<string, int>();
            var weldedVerticesMap = new Dictionary<int, int>();
            var weldedVertices = new List<float>();
            var weldedIndices = new List<int>();

            for (int v = 0, c = 0; v < positionBuffer.count; v++)
            {
                var v3 = v * 3;
                var xyz =
                        Math.Round(positionBuffer.Array[v3] * WELD_FACTOR) + ":" +
                        Math.Round(positionBuffer.Array[v3 + 1] * WELD_FACTOR) + ":" +
                        Math.Round(positionBuffer.Array[v3 + 2] * WELD_FACTOR);

                var key = xyz;
                if (!map.ContainsKey(key))
                {
                    map[key] = c++;
                    weldedVertices.Add(
                      positionBuffer.Array[v3],
                      positionBuffer.Array[v3 + 1],
                      positionBuffer.Array[v3 + 2]
                    );
                }
                weldedVerticesMap[v] = map[key];
            }
            for (int t = 0; t < indexBuffer.count; t += 3)
                for (int i = 0; i < 3; i++)
                {
                    var source = indexBuffer.Array[t + i];
                    weldedIndices.Add(weldedVerticesMap[source]);
                }

            return ( weldedVertices.ToArray(),weldedIndices.ToArray());
        }
        private IEdgeArrays ExtractIndexed(BufferGeometry geometry)
        {
            (float[] weldedVertices,int[] weldedIndices) = WeldIndexed(geometry.Index, geometry.Attributes["position"] as BufferAttribute<float>);
            var edges = ExtractEdgesFromIndex(weldedIndices, weldedVertices);
            List<float> vArray = new List<float>();
            List<float> n0Array = new List<float>();
            List<float> n1Array = new List<float>();
            List<float> otherVertArray = new List<float>();

            Action<int, Vector3, Vector3> extract = (index, n0, n1) =>
            {
                var _index = index * 3;
                n0Array.AddRange(n0.ToArray());
                n1Array.AddRange(n1.ToArray());
                for (int i = 0; i < 3; i++)
                    vArray.Add(weldedVertices[_index + i]);
            };

            edges.ForEach((edge) =>
            {
                var a = edge.a;
                var b = edge.b;
                var n0 = edge.n0;
                var n1 = edge.n1;

                extract(a, n0, n1);
                extract(b, n0, n1);

                for (int i = 0; i < 3; i++)
                    otherVertArray.Add(weldedVertices[b * 3 + i]);
                otherVertArray.Add(0);

                for (int i = 0; i < 3; i++)
                    otherVertArray.Add(weldedVertices[a * 3 + i]);
                otherVertArray.Add(1);
            });
            return new IEdgeArrays { vArray = vArray, n0Array = n0Array, n1Array = n1Array, otherVertArray = otherVertArray };
        }
    }
}
