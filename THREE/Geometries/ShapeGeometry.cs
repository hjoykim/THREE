using System;
using System.Collections;
using System.Collections.Generic;
using THREE.Core;
using THREE.Extras;
using THREE.Extras.Core;
using THREE.Math;

namespace THREE.Geometries
{
    public class ShapeGeometry : Geometry
    {
        public Hashtable parameter;

        public ShapeGeometry(List<Shape> shapes, float? curveSegments = null) : base()
        {
            parameter = new Hashtable()
            {
                {"shapes",shapes },
                {"curveSegments",curveSegments }
            };

            this.FromBufferGeometry(new ShapeBufferGeometry(shapes, curveSegments));
            this.MergeVertices();
        }
    }

    public class ShapeBufferGeometry : BufferGeometry
    {
        public Hashtable parameter;

        private List<int> indices = new List<int>();
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();

        private float CurveSegments;

        int groupStart = 0;

        int groupCount = 0;

        public ShapeBufferGeometry(List<Shape> shapes, float? curveSegments = null) : base()
        {
            parameter = new Hashtable()
            {
                {"shapes",shapes },
                {"curveSegments",curveSegments }
            };

            CurveSegments = curveSegments != null ? curveSegments.Value : 12;


            // helper variables



            if (shapes.Count == 1)
            {
                AddShape(shapes[0]);
            }
            else
            {
                for(int i = 0; i < shapes.Count; i++)
                {
                    AddShape(shapes[i]);
                    this.AddGroup(groupStart, groupCount, i);

                    groupStart += groupCount;
                    groupCount = 0;
                }
            }

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
            this.SetAttribute("uv", uvAttributes.CopyVector2sArray(uvs.ToArray()));
        }
        private void AddShape(Shape shape)
        {
            int i, l;

            List<Vector3> shapeHole = null;

            var indexOffset = vertices.Count / 3;
            var points = shape.ExtractPoints(CurveSegments);
                        
            var shapeVertices = (List<Vector3>)points["shape"];
            var shapeHoles = (List < List < Vector3 >> )points["holes"];

            // check direction of vertices

            if (ShapeUtils.IsClockWise(shapeVertices) == false)
            {

                shapeVertices.Reverse();

            

            for (i = 0, l = shapeHoles.Count; i < l; i++)
            {

                shapeHole = shapeHoles[i];

                if (ShapeUtils.IsClockWise(shapeHole) == true)
                {
                    shapeHole.Reverse();
                    shapeHoles[i] = shapeHole;

                }

            }
            }
            var faces = ShapeUtils.TriangulateShape(shapeVertices, shapeHoles);

            // join vertices of inner and outer paths to a single array

            for (i = 0, l = shapeHoles.Count; i < l; i++)
            {

                shapeHole = shapeHoles[i];
                shapeVertices.AddRange(shapeHole);

            }

            // vertices, normals, uvs

            for (i = 0, l = shapeVertices.Count; i < l; i++)
            {

                var vertex = shapeVertices[i];

                vertices.Add(vertex);
                normals.Add(new Vector3(0, 0, 1));
                uvs.Add(new Vector2(vertex.X, vertex.Y)); // world uvs

            }

            // incides

            for (i = 0, l = faces.Count; i < l; i++)
            {

                var face = faces[i];

                var a = face[0] + indexOffset;
                var b = face[1] + indexOffset;
                var c = face[2] + indexOffset;

                indices.Add(a, b, c);
                groupCount += 3;
            }

        }
    }
}
