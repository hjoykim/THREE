using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace THREE
{
    public interface UVGenerator
    {
        List<Vector2> GenerateTopUV(Geometry geometry, List<float> vertices, int indexA, int indexB, int indexC);
        List<Vector2> GenerateSideWallUV(Geometry geometry, List<float> vertices, int indexA, int indexB, int indexC, int indexD);
    }
    public class WorldUVGenerator : UVGenerator
    {
        public List<Vector2> GenerateTopUV(Geometry geometry, List<float> vertices, int indexA, int indexB, int indexC )
        {

            var a_x = vertices[indexA * 3];
            var a_y = vertices[indexA * 3 + 1];
            var b_x = vertices[indexB * 3];
            var b_y = vertices[indexB * 3 + 1];
            var c_x = vertices[indexC * 3];
            var c_y = vertices[indexC * 3 + 1];

            return new List<Vector2>(){
                new Vector2(a_x, a_y),
                new Vector2(b_x, b_y),
                new Vector2(c_x, c_y)
            };

        }

	    public List<Vector2> GenerateSideWallUV(Geometry geometry, List<float> vertices, int indexA, int indexB, int indexC, int indexD )
        {

            var a_x = vertices[indexA * 3];
            var a_y = vertices[indexA * 3 + 1];
            var a_z = vertices[indexA * 3 + 2];
            var b_x = vertices[indexB * 3];
            var b_y = vertices[indexB * 3 + 1];
            var b_z = vertices[indexB * 3 + 2];
            var c_x = vertices[indexC * 3];
            var c_y = vertices[indexC * 3 + 1];
            var c_z = vertices[indexC * 3 + 2];
            var d_x = vertices[indexD * 3];
            var d_y = vertices[indexD * 3 + 1];
            var d_z = vertices[indexD * 3 + 2];

            if (System.Math.Abs(a_y - b_y) < 0.01)
            {

                return new List<Vector2>(){
                    new Vector2(a_x, 1 - a_z),
                    new Vector2(b_x, 1 - b_z),
                    new Vector2(c_x, 1 - c_z),
                    new Vector2(d_x, 1 - d_z)
                };

            }
            else
            {

                return new List<Vector2>(){
                    new Vector2(a_y, 1 - a_z),
                    new Vector2(b_y, 1 - b_z),
                    new Vector2(c_y, 1 - c_z),
                    new Vector2(d_y, 1 - d_z)
                };

            }

        }
    }
    public class ExtrudeGeometry : Geometry
    {
        public Hashtable parameters;

        public ExtrudeGeometry(List<Shape> shapes,Hashtable options)
        {
            this.type = "ExtrudeGeometry";

            parameters = new Hashtable()
            {
                {"shapes",shapes },
                {"options",options }
            };

            this.FromBufferGeometry(new ExtrudeBufferGeometry(shapes, options));

            this.MergeVertices();
        }
        public ExtrudeGeometry(Shape shape,Hashtable options) : this(new List<Shape>() { shape }, options)
        {

        }
    }

    public class ExtrudeBufferGeometry : BufferGeometry
    {
        public Hashtable parameters;

        private Hashtable options;

        private List<float> verticesArray = new List<float>();

        private List<float> uvArray = new List<float>();

        private List<float> placeholder = new List<float>();

        private int? curveSegments;

        private int? steps;

        private int? depth;

        private bool? bevelEnabled;

        private int? bevelThickness;

        private float? bevelSize;

        private int? bevelOffset;

        private int? bevelSegments;

        private Curve extrudePath;

        private UVGenerator uvgen;

        private List<Vector3> contour=new List<Vector3>();

        List<List<int>> faces = null;

        int vlen, flen;

        private List<List<Vector3>> holes;


        public ExtrudeBufferGeometry() : base()
        {

        }
        public ExtrudeBufferGeometry(Shape shape, Hashtable options) : this(new List<Shape>() { shape }, options)
        {
        }
        
        public ExtrudeBufferGeometry(List<Shape> shapes,Hashtable options)
        {
            this.type = "ExtrudeBufferGeometry";

            Init(shapes, options);
           
        }

       public void Init(List<Shape> shapes,Hashtable options)
        {
            parameters = new Hashtable()
            {
                {"shapes",shapes },
                {"options",options }
            };

            this.options = options;

            for (int i = 0; i < shapes.Count; i++)
            {
                var shape = shapes[i];
                AddShape(shape);
            }

            this.SetAttribute("position", new BufferAttribute<float>(verticesArray.ToArray(), 3));

            this.SetAttribute("uv", new BufferAttribute<float>(uvArray.ToArray(), 2));

            this.ComputeVertexNormals();
        }

        private void AddShape(Shape shape)
        {
            placeholder.Clear();

            if (options != null) {

                curveSegments = options.ContainsKey("curveSegments") ? (int)options["curveSegments"] : 12;

                steps = options.ContainsKey("steps") ? (int)options["steps"] : 1;

                depth = options.ContainsKey("depth") ? (int)options["depth"] : 100;

                bevelEnabled = options.ContainsKey("bevelEnabled") ? (bool)options["bevelEnabled"] : true;

                bevelThickness = options.ContainsKey("bevelThickness") ? (int)options["bevelThickness"] : 6;

                bevelSize = options.ContainsKey("bevelSize") ? (float)options["bevelSize"] : (float)(bevelThickness - 2);

                bevelOffset = options.ContainsKey("bevelOffset") ? (int)options["bevelOffset"] : 0;

                bevelSegments = options.ContainsKey("bevelSegments") ? (int)options["bevelSegments"] : 3;

                extrudePath = options.ContainsKey("extrudePath") ? (Curve)options["extrudePath"] : null;

                uvgen = options.ContainsKey("UVGenerator") ? (UVGenerator)options["UVGenerator"] : new WorldUVGenerator();

                // deprecated options

                if (options.ContainsKey("amount"))
                {
                    Debug.WriteLine("THREE.Geometries.ExtrudeBufferGeometry: amount has been renamed to depth.");
                    depth = (int)options["amount"];
                }
            }

            bool extrudeByPath = false;
            
            List<Vector3> extrudePts = null;

            Hashtable splineTube = null;

            Vector3 binormal = new Vector3();
            Vector3 normal = new Vector3();
            Vector3 position2 = new Vector3();

            if (extrudePath!=null)
            {

                extrudePts = extrudePath.GetSpacedPoints(steps);

                extrudeByPath = true;
            
                bevelEnabled = false; // bevels not supported for path extrusion

                // SETUP TNB variables

                // TODO1 - have a .isClosed in spline?

                splineTube = extrudePath.ComputeFrenetFrames((int)steps, false);

                // console.log(splineTube, 'splineTube', splineTube.normals.length, 'steps', steps, 'extrudePts', extrudePts.length);

                binormal = new Vector3();
                normal = new Vector3();
                position2 = new Vector3();

            }
            if (!bevelEnabled.Value)
            {
                bevelSegments = 0;
                bevelThickness = 0;
                bevelSize = 0;
                bevelOffset = 0;
            }

            // Variables initialization

          

            Hashtable shapePoints = shape.ExtractPoints(curveSegments.Value);

            var vertices = (List<Vector3>)shapePoints["shape"];
            this.holes = (List<List<Vector3>>)shapePoints["holes"];

            var reverse = !ShapeUtils.IsClockWise(vertices);

            if (reverse)
            {

                vertices.Reverse();

                // Maybe we should also check if holes are in the opposite direction, just to be safe ...

                for (int h = 0, hl = holes.Count; h < hl; h++)
                {

                    List<Vector3> ahole = holes[h];

                    if (ShapeUtils.IsClockWise(ahole))
                    {
                        ahole.Reverse();
                        holes[h] = ahole;
                    }
                }
            }

            faces = ShapeUtils.TriangulateShape(vertices, holes);

            this.contour.Clear();
            this.contour.Concat(vertices); // vertices has all points but contour has only points of circumference

          

            for (int h = 0, hl = holes.Count; h < hl; h++)
            {

                List<Vector3> ahole = holes[h];

                vertices = vertices.Concat(ahole).ToList();

            }

            vlen = vertices.Count;

            flen = faces.Count;

            float t, bs, z;

            List<Vector2> contourMovements = new List<Vector2>();

            for (int i = 0, il = contour.Count, j = il - 1, k = i + 1; i < il; i++, j++, k++)
            {

                if (j == il) j = 0;
                if (k == il) k = 0;

                //  (j)---(i)---(k)
                // console.log('i,j,k', i, j , k)

                contourMovements.Add(GetBevelVec(contour[i], contour[j], contour[k]));

            }

            var holesMovements = new List<List<Vector2>>();

            

            List<Vector2> verticesMovements = contourMovements;

            for (int h = 0, hl = holes.Count; h < hl; h++)
            {

                List<Vector3> ahole = holes[h];

                var oneHoleMovements = new List<Vector2>();

                for (int i = 0, il = ahole.Count, j = il - 1, k = i + 1; i < il; i++, j++, k++)
                {

                    if (j == il) j = 0;
                    if (k == il) k = 0;

                    //  (j)---(i)---(k)
                    oneHoleMovements.Add(GetBevelVec(ahole[i], ahole[j], ahole[k]));

                }

                holesMovements.Add(oneHoleMovements);
                verticesMovements = verticesMovements.Concat(oneHoleMovements).ToList();

            }


            // Loop bevelSegments, 1 for the front, 1 for the back

            for (int b = 0; b < bevelSegments; b++)
            {

                //for ( b = bevelSegments; b > 0; b -- ) {

                t = b / bevelSegments.Value;
                z = bevelThickness.Value * (float)System.Math.Cos(t * System.Math.PI / 2);
                bs = bevelSize.Value * (float)System.Math.Sin(t * System.Math.PI / 2) + bevelOffset.Value;

                // contract shape

                for (int i = 0, il = contour.Count; i < il; i++)
                {

                    Vector2 vert = ScalePt2(contour[i], contourMovements[i], bs);

                    V(vert.X, vert.Y, -z);

                }

                // expand holes

                for (int h = 0, hl = holes.Count; h < hl; h++)
                {

                    List<Vector3> ahole = holes[h];
                    List<Vector2> oneHoleMovements = holesMovements[h];

                    for (int i = 0, il = ahole.Count; i < il; i++)
                    {

                        Vector2 vert = ScalePt2(ahole[i], oneHoleMovements[i], bs);

                        V(vert.X, vert.Y, -z);

                    }

                }

            }

            bs = bevelSize.Value + bevelOffset.Value;

            // Back facing vertices

            for (int i = 0; i < vlen; i++)
            {

                Vector2 vert = bevelEnabled.Value ? ScalePt2(vertices[i], verticesMovements[i], bs) : vertices[i].ToVector2();

                if (!extrudeByPath)
                {

                    V(vert.X, vert.Y, 0);

                }
                else
                {

                    // v( vert.x, vert.y + extrudePts[ 0 ].y, extrudePts[ 0 ].x );

                    normal.Copy((splineTube["normals"] as List<Vector3>)[0]).MultiplyScalar(vert.X);
                    binormal.Copy((splineTube["binormals"] as List<Vector3>)[0]).MultiplyScalar(vert.Y);

                    position2.Copy(extrudePts[0]).Add(normal).Add(binormal);

                    V(position2.X, position2.Y, position2.Z);

                }

            }

            // Add stepped vertices...
            // Including front facing vertices          

            for (int s = 1; s <= steps; s++)
            {

                for (int i = 0; i < vlen; i++)
                {

                    Vector2 vert = bevelEnabled.Value ? ScalePt2(vertices[i], verticesMovements[i], bs) : vertices[i].ToVector2();

                    if (!extrudeByPath)
                    {

                        V(vert.X, vert.Y, depth.Value / (float)steps.Value * s);

                    }
                    else
                    {

                        // v( vert.x, vert.y + extrudePts[ s - 1 ].y, extrudePts[ s - 1 ].x );

                        normal.Copy((splineTube["normals"] as List<Vector3>)[s]).MultiplyScalar(vert.X);
                        binormal.Copy((splineTube["binormals"] as List<Vector3>)[s]).MultiplyScalar(vert.Y);

                        position2.Copy(extrudePts[s]).Add(normal).Add(binormal);

                        V(position2.X, position2.Y, position2.Z);

                    }

                }

            }


            // Add bevel segments planes

            //for ( b = 1; b <= bevelSegments; b ++ ) {
            for (int b = bevelSegments.Value - 1; b >= 0; b--)
            {

                t = b / (float)bevelSegments.Value;
                z = bevelThickness.Value * (float)System.Math.Cos(t * System.Math.PI / 2);
                bs = bevelSize.Value * (float)System.Math.Sin(t * System.Math.PI / 2) + bevelOffset.Value;

                // contract shape

                for (int i = 0, il = contour.Count; i < il; i++)
                {

                    Vector2 vert = ScalePt2(contour[i], contourMovements[i], bs);
                    V(vert.X, vert.Y, depth.Value + z);

                }

                // expand holes

                for (int h = 0, hl = holes.Count; h < hl; h++)
                {

                    List<Vector3> ahole = holes[h];

                    List<Vector2> oneHoleMovements = holesMovements[h];

                    for (int i = 0, il = ahole.Count; i < il; i++)
                    {

                        Vector2 vert = ScalePt2(ahole[i], oneHoleMovements[i], bs);

                        if (!extrudeByPath)
                        {

                            V(vert.X, vert.Y, depth.Value + z);

                        }
                        else
                        {

                            V(vert.X, vert.Y + extrudePts[(int)steps - 1].Y, extrudePts[(int)steps - 1].X + z);

                        }

                    }

                }
            }

            /* Faces */

            // Top and bottom faces

            BuildLidFaces();

            // Sides faces

            BuildSideFaces();
        }

        private void BuildLidFaces()
        {

            var start = verticesArray.Count / 3;

            if (bevelEnabled.Value)
            {

                int layer = 0; // steps + 1
                var offset = vlen * layer;

                // Bottom faces

                for (int i  = 0; i < flen; i++)
                {

                    var face = faces[i];
                    F3(face[2] + offset, face[1] + offset, face[0] + offset);

                }

                layer = (int)steps + (int)bevelSegments * 2;
                offset = vlen * layer;

                // Top faces

                for (int i = 0; i < flen; i++)
                {

                    var face = faces[i];
                    F3(face[0] + offset, face[1] + offset, face[2] + offset);

                }

            }
            else
            {

                // Bottom faces

                for (int i = 0; i < flen; i++)
                {

                    var face = faces[i];
                    F3(face[2], face[1], face[0]);

                }

                // Top faces

                for (int i = 0; i < flen; i++)
                {

                    var face = faces[i];
                    F3(face[0] + vlen * (int)steps, face[1] + vlen * (int)steps, face[2] + vlen * (int)steps);

                }

            }

            this.AddGroup(start, verticesArray.Count / 3 - start, 0);

        }

        // Create faces for the z-sides of the shape

        private void BuildSideFaces()
        {

            var start = verticesArray.Count / 3;
            var layeroffset = 0;
            Sidewalls(this.contour, layeroffset);
            layeroffset += contour.Count;

            for (int h = 0, hl = holes.Count; h < hl; h++)
            {

                List<Vector3> ahole = holes[h];
                Sidewalls(ahole, layeroffset);

                //, true
                layeroffset += ahole.Count;

            }


            this.AddGroup(start, verticesArray.Count / 3 - start, 1);


        }

        private Vector2 GetBevelVec(Vector3 inPt, Vector3 inPrev, Vector3 inNext )
        {

            // computes for inPt the corresponding point inPt' on a new contour
            //   shifted by 1 unit (length of normalized vector) to the left
            // if we walk along contour clockwise, this new contour is outside the old one
            //
            // inPt' is the intersection of the two lines parallel to the two
            //  adjacent edges of inPt at a distance of 1 unit on the left side.

            float v_trans_x, v_trans_y, shrink_by; // resulting translation vector for inPt

            // good reading for geometry algorithms (here: line-line intersection)
            // http://geomalgorithms.com/a05-_intersect-1.html

            float v_prev_x = inPt.X - inPrev.X,
                v_prev_y = inPt.Y - inPrev.Y,
                v_next_x = inNext.X - inPt.X,
                v_next_y = inNext.Y - inPt.Y;

            var v_prev_lensq = (v_prev_x * v_prev_x + v_prev_y * v_prev_y);

            // check for collinear edges
            var collinear0 = (v_prev_x * v_next_y - v_prev_y * v_next_x);

            if (System.Math.Abs(collinear0) > float.Epsilon)
            {

                // not collinear

                // length of vectors for normalizing

                var v_prev_len = System.Math.Sqrt(v_prev_lensq);
                var v_next_len = System.Math.Sqrt(v_next_x * v_next_x + v_next_y * v_next_y);

                // shift adjacent points by unit vectors to the left

                var ptPrevShift_x = (inPrev.X - v_prev_y / v_prev_len);
                var ptPrevShift_y = (inPrev.Y + v_prev_x / v_prev_len);

                var ptNextShift_x = (inNext.X - v_next_y / v_next_len);
                var ptNextShift_y = (inNext.Y + v_next_x / v_next_len);

                // scaling factor for v_prev to intersection point

                var sf = ((ptNextShift_x - ptPrevShift_x) * v_next_y -
                        (ptNextShift_y - ptPrevShift_y) * v_next_x) /
                    (v_prev_x * v_next_y - v_prev_y * v_next_x);

                // vector from inPt to intersection point

                v_trans_x = (float)(ptPrevShift_x + v_prev_x * sf - inPt.X);
                v_trans_y = (float)(ptPrevShift_y + v_prev_y * sf - inPt.Y);

                // Don't normalize!, otherwise sharp corners become ugly
                //  but prevent crazy spikes
                var v_trans_lensq = (v_trans_x * v_trans_x + v_trans_y * v_trans_y);
                if (v_trans_lensq <= 2)
                {

                    return new Vector2(v_trans_x, v_trans_y);

                }
                else
                {

                    shrink_by = (float)System.Math.Sqrt(v_trans_lensq / 2);

                }

            }
            else
            {

                // handle special case of collinear edges

                var direction_eq = false; // assumes: opposite
                if (v_prev_x > float.Epsilon)
                {

                    if (v_next_x > float.Epsilon)
                    {

                        direction_eq = true;

                    }

                }
                else
                {

                    if (v_prev_x < -float.Epsilon)
                    {

                        if (v_next_x < -float.Epsilon)
                        {

                            direction_eq = true;

                        }

                    }
                    else
                    {

                        if (System.Math.Sign(v_prev_y) == System.Math.Sign(v_next_y))
                        {

                            direction_eq = true;

                        }

                    }

                }

                if (direction_eq)
                {

                    // console.log("Warning: lines are a straight sequence");
                    v_trans_x = -v_prev_y;
                    v_trans_y = v_prev_x;
                    shrink_by = (float)System.Math.Sqrt(v_prev_lensq);

                }
                else
                {

                    // console.log("Warning: lines are a straight spike");
                    v_trans_x = v_prev_x;
                    v_trans_y = v_prev_y;
                    shrink_by = (float)System.Math.Sqrt(v_prev_lensq / 2);

                }

            }

            return new Vector2(v_trans_x / shrink_by, v_trans_y / shrink_by);

        }
        private Vector2 ScalePt2(Vector3 pt, Vector2 vec, float size )
        {

            return (vec.Clone() as Vector2).MultiplyScalar(size).Add(pt.ToVector2());

        }

        private void Sidewalls(List<Vector3> contour, int layeroffset )
        {

            int j, k;
            int i = contour.Count;

            while (--i >= 0)
            {

                j = i;
                k = i - 1;
                if (k < 0) k = contour.Count - 1;

                //console.log('b', i,j, i-1, k,vertices.length);

                int s = 0,
                    sl = (int)steps + (int)bevelSegments * 2;

                for (s = 0; s < sl; s++)
                {

                    var slen1 = vlen * s;
                    var slen2 = vlen * (s + 1);

                    int a = layeroffset + j + slen1,
                        b = layeroffset + k + slen1,
                        c = layeroffset + k + slen2,
                        d = layeroffset + j + slen2;

                    F4(a, b, c, d);

                }

            }

        }

        private void V(float x,float y,float z)
        {
            placeholder.Add(x, y, z);
        }
        private void F3(int a, int b, int c )
        {

            AddVertex(a);
            AddVertex(b);
            AddVertex(c);

            var nextIndex = verticesArray.Count / 3;
            var uvs = uvgen.GenerateTopUV(this,verticesArray, nextIndex - 3, nextIndex - 2, nextIndex - 1);

            AddUV(uvs[0]);
            AddUV(uvs[1]);
            AddUV(uvs[2]);

        }

        private void F4(int a, int b, int c, int d )
        {

            AddVertex(a);
            AddVertex(b);
            AddVertex(d);

            AddVertex(b);
            AddVertex(c);
            AddVertex(d);


            var nextIndex = verticesArray.Count / 3;
            var uvs = uvgen.GenerateSideWallUV(this, verticesArray, nextIndex - 6, nextIndex - 3, nextIndex - 2, nextIndex - 1);

            AddUV(uvs[0]);
            AddUV(uvs[1]);
            AddUV(uvs[3]);

            AddUV(uvs[1]);
            AddUV(uvs[2]);
            AddUV(uvs[3]);

        }

        private void AddVertex(int index )
        {

            verticesArray.Add(placeholder[index * 3 + 0]);
            verticesArray.Add(placeholder[index * 3 + 1]);
            verticesArray.Add(placeholder[index * 3 + 2]);

        }


        private void AddUV(Vector2 vector2 )
        {

            uvArray.Add(vector2.X);
            uvArray.Add(vector2.Y);

        }
    }
}
