
using MIConvexHull;
using SkiaSharp;
using System;
using System.Collections;

namespace THREE
{
    [Serializable]
    public class WireframeGeometry : BufferGeometry 
    {
        public WireframeGeometry(BufferGeometry geometry) : base()
        {
            // buffer

            List<float> vertices = new List<float>();

            // helper variables

            int[] edge = new int[] { 0, 0 };
            Dictionary<string,Hashtable> edges = new Dictionary<string,Hashtable>();

            var vertex = new Vector3();

            if (geometry.Index != null)
            {

                // indexed BufferGeometry

                var position = geometry.Attributes["position"] as BufferAttribute<float>;
                var indices = geometry.Index;
                var groups = geometry.Groups;

                if (groups.Count == 0)
                {
                    groups.Add(new DrawRange { Start= 0, Count= indices.count, MaterialIndex = 0 }) ;
                }

                // create a data structure that contains all eges without duplicates

                for (int o = 0, ol = groups.Count; o < ol; ++o)
                {

                    var group = groups[o];

                    var start = (int)group.Start;
                    var count = (int)group.Count;

                    for (int i = start, l = (start + count); i < l; i += 3)
                    {

                        for (int j = 0; j < 3; j++)
                        {

                            var edge1 = indices.GetX(i + j);
                            var edge2 = indices.GetX(i + (j + 1) % 3);
                            edge[0] = Math.Min(edge1, edge2); // sorting prevents duplicates
                            edge[1] = Math.Max(edge1, edge2);

                            string key = edge[0] + "," + edge[1];

                            if (!edges.ContainsKey(key))
                            {
                                edges[key] = new Hashtable{ { "index1", edge[0] },{ "index2", edge[1] } };
                            }
                        }
                    }
                }

                // generate vertices

                foreach (var key in edges ) {
                    var e = key.Value;
                    vertex.FromBufferAttribute(position, (int)e["index1"]);
                    vertices.Add(vertex.X, vertex.Y, vertex.Z);
                    vertex.FromBufferAttribute(position, (int)e["index2"]);
                    vertices.Add(vertex.X, vertex.Y, vertex.Z);
                }

            }
            else
            {

                // non-indexed BufferGeometry

                var position = geometry.Attributes["position"] as BufferAttribute<float>;

                for (int i = 0, l = (position.count / 3); i < l; i++)
                {

                    for (int j = 0; j < 3; j++)
                    {

                        // three edges per triangle, an edge is represented as (index1, index2)
                        // e.g. the first triangle has the following edges: (0,1),(1,2),(2,0)

                        int index1 = 3 * i + j;
                        vertex.FromBufferAttribute(position, index1);
                        vertices.Add(vertex.X, vertex.Y, vertex.Z);

                        int index2 = 3 * i + ((j + 1) % 3);
                        vertex.FromBufferAttribute(position, index2);
                        vertices.Add(vertex.X, vertex.Y, vertex.Z);
                    }
                }

            }

            // build geometry

            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));


        }
    }

}
