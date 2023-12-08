using MIConvexHull;
using System.Collections.Generic;
using System.Linq;

namespace THREE
{
    public class ConvexGeometry :Geometry
    {
        public ConvexGeometry(Vector3[] points) : base() 
        {
            this.FromBufferGeometry(new ConvexBufferGeometry(points));
            this.MergeVertices();
        }
    }

    public class ConvexBufferGeometry : BufferGeometry
    {
        class TVertex : IVertex
        {
            public double[] Position { get; set; }

            public TVertex(double x,double y,double z)
            {
                Position = new double[] { x, y, z };
            }
        }

        class TFace : ConvexFace<TVertex, TFace>
        {

        }

        public ConvexBufferGeometry() : base() 
        {
        }
        public ConvexBufferGeometry(List<Vector3> points) : base()
        {
            List<TVertex> vertices = new List<TVertex>();

            for(int i = 0; i < points.Count; i++)
            {
                vertices.Add(new TVertex(points[i].X, points[i].Y, points[i].Z));
            }
            var convexHull = MIConvexHull.ConvexHull.Create<TVertex, TFace>(vertices);
            var faces = convexHull.Result.Faces.ToList();
            (var positions, var normals) = ConvertThreeVertices(faces);

            this.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));

        }
        public ConvexBufferGeometry(Vector3[] points) : this(points.ToList()) 
        {
           
        }
        private (List<float>,List<float>) ConvertThreeVertices(List<TFace> faces)
        {
            List<float> tvertices = new List<float>();
            List<float> tnormals = new List<float>();

            for(int i = 0; i < faces.Count; i++)
            {
                var face = faces[i];

                tvertices.Add((float)face.Vertices[0].Position[0], (float)face.Vertices[0].Position[1], (float)face.Vertices[0].Position[2]);
                tvertices.Add((float)face.Vertices[1].Position[0], (float)face.Vertices[1].Position[1], (float)face.Vertices[1].Position[2]);
                tvertices.Add((float)face.Vertices[2].Position[0], (float)face.Vertices[2].Position[1], (float)face.Vertices[2].Position[2]);

                tnormals.Add((float)face.Normal[0], (float)face.Normal[1], (float)face.Normal[2]);
                tnormals.Add((float)face.Normal[0], (float)face.Normal[1], (float)face.Normal[2]);
                tnormals.Add((float)face.Normal[0], (float)face.Normal[1], (float)face.Normal[2]);
            }

            return (tvertices, tnormals);
        }
    }
}
