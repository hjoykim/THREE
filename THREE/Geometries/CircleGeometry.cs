using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Math;

namespace THREE.Geometries
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

            List<Vector3> vertices = new List<Vector3>();

            List<Vector3> normals = new List<Vector3>();

            List<Vector2> uvs = new List<Vector2>();

            vertices.Add(new Vector3(0, 0, 0));

            normals.Add(new Vector3(0, 0, 1));

            uvs.Add(new Vector2(0.5f, 0.5f));

            Vector3 vertex = new Vector3();

            Vector2 uv = new Vector2();

            for (int s = 0,i=0; s <= segments; s++,i++)
            {

                var segment = thetaStart + s / segments * thetaLength;

                // vertex

                vertex.X = (float)(radius * System.Math.Cos(segment.Value));
                vertex.Y = (float)(radius * System.Math.Sin(segment.Value));

                vertices.Add((Vector3)vertex.Clone());

                // normal

                normals.Add(new Vector3(0, 0, 1));

                // uvs

                uv.X = (vertices[i].X / radius.Value + 1) / 2.0f;
                uv.Y = (vertices[i+1].X / radius.Value + 1) / 2.0f;

                uvs.Add((Vector2)uv.Clone());

            }

            // indices

            for (int i = 1; i <= segments; i++)
            {
                indices.Add(i, i + 1, 0);
            }

            // build geometry

            this.SetIndex(indices);

            BufferAttribute<float> positions = new BufferAttribute<float>();
            positions.ItemSize = 3;
            positions.Type = typeof(float);            

            this.SetAttribute("position", positions.CopyVector3sArray(vertices.ToArray()));

            BufferAttribute<float> normalAttributes = new BufferAttribute<float>();
            normalAttributes.ItemSize = 3;
            normalAttributes.Type = typeof(float);
            this.SetAttribute("normal", normalAttributes.CopyVector3sArray(normals.ToArray()));

            BufferAttribute<float> uvAttributes = new BufferAttribute<float>();
            uvAttributes.ItemSize = 2;
            uvAttributes.Type = typeof(float);
            this.SetAttribute("uv",uvAttributes.CopyVector2sArray(uvs.ToArray()));

        }
    }
}
