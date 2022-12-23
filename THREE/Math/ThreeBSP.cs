using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace THREE
{
    public class ThreeBSP
    {
        public static float EPSILON = 1e-5f;

        public static int COPLANAR = 0;

        public static int FRONT = 1;

        public static int BACK = 2;

        public static int SPANNING = 3;

        private Matrix4d Matrix;

        private Geometry Geometry;

        private Node Tree;

        public static int INDEX = 0;
        public ThreeBSP(object treeIsh = null, Matrix4d matrix = null)
        {
            if (matrix == null)
                this.Matrix = new Matrix4d();
            else
                this.Matrix = matrix;

            this.Tree = this.ToTree(treeIsh);
        }
        private void FaceFunction(Geometry geometry, Face3 face, int i, List<Polygon> polygons)
        {
            var _ref1 = geometry.FaceVertexUvs;

            //var faceVertexUvs, idx, polygon, vIndex, vName, vertex, _j, _len1, _ref1, _ref2;
            var faceVertexUvs = _ref1 != null && _ref1.Count > 0 ? _ref1[0][i] : null;
            if (faceVertexUvs == null)
            {
                faceVertexUvs = new List<Vector2>() { new Vector2(), new Vector2(), new Vector2(), new Vector2() };
            }
            var polygon = new Polygon();

            string[] _ref2 = new string[] { "a", "b", "c", "d" };
            for (int vIndex = 0, j = 0, len1 = _ref2.Length; j < len1; vIndex = ++j)
            {
                int? idx = null;
                switch (_ref2[vIndex])
                {
                    case "a": idx = face.a; break;
                    case "b": idx = face.b; break;
                    case "c": idx = face.c; break;
                }
                if (idx != null)
                {
                    var vertex = geometry.Vertices[idx.Value];
                    Vertex vertex1 = new Vertex(vertex.X, vertex.Y, vertex.Z, face.VertexNormals[0].ToVector3d(), new Vector2d(faceVertexUvs[vIndex].X, faceVertexUvs[vIndex].Y));
                    vertex1.ApplyMatrix4(this.Matrix);
                    polygon.Vertices.Add(vertex1);
                }
            }
            //polygon.CalculateProperties();

            polygons.Add(polygon.CalculateProperties());
        }
        public Node ToTree(object treeIsh = null)
        {
            //var face, geometry, i, polygons, _fn, _i, _len, _ref,


            if (treeIsh is Node)
            {
                return treeIsh as Node;
            }

            List<Polygon> polygons = new List<Polygon>();

            Geometry geometry = null;

            if (treeIsh is Geometry)
                geometry = treeIsh as Geometry;
            else if (treeIsh is Mesh)
            {
                (treeIsh as Mesh).UpdateMatrix();

                this.Matrix = ((treeIsh as Mesh).Matrix.Clone() as Matrix4).ToMatrix4d();

                geometry = (treeIsh as Mesh).Geometry;
            }

           
            
            for (int i = 0; i < geometry.Faces.Count; i++)
            {
                var face = geometry.Faces[i];
                FaceFunction(geometry, face, i, polygons);

            }

            return new Node(polygons);
        }

        public ThreeBSP Subtract(ThreeBSP other)
        {
            var us = this.Tree.Clone();
            var them = other.Tree.Clone();
           

            us.Invert().ClipTo(them);
            them.ClipTo(us).Invert().ClipTo(us).Invert();
            

            return new ThreeBSP(us.Build(them.AllPolygons()).Invert(), this.Matrix);
            
        }

        public ThreeBSP Union(ThreeBSP other)
        {
            var a = this.Tree.Clone() as Node;
            var b = other.Tree.Clone() as Node;

            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());

            return new ThreeBSP(a,this.Matrix);
        }

        public ThreeBSP Intersect(ThreeBSP other)
        {
            var a = this.Tree.Clone() as Node;
            var b = other.Tree.Clone() as Node;

            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            a.Build(b.AllPolygons());
            a.Invert();
            return new ThreeBSP(a,this.Matrix);
        }
        public Geometry ToGeometry()
        {
            var matrix = new Matrix4d().GetInverse(this.Matrix);

            var geometry = new Geometry();

            var polygons = this.Tree.AllPolygons();

            for(int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                List<Vertex> polyVerts = new List<Vertex>();
                for(int j = 0; j < polygons[i].Vertices.Count; j++)
                {
                    var v = polygon.Vertices[j];
                    polyVerts.Add(v.Clone().ApplyMatrix4(matrix));
                }

                for(int idx = 1; idx < polyVerts.Count; idx++)
                {
                    var verts = new List<Vertex>() { polyVerts[0], polyVerts[idx - 1], polyVerts[idx] };
                    var vertUVs = new List<Vector2d>();
                    for(int k = 0; k < verts.Count; k++)
                    {
                        var u = verts[k];
                        vertUVs.Add(new Vector2d(u.UV != null ? u.UV.X : 0, u.UV != null ? u.UV.Y : 0));                       
                    }

                    int[] index = new int[verts.Count];
                    for(int k = 0; k < verts.Count; k++)
                    {
                        geometry.Vertices.Add(verts[k].ToVector3());
                        index[k] = geometry.Vertices.Count - 1;
                    }                   
                    var face = new Face3(index[0],index[1],index[2],polygon.Normal.Clone().ToVector3());
                    
                    geometry.Faces.Add(face);
                    if (geometry.FaceVertexUvs.Count == 0)
                    {
                        geometry.FaceVertexUvs.Add(new List<List<Vector2>>());
                    }
                    List<Vector2> vertUVS2d = new List<Vector2>();
                    vertUVs.ForEach(delegate (Vector2d v) { vertUVS2d.Add(v.ToVector2()); });
                    geometry.FaceVertexUvs[0].Add(vertUVS2d);                   
                }
            }
            return geometry;
            //int i, j;
            //Matrix4 matrix = new Matrix4().GetInverse(this.Matrix);

            //Geometry geometry = new Geometry();
            //var polygons = this.Tree.AllPolygons();
            //var polygon_count = polygons.Count;
            //Polygon polygon;
            //int polygon_vertice_count;
            //Hashtable vertice_dict = new Hashtable();
            //int vertex_idx_a, vertex_idx_b, vertex_idx_c;
            //Vertex vertex;
            //Face3 face;
            //List<Vector2> verticeUvs;

            //for (i = 0; i < polygon_count; i++)
            //{
            //    polygon = polygons[i];
            //    polygon_vertice_count = polygon.Vertices.Count;

            //    for (j = 2; j < polygon_vertice_count; j++)
            //    {
            //        verticeUvs = new List<Vector2>();

            //        vertex = polygon.Vertices[0];
            //        verticeUvs.Add(new Vector2(vertex.UV.X, vertex.UV.Y));
            //        Vector3 vertex1 = new Vector3(vertex.X, vertex.Y, vertex.Z);
            //        vertex1.ApplyMatrix4(matrix);

            //        if(vertice_dict.ContainsKey(vertex1.X + "," + vertex1.Y + "," + vertex.Z))                   
            //        {
            //            vertex_idx_a = (int)vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z];
            //        }
            //        else
            //        {
            //            geometry.Vertices.Add(vertex1);
            //            vertex_idx_a = geometry.Vertices.Count - 1;
            //            vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z] = geometry.Vertices.Count - 1;
            //        }

            //        vertex = polygon.Vertices[j - 1];
            //        verticeUvs.Add(new Vector2(vertex.UV.X, vertex.UV.Y));
            //        vertex1 = new Vector3(vertex.X, vertex.Y, vertex.Z);
            //        vertex1.ApplyMatrix4(matrix);

            //        if (vertice_dict.ContainsKey(vertex1.X + "," + vertex1.Y + "," + vertex.Z))
            //        {
            //            vertex_idx_b= (int)vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z];
            //        }
            //        else
            //        {
            //            geometry.Vertices.Add(vertex1);
            //            vertex_idx_b = geometry.Vertices.Count - 1;
            //            vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z] = geometry.Vertices.Count - 1;
            //        }

            //        vertex = polygon.Vertices[j];
            //        verticeUvs.Add(new Vector2(vertex.UV.X, vertex.UV.Y));
            //        vertex1 = new Vector3(vertex.X, vertex.Y, vertex.Z);
            //        vertex.ApplyMatrix4(matrix);
            //        if (vertice_dict.ContainsKey(vertex1.X + "," + vertex1.Y + "," + vertex.Z))
            //        {
            //            vertex_idx_c = (int)vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z];
            //        }
            //        else
            //        {
            //            geometry.Vertices.Add(vertex1);
            //            vertex_idx_c = geometry.Vertices.Count - 1;
            //            vertice_dict[vertex1.X + "," + vertex1.Y + "," + vertex.Z] = geometry.Vertices.Count - 1;
            //        }

            //        face = new Face3(
            //            vertex_idx_a,
            //            vertex_idx_b,
            //            vertex_idx_c,
            //            new Vector3(polygon.Normal.X, polygon.Normal.Y, polygon.Normal.Z)
            //        );

            //        geometry.Faces.Add(face);
            //        if(geometry.FaceVertexUvs.Count==0)
            //        {
            //            geometry.FaceVertexUvs.Add(new List<List<Vector2>>());
            //        }
            //        geometry.FaceVertexUvs[0].Add(verticeUvs);
            //    }

            //}


        }

        public Mesh ToMesh(Material material=null)
        {
            var geometry = this.ToGeometry();

            if (material == null) material = new MeshNormalMaterial();
            var mesh = new Mesh(geometry, material);

            mesh.Position.SetFromMatrixPosition(this.Matrix.ToMatrix4f());
            mesh.Rotation.SetFromRotationMatrix(this.Matrix.ToMatrix4f());

            return mesh; 
        }
    }

    public class Polygon 
    {
        public Vector3d Normal;

        public List<Vertex> Vertices;

        public double W;

        protected float EPSILON = .00005f;
        public Polygon(List<Vertex> vertices = null, Vector3d normal = null, double? w = null)
        {
            if (vertices == null)
                this.Vertices = new List<Vertex>();
            else
                this.Vertices = vertices;

            Normal = normal != null ? normal : new Vector3d();

            if (w != null) W = w.Value;
            if (this.Vertices.Count > 0)
            {
                CalculateProperties();
            }

        }
        protected Polygon(Polygon other) : base()
        {
            if (other.Vertices != null)
            {
                this.Vertices = new List<Vertex>();
                for (int i = 0; i < other.Vertices.Count; i++)
                {
                    Vertex v = other.Vertices[i];

                    this.Vertices.Add(v.Clone());
                }
            }
            this.Normal = other.Normal.Clone();

            this.W = other.W;
        }

        public Polygon CalculateProperties()
        {
            var a = this.Vertices[0];
            var b = this.Vertices[1];
            var c = this.Vertices[2];

            this.Normal = b.Clone().Sub(a).Cross(c.Clone().Sub(a)).Normalize();

            this.W = this.Normal.Clone().Dot(a);

            return this;
        }

        public void Invert()
        {
            this.Normal.MultiplyScalar(-1);
            this.W *= -1;
            this.Vertices.Reverse();
        }

        public int ClassifyVertex(Vertex vertex)
        {
            var side = this.Normal.Dot(vertex) - this.W;

            if (side < -ThreeBSP.EPSILON)
                return ThreeBSP.BACK;
            else if(side > ThreeBSP.EPSILON)
            {
                return ThreeBSP.FRONT;
            }
            else
            {
                return ThreeBSP.COPLANAR;
            }

        }

        public int ClassifySide(Polygon polygon)
        {
            int i, classification,
            front = 0,
            back = 0,
            vertice_count = polygon.Vertices.Count;

            for (i = 0; i < vertice_count; i++)
            {
                var vertex = polygon.Vertices[i];
                classification = this.ClassifyVertex(vertex);
                if (classification == ThreeBSP.FRONT)
                {
                    front++;
                }
                else if (classification == ThreeBSP.BACK)
                {
                    back++;
                }
            }

            if (front > 0 && back == 0)
            {
                return ThreeBSP.FRONT;
            }
            if (front == 0 && back > 0)
            {
                return ThreeBSP.BACK;
            }
            if ((front == back && back == 0))
            {
                return ThreeBSP.COPLANAR;
            }
            return ThreeBSP.SPANNING;
            
        }

        public List<Polygon> Tessellate(Polygon poly)
        {

            if (this.ClassifySide(poly) != ThreeBSP.SPANNING)
            {
                return new List<Polygon>() { poly };
            }
            List<Vertex> f = new List<Vertex>();
            List<Vertex> b = new List<Vertex>();

            var count = poly.Vertices.Count;
            int j;
            for (int i = 0; i < poly.Vertices.Count; i++)
            {
                Vertex vi = poly.Vertices[i];
                Vertex vj = poly.Vertices[(i + 1) % count];

                var ti = ClassifyVertex(vi);
                var tj = ClassifyVertex(vj);

                if (ti != ThreeBSP.BACK)
                    f.Add(vi);

                if (ti != ThreeBSP.FRONT)
                    b.Add(vi);

                if ((ti | tj) == ThreeBSP.SPANNING)
                {
                    var t = (this.W - this.Normal.Dot(vi)) / this.Normal.Dot(vj.Clone().Sub(vi));
                    var v = vi.Interpolate(vj, t);
                    f.Add(v);
                    b.Add(v);
                }
            }

            List<Polygon> polys = new List<Polygon>();
            if (f.Count >= 3)
                polys.Add(new Polygon(f,w:this.W));

            if (b.Count >= 3)
                polys.Add(new Polygon(b,w:this.W));

            return polys;
        }
        
        public void SubDivide(Polygon polygon, List<Polygon> coplanar_front, List<Polygon> coplanar_back, List<Polygon> front, List<Polygon> back)
        {
            var _ref = this.Tessellate(polygon);

            //List<List<Polygon>> results = new List<List<Polygon>>();

            for (int i = 0; i < _ref.Count; i++)
            {
                var poly = _ref[i];
                var side = this.ClassifySide(poly);

                switch (side)
                {
                    case 1: // ThreeBSP.FRONT
                        front.Add(poly);
                        //results.Add(front);
                        break;

                    case 2: //ThreeBSP.BACK
                        back.Add(poly);
                        //results.Add(back);
                        break;
                    case 0:// ThreeBSP.COPLANAR
                        if (this.Normal.Dot(poly.Normal) > 0)
                        {
                            coplanar_front.Add(poly);
                            //results.Add(coplanar_front);
                        }
                        else
                        {
                            coplanar_back.Add(poly);
                            //results.Add(coplanar_back);
                        }
                        break;
                    default:
                        throw new Exception("Polygon(" + ThreeBSP.INDEX + ") : BUG : Polygon of classification " + side + " in SubDivision");

                }
            }
            //return results;
        }
        public Polygon Clone()
        {
            return new Polygon(this);
        }
    }

    public class Vertex : Vector3d
    {
        public Vector3d Normal;

        public Vector2d UV;

        public Vertex(double x, double y, double z, Vector3d normal, Vector2d uv) : base(x, y, z)
        {
            this.Normal = normal != null ? normal : new Vector3d();
            this.UV = uv != null ? uv : new Vector2d();
        }

        //public Vertex Add(Vertex vertex)
        //{
        //    this.X += vertex.X;
        //    this.Y += vertex.Y;
        //    this.Z += vertex.Z;
        //    return this;
        //}

        //public Vertex Sub(Vertex vertex)
        //{
        //    this.X -= vertex.X;
        //    this.Y -= vertex.Y;
        //    this.Z -= vertex.Z;
        //    return this;
        //}
        public Vertex Lerp(Vertex v,double t)
        {
            this.X += (v.X - this.X) * t;
            this.Y += (v.Y - this.Y) * t;
            this.Z += (v.Z - this.Z) * t;

            UV.Add(v.UV.Clone()).Sub(this.UV).MultiplyScalar(t);
            Normal.Lerp(v, t);
            return this;
        }
        public Vertex Interpolate(Vertex other,double t)
        {
            return this.Clone().Lerp(other, t);
        }

        public new Vertex Clone()
        {
            return new Vertex(this.X,this.Y,this.Z,this.Normal.Clone(),this.UV.Clone());
        }

        //public new Vertex Normalize()
        //{
        //    var length = System.Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

        //    this.X /= length;
        //    this.Y /= length;
        //    this.Z /= length;

        //    return this;
        //}

        public new Vertex ApplyMatrix4(Matrix4d m)
        {
            //double x = this.X, y = this.Y, z = this.Z;

            //var e = m.Elements;

            //this.X = e[0] * x + e[4] * y + e[8] * z + e[12];
            //this.Y = e[1] * x + e[5] * y + e[9] * z + e[13];
            //this.Z = e[2] * x + e[6] * y + e[10] * z + e[14];

            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var e = m.Elements;

            var w = 1 / (e[3] * x + e[7] * y + e[11] * z + e[15]);

            this.X = (e[0] * x + e[4] * y + e[8] * z + e[12]) * w;
            this.Y = (e[1] * x + e[5] * y + e[9] * z + e[13]) * w;
            this.Z = (e[2] * x + e[6] * y + e[10] * z + e[14]) * w;

            return this;
        }

    }
    public class Node :Hashtable
    {
        public List<Polygon> Polygons = new List<Polygon>();

        public Polygon Divider;

        public Node Front
        {
            get { return (Node)this["front"]; }
            set { this["front"] = value; }
        }

        public Node Back
        {
            get { return (Node)this["back"]; }
            set { this["back"] = value; }
        }
        

        public Node(List<Polygon> polygons = null)
        {
            if (polygons != null && polygons.Count > 0)
            {
                Build(polygons);
            }
        }
        protected Node(Node other)
        {
            Divider = other.Divider.Clone();


            for(int i=0;i<other.Polygons.Count;i++)
            {
                this.Polygons.Add(other.Polygons[i].Clone());
            }

            this.Front = other.Front != null ? other.Front.Clone() as Node : null;

            this.Back = other.Back != null ? other.Back.Clone() as Node : null;
        }
        public new Node Clone()
        {
            return new Node(this);
        }
        public Node Build(List<Polygon> polygons)
        {
            if (this.Divider == null)
                this.Divider = polygons[0].Clone() as Polygon;

            List<Polygon> front = new List<Polygon>();
            List<Polygon> back = new List<Polygon>();

            Hashtable sides = new Hashtable()
            {
                {"front",front },
                {"back",back }
            };

            //List<object> results = new List<object>();

            for (int i = 0; i < polygons.Count; i++)
            {
                var poly = polygons[i];
                this.Divider.SubDivide(poly, this.Polygons, this.Polygons, front, back);
            }

            foreach(string side in sides.Keys)
            {
                var polys = (List<Polygon>)sides[side];
                if (polys.Count > 0)
                {
                    if (!this.ContainsKey(side) || this[side]==null)
                    {
                        this[side] = new Node();                       
                    }
                    (this[side] as Node).Build(polys);

                }
            }

            //if(front.Count>0)
            //{
            //    if (this.Front == null) this.Front = new Node();
            //    this.Front.Build(front);
            //}

            //if(back.Count>0)
            //{
            //    if (this.Back == null) this.Back = new Node();
            //    this.Back.Build(back);
            //}

            return this;
        }

        public bool IsConvex(List<Polygon> polys)
        {
            for (int i = 0; i < polys.Count; i++)
            {
                var inner = polys[i];
                for (int j = 0; j < polys.Count; j++)
                {
                    var outer = polys[j];
                    if (inner != outer && outer.ClassifySide(inner) != ThreeBSP.BACK)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
       
        public List<Polygon> AllPolygons()
        {
            Polygons.Concat(this["front"] != null ? this.Front.AllPolygons() : new List<Polygon>()).Concat(this["back"] != null ? this.Back.AllPolygons(): new List<Polygon>());

            return Polygons;
        }

        public Node Invert()
        {

            for (int i = 0; i <Polygons.Count; i++)
            {
                Polygons[i].Invert();
            }

            if (Divider != null)
                Divider.Invert();

            if (Front != null)
                Front.Invert();

            if (Back != null)
                Back.Invert();

            var tmp = this.Front;

            this.Front = this.Back;

            this.Back = tmp;

            return this;
        }

        public List<Polygon> ClipPolygons(List<Polygon> polygons)
        {
            ThreeBSP.INDEX++;

            if(ThreeBSP.INDEX==27601)
            {
                Debug.WriteLine("INDEX 3");
            }
            if (this.Divider == null) return polygons;

            var front = new List<Polygon>();
            var back = new List<Polygon>();

            for (int i = 0; i < polygons.Count; i++)
            {
                var poly = polygons[i];
                this.Divider.SubDivide(poly, front, back, front, back);
            }

            if (this.Front != null)
            {
                front = this.Front.ClipPolygons(front);
            }

            if (this.Back != null)
            {
                back = this.Back.ClipPolygons(back);
            }

            if(this.Back!=null)
                return front.Concat(back);
            else
                return front;
        }

        public Node ClipTo(Node node)
        {
            this.Polygons = node.ClipPolygons(this.Polygons);
            if (this.Front != null)
            {
                this.Front.ClipTo(node);
            }

            if (this.Back != null)
            {
                this.Back.ClipTo(node);
            }

            return this;
        }
    }
}

