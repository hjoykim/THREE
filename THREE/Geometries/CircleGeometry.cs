using System.Collections;
using System.Collections.Generic;


namespace THREE
{
    public class CircleGeometry : Geometry
    {
        public Hashtable parameters;
        public CircleGeometry(float? radius=null,float? segments=null,float? thetaStart=null,float? thetaLength=null) : base()
        {
            this.type = "CircleGeometry";

            this.parameters = new Hashtable()
            {
                {"radius",radius },
                {"segments",segments },
                {"thetaStart",thetaStart },
                {"thetaLength",thetaLength }
            };

            this.FromBufferGeometry(new CircleBufferGeometry(radius, segments, thetaStart, thetaLength));

            this.MergeVertices();
        }
    }

    public class CircleBufferGeometry : BufferGeometry
    {
        public Hashtable parameters;

        public CircleBufferGeometry(float? radius = null, float? segments = null, float? thetaStart = null, float? thetaLength = null) : base()
        {
            this.type = "CircleBufferGeometry";

            this.parameters = new Hashtable()
            {
                {"radius",radius },
                {"segments",segments },
                {"thetaStart",thetaStart },
                {"thetaLength",thetaLength }
            };

            if (radius == null) radius = 1;

            segments = segments != null ? (float)System.Math.Max(3, segments.Value) : 8;

            thetaStart = thetaStart != null ? thetaStart : 0;
            thetaLength = thetaLength != null ? thetaLength : (float)System.Math.PI * 2;

            List<int> indices = new List<int>();

            List<float> vertices = new List<float>();

            List<float> normals = new List<float>();

            List<float> uvs = new List<float>();

            vertices.Add(0,0,0);

            normals.Add(0,0,1);

            uvs.Add(0.5f, 0.5f);

            Vector3 vertex = new Vector3();

            Vector2 uv = new Vector2();

            for (int s = 0,i=3; s <= segments; s++,i+=3)
            {

                var segment = thetaStart + s / segments * thetaLength;

                // vertex

                vertex.X = (float)(radius * System.Math.Cos(segment.Value));
                vertex.Y = (float)(radius * System.Math.Sin(segment.Value));

                vertices.Add(vertex.X,vertex.Y,vertex.Z);

                // normal

                normals.Add(0, 0, 1);

                // uvs

                uv.X = (vertices[i] / radius.Value + 1) / 2.0f;
                uv.Y = (vertices[i+1] / radius.Value + 1) / 2.0f;

                uvs.Add(uv.X, uv.Y);

            }

            // indices

            for (int i = 1; i <= segments; i++)
            {
                indices.Add(i, i + 1, 0);
            }

            // build geometry

            this.SetIndex(indices);              

            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(),3));


            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(),3));


            this.SetAttribute("uv",new BufferAttribute<float>(uvs.ToArray(),2));

        }
    }
}
