using System.Collections;
using System.Collections.Generic;

namespace THREE
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
        private List<float> vertices = new List<float>();
        private List<float> normals = new List<float>();
        private List<float> uvs = new List<float>();

        private float CurveSegments;

        int groupStart = 0;

        int groupCount = 0;

        public ShapeBufferGeometry(Shape shape,float? curveSegments = null) : base()
        {
            parameter = new Hashtable()
            {
                {"shapes",shape },
                {"curveSegments",curveSegments }               
            };

            CurveSegments = curveSegments != null ? curveSegments.Value : 12;

            AddShape(shape);

            this.SetIndex(indices);

            BufferAttribute<float> positions = new BufferAttribute<float>(vertices.ToArray(), 3);
         

            this.SetAttribute("position", positions);

            BufferAttribute<float> normalAttributes = new BufferAttribute<float>(normals.ToArray(), 3);
          
            this.SetAttribute("normal", normalAttributes);

            BufferAttribute<float> uvAttributes = new BufferAttribute<float>(uvs.ToArray(), 2);
           
            this.SetAttribute("uv", uvAttributes);

        }
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

            BufferAttribute<float> positions = new BufferAttribute<float>(vertices.ToArray(),3);
            //positions.ItemSize = 3;
            //positions.Type = typeof(float);

            this.SetAttribute("position", positions);

            BufferAttribute<float> normalAttributes = new BufferAttribute<float>(normals.ToArray(),3);
            //normalAttributes.ItemSize = 3;
            //normalAttributes.Type = typeof(float);
            this.SetAttribute("normal", normalAttributes);

            BufferAttribute<float> uvAttributes = new BufferAttribute<float>(uvs.ToArray(),2);
            //uvAttributes.ItemSize = 2;
            //uvAttributes.Type = typeof(float);
            this.SetAttribute("uv", uvAttributes);
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
                shapeVertices = shapeVertices.Concat(shapeHole);

            }

            // vertices, normals, uvs

            for (i = 0, l = shapeVertices.Count; i < l; i++)
            {

                var vertex = shapeVertices[i];

                vertices.Add(vertex.X,vertex.Y,vertex.Z);
                normals.Add(0,0,1);
                uvs.Add(vertex.X, vertex.Y); // world uvs

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
