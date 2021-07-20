using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Math;

namespace THREE.Geometries
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
        

        public ConvexBufferGeometry() : base() 
        {
        }
        public ConvexBufferGeometry(Vector3[] points) : base() 
        {
            List<float> vertices = new List<float>();
            List<float> normals = new List<float>();
            ConvexHull convexHull = new ConvexHull().SetFromPoints(points);

            var faces = convexHull.faces;

            for(int i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                var edge = face.Edge;

                do
                {
                    var point = edge.Head().Point;
                    vertices.Add(point.X); vertices.Add(point.Y); vertices.Add(point.Z);
                    normals.Add(face.Normal.X); normals.Add(face.Normal.Y); normals.Add(face.Normal.Z);

                    edge = edge.Next;

                } while (edge != face.Edge);
            }

            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
        }
    }
}
