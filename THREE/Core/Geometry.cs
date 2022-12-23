using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace THREE
{
    public struct MorphTarget
    {
        public string Name;
        public List<Vector3> Data;
    }

    public struct MorphColor
    {
        public string Name;

        public List<Color> Colors;
    }

    public class Geometry :ICloneable,IDisposable
    {

        protected static int GeometryIdCount=0;

        private bool _disposed = false;

        public int Id;

        public string Name;

        public string type;

        public Box3 BoundingBox = null;

        public Sphere BoundingSphere = null;

        public Guid Uuid = Guid.NewGuid();

        public List<Vector3> Vertices = new List<Vector3>();

        public List<Color> Colors = new List<Color>(); // one-to-one vertex colors, used in Points and Line

        public List<Face3> Faces = new List<Face3>();

        public List<Vector3> Normals = new List<Vector3>();

        public List<Vector2> Uvs = new List<Vector2>();

        public List<Vector2> Uvs2 = new List<Vector2>();

        public List<DrawRange> Groups = new List<DrawRange>();

        public List<List<List<Vector2>>> FaceVertexUvs = new List<List<List<Vector2>>>();

        public Hashtable MorphTargets = new Hashtable();

        public Hashtable MorphNormals = new Hashtable();

        public List<Vector4> SkinWeights = new List<Vector4>();

        public List<Vector4> SkinIndices = new List<Vector4>();

        public bool IsBufferGeometry { get; set; }
        // update flags
        public bool VerticesNeedUpdate = false;

        public bool ElementsNeedUpdate = false;

        public bool UvsNeedUpdate = false;

        public bool NormalsNeedUpdate = false;

        public bool ColorsNeedUpdate = false;

        public List<float> LineDistances = new List<float>();

        public bool LineDistancesNeedUpdate = false;

        public bool GroupsNeedUpdate = false;

        public BufferGeometry __bufferGeometry;

        public DirectGeometry __directGeometry;

        public float[] normalArray;

        Object3D _obj = new Object3D();

        Vector3 _offset = Vector3.Zero();

        public Geometry()
        {
            Id = GeometryIdCount;
            GeometryIdCount += 1;

            this.Name = "";

            this.type = "Geometry";

            //List<Vector2> uvsList2 = new List<Vector2>();

            //List<List<Vector2>> uvsList1 = new List<List<Vector2>>();

            //uvsList1.Add(uvsList2);
  

            //this.FaceVertexUvs.Add(uvsList1);


            this.IsBufferGeometry = false;
        }
        
        protected Geometry(Geometry source) : this()
        {
            this.Copy(source);
        }
       
        public Geometry Copy(Geometry source)
        {
            this.Name = source.Name;
            var vertices = source.Vertices;

            this.Vertices = new List<Vector3>(source.Vertices);
            this.Colors = new List<Color>(source.Colors);
            this.Faces = new List<Face3>(source.Faces);
            this.FaceVertexUvs = new List<List<List<Vector2>>>(source.FaceVertexUvs);
            this.MorphTargets = (Hashtable)source.MorphTargets.Clone();
            this.MorphNormals = (Hashtable)source.MorphNormals.Clone();
            this.SkinWeights = new List<Vector4>(source.SkinWeights);
            this.SkinIndices = new List<Vector4>(source.SkinIndices);
            this.LineDistances = new List<float>(source.LineDistances);

            if (source.BoundingBox != null)
                this.BoundingBox = (Box3)source.BoundingBox.Clone();

            if (source.BoundingSphere != null)
                this.BoundingSphere = (Sphere)source.BoundingSphere.Clone();

            this.ElementsNeedUpdate = source.ElementsNeedUpdate;
            this.VerticesNeedUpdate = source.VerticesNeedUpdate;
            this.UvsNeedUpdate = source.UvsNeedUpdate;
            this.NormalsNeedUpdate = source.NormalsNeedUpdate;
            this.ColorsNeedUpdate = source.ColorsNeedUpdate;
            this.LineDistancesNeedUpdate = source.LineDistancesNeedUpdate;
            this.GroupsNeedUpdate = source.GroupsNeedUpdate;

            return this;
        }
        public virtual Geometry ApplyMatrix4(Matrix4 matrix)
        {
            var normalMatrix = new Matrix3().GetNormalMatrix(matrix);

            for (int i = 0; i < this.Vertices.Count; i++)
            {
                Vector3 vertex = this.Vertices[i];
                vertex.ApplyMatrix4(matrix);
            }

            for (int i = 0; i < this.Faces.Count; i++)
            {
                Face3 face = this.Faces[i];
                face.Normal.ApplyMatrix3(normalMatrix).Normalize();
                for (int j = 0; j < face.VertexNormals.Count; j++)
                {
                    face.VertexNormals[j].ApplyMatrix3(normalMatrix).Normalize();
                }
            }
            if (this.BoundingBox != null)
            {
                this.ComputeBoundingBox();
            }
            if (this.BoundingSphere != null)
            {
                this.ComputeBoundingSphere();
            }

            this.VerticesNeedUpdate = true;
            this.NormalsNeedUpdate = true;

            return this;
        }

        // angle : Degree
        public virtual Geometry RotateX(float angle)
        {
            return this.ApplyMatrix4(Matrix4.Identity().MakeRotationX(angle));
        }

        public virtual Geometry RotateY(float angle)
        {
            return this.ApplyMatrix4(Matrix4.Identity().MakeRotationY(angle));
        }

        public virtual Geometry RotateZ(float angle)
        {
            return this.ApplyMatrix4(Matrix4.Identity().MakeRotationZ(angle));
        }
       
        public virtual Geometry Translate(float x,float y,float z)
        {
            return this.ApplyMatrix4(Matrix4.Identity().MakeTranslation(x,y,z));
        }


        public virtual Geometry Scale(float x, float y, float z)
        {
            Matrix4 m = Matrix4.Identity().MakeScale(x,y,z);          

            this.ApplyMatrix4(m);

            return this;
        }

        public virtual Geometry LookAt(Vector3 vector)
        {
            _obj.LookAt(vector);
            _obj.UpdateMatrix();

            return this.ApplyMatrix4(_obj.Matrix);
        }

        public Geometry FromBufferGeometry(BufferGeometry geometry)
        {

            var indices = geometry.Index != null ? geometry.Index.Array : null;
            var attributes = geometry.Attributes;

            if (!attributes.ContainsKey("position"))
            {
                Trace.TraceError("THREE.Core.Geometry.FromBufferGeometry():Position attribute required for conversion.");
                return this;
            }

            float[] positions = ((BufferAttribute<float>)attributes["position"]).Array;
            float[] normals = null; // as BufferAttribute<float>)["array"] as float[];
            float[] colors = null;// as BufferAttribute<float>)["array"] as float[];
            float[] uvs = null;// as BufferAttribute<float>)["array"] as float[];
            float[] uvs2 = null;//as BufferAttribute<float>)["array"] as float[];

            
            if(attributes.ContainsKey("normal"))
                normals = ((BufferAttribute<float>)attributes["normal"]).Array;

            if(attributes.ContainsKey("color"))
                colors =  ((BufferAttribute<float>)attributes["color"]).Array;

            if(attributes.ContainsKey("uv"))
                uvs = ((BufferAttribute<float>)attributes["uv"]).Array;

            if(attributes.ContainsKey("uv2"))
                uvs2 = ((BufferAttribute<float>)attributes["uv2"]).Array;

            if (uvs2 != null && uvs2.Length > 0)
            {
                List<Vector2> uvsList2 = new List<Vector2>();
                List<List<Vector2>> uvsList1 = new List<List<Vector2>>();
                uvsList1.Add(uvsList2);

                this.FaceVertexUvs.Add(uvsList1);
            }

            for (int i = 0; i < positions.Length; i += 3)
            {
                this.Vertices.Add(new Vector3().FromArray(positions, i));
                if (colors != null && colors.Length > 0) {
                   
                    this.Colors.Add(Color.ColorName(ColorKeywords.white).FromArray(colors as float[], i));                   
                }
            }

            var groups = geometry.Groups;

            if (groups.Count > 0)
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];

                    var start = (int)group.Start;
                    var count = (int)group.Count;

                    for (int j = start, j1 = start + count; j < j1; j += 3)
                    {
                        if (indices != null && indices.Length > 0)
                        {
                            AddFace(indices[j], indices[j + 1], indices[j + 2], group.MaterialIndex, normals, uvs, uvs2);
                        }
                        else
                        {
                            AddFace(j, j + 1, j + 2, group.MaterialIndex, normals, uvs, uvs2);
                        }
                    }
                }
            }
            else
            {
                if (indices != null && indices.Length > 0)
                {
                    for (int i = 0; i < indices.Length; i += 3)
                    {
                        AddFace(indices[i], indices[i + 1], indices[i + 2], 0, normals, uvs, uvs2);
                    }
                }
                else
                {
                    for (int i = 0; i < positions.Length / 3; i += 3)
                    {
                        AddFace(i, i + 1, i + 2, 0, normals, uvs, uvs2);
                    }
                }
            }

            this.ComputeFaceNormals();

            if (geometry.BoundingBox != null)
            {
                this.BoundingBox = (Box3)geometry.BoundingBox.Clone();
            }

            if (geometry.BoundingSphere != null)
            {
                this.BoundingSphere = (Sphere)geometry.BoundingSphere.Clone();
            }
            return this;
        }

        public virtual Geometry Center()
        {
            this.ComputeBoundingBox();
            this.BoundingBox.GetCenter(_offset).Negate();
            this.Translate(_offset.X, _offset.Y, _offset.Z);

            return this;
        }

        public virtual Geometry Normalize()
        {
            this.ComputeBoundingSphere();

            var center = this.BoundingSphere.Center;
            var radius = this.BoundingSphere.Radius;

            var s = radius == 0 ? 1 : 1.0f / radius;
            Matrix4 matrix = new Matrix4(
                s, 0, 0, -s * center.X,
                0, s, 0, -s * center.Y,
                0, 0, s, -s * center.Z,
                0, 0, 0, 1);

            return this.ApplyMatrix4(matrix);
        }          

        public virtual void ComputeFaceNormals()
        {
            var cb = new Vector3();
            var ab = new Vector3();
            //int vLen = this.Vertices.Count - 1;
            for (int f = 0, f1 = this.Faces.Count; f < f1; f++)
            {
                var face = this.Faces[f];

                if (face.Normal == null) face.Normal = new Vector3();
                //if (face.c > vLen)
                //    continue;

                var vA = this.Vertices[face.a];
                var vB = this.Vertices[face.b];                             
                var vC = this.Vertices[face.c];

                cb = vC - vB;
                ab = vA - vB;
                cb.Cross(ab);

                cb.Normalize();

                face.Normal.Copy(cb);

            }
        }
        public virtual void ComputeVertexNormals(bool? areaWeighted=null)
        {
            if (areaWeighted == null) areaWeighted = true;

           

            //vertices = new Array(this.vertices.length);
            Vector3[] vertices = new Vector3[this.Vertices.Count];

            for (int v = 0;v < this.Vertices.Count; v++)
            {

                vertices[v] = new Vector3();

            }

            if (areaWeighted.Value)
            {

                // vertex normals weighted by triangle areas
                // http://www.iquilezles.org/www/articles/normals/normals.htm

                Vector3 vA, vB, vC;
                var cb = new Vector3();
                var ab = new Vector3();

                for (int f = 0;f< this.Faces.Count;f++)
                {

                    var face = this.Faces[f];

                    vA = this.Vertices[face.a];
                    vB = this.Vertices[face.b];
                    vC = this.Vertices[face.c];

                    cb.SubVectors(vC, vB);
                    ab.SubVectors(vA, vB);
                    cb.Cross(ab);

                    vertices[face.a].Add(cb);
                    vertices[face.b].Add(cb);
                    vertices[face.c].Add(cb);

                }

            }
            else
            {

                this.ComputeFaceNormals();

                for (int f = 0;f<this.Faces.Count; f++)
                {

                    var face = this.Faces[f];

                    vertices[face.a].Add(face.Normal);
                    vertices[face.b].Add(face.Normal);
                    vertices[face.c].Add(face.Normal);

                }

            }

            for (int v = 0;v< this.Vertices.Count; v++)
            {

                vertices[v].Normalize();

            }

            for (int f = 0;f< this.Faces.Count; f++)
            {

                var face = this.Faces[f];

                var vertexNormals = face.VertexNormals;

                if (vertexNormals.Count == 3)
                {

                    vertexNormals[0].Copy(vertices[face.a]);
                    vertexNormals[1].Copy(vertices[face.b]);
                    vertexNormals[2].Copy(vertices[face.c]);

                }
                else
                {

                    vertexNormals.Add((Vector3)vertices[face.a].Clone());
                    vertexNormals.Add((Vector3)vertices[face.b].Clone());
                    vertexNormals.Add((Vector3)vertices[face.c].Clone());

                }

            }

            if (this.Faces.Count > 0)
            {

                this.NormalsNeedUpdate = true;

            }

        }


        public virtual void ComputeFlatVertexNormals()
        {

            this.ComputeFaceNormals();

            for (int f = 0;f< this.Faces.Count; f++)
            {

                var face = this.Faces[f];

                var vertexNormals = face.VertexNormals;

                if (vertexNormals.Count == 3)
                {

                    vertexNormals[0].Copy(face.Normal);
                    vertexNormals[1].Copy(face.Normal);
                    vertexNormals[2].Copy(face.Normal);

                }
                else
                {

                    vertexNormals[0] = (Vector3)face.Normal.Clone();
                    vertexNormals[1] = (Vector3)face.Normal.Clone();
                    vertexNormals[2] = (Vector3)face.Normal.Clone();

                }

            }

            if (this.Faces.Count > 0)
            {

                this.NormalsNeedUpdate = true;

            }
        }

        public virtual void ComputeMorphNormals()
        {
        }

        
        public virtual void ComputeBoundingBox()
        {
            if (this.BoundingBox == null)
            {
                this.BoundingBox = new Box3();
            }

            this.BoundingBox.SetFromPoints(this.Vertices);
        }

        public virtual void ComputeBoundingSphere()
        {
            if (this.BoundingSphere == null)
            {
                this.BoundingSphere = new Sphere();
            }
            this.BoundingSphere.SetFromPoints(this.Vertices);
        }

        private void AddFace(int a, int b, int c, int materialIndex, float[] normals, float[] uvs, float[] uvs2)
        {
            var vertexColors = new List<Color>();
            if (this.Colors.Count > 0)
            {
                vertexColors.Add(this.Colors[a]);
                vertexColors.Add(this.Colors[b]);
                vertexColors.Add(this.Colors[c]);
            }

            var vertexNormals = new List<Vector3>();

            if (normals != null && normals.Length > 0)
            {
                vertexNormals.Add(new Vector3().FromArray(normals, a * 3));
                vertexNormals.Add(new Vector3().FromArray(normals, b * 3));
                vertexNormals.Add(new Vector3().FromArray(normals, c * 3));
            }

            var face = new Face3(a, b, c, vertexNormals, vertexColors, materialIndex);

            this.Faces.Add(face);

            if (uvs != null && uvs.Length > 0)
            {
                List<Vector2> list2 = new List<Vector2>();
                list2.Add(new Vector2().FromArray(uvs, a * 2));
                list2.Add(new Vector2().FromArray(uvs, b * 2));
                list2.Add(new Vector2().FromArray(uvs, c * 2));
                List<List<Vector2>> list1 = new List<List<Vector2>>();
                list1.Add(list2);
                if (this.FaceVertexUvs.Count == 0)
                    this.FaceVertexUvs.Add(list1);
                else
                    this.FaceVertexUvs[0].Add(list2);
                
            }
            if (uvs2 != null && uvs2.Length > 0)
            {
                List<Vector2> list2 = new List<Vector2>();
                list2.Add(new Vector2().FromArray(uvs2, a * 2));
                list2.Add(new Vector2().FromArray(uvs2, b * 2));
                list2.Add(new Vector2().FromArray(uvs2, c * 2));
                List<List<Vector2>> list1 = new List<List<Vector2>>();
                list1.Add(list2);
                if (this.FaceVertexUvs.Count == 1)
                    this.FaceVertexUvs.Add(list1);
                else
                    this.FaceVertexUvs[1].Add(list2);
               
            }

        }

        public object Clone()
        {
            return new Geometry(this);
        }

        public virtual void Merge(Geometry geometry, Matrix4 matrix, int materialIndexOffset=0)
        {
            Matrix3 normalMatrix = new Matrix3();
            int vertexOffset = this.Vertices.Count;
            List<Vector3> vertices1 = this.Vertices;
            List<Vector3> vertices2 = geometry.Vertices;
            List<Face3> faces1 = this.Faces;
            List<Face3> faces2 = geometry.Faces;
            List<Color> color1 = this.Colors;
            List<Color> color2 = geometry.Colors;

            normalMatrix.GetNormalMatrix(matrix);

            // vertices
            for (int i = 0; i < vertices2.Count; i++)
            {
                var vertex = vertices2[i];

                var vertexCopy = vertex;

                vertexCopy = vertexCopy.ApplyMatrix4(matrix);

                vertices1.Add(vertexCopy);
            }

            // colors

            for (int i = 0; i < color2.Count; i++)
            {
                color1.Add(color2[i]);
            }

            // faces

            for (int i = 0; i < faces2.Count; i++)
            {
                var face = faces2[i];
                Vector3 normal;
                var faceVertexNormals = face.VertexNormals;
                var faceVertexColors = face.VertexColors;
                
                var faceCopy = new Face3(face.a + vertexOffset, face.b + vertexOffset, face.c + vertexOffset);
                faceCopy.Normal.Copy(face.Normal);

                Color color;


                faceCopy.Normal.ApplyMatrix3(normalMatrix).Normalize();

                for (var j = 0; j < faceVertexNormals.Count; j++)
                {
                    normal = (Vector3)faceVertexNormals[j].Clone();

                    normal.ApplyMatrix3(normalMatrix).Normalize();

                    faceCopy.VertexNormals.Add(normal);
                }

                faceCopy.Color = face.Color;

                for (int j = 0; j < faceVertexColors.Count; j++)
                {
                    color = faceVertexColors[j];
                    faceCopy.VertexColors.Add(color);
                }

                faceCopy.MaterialIndex = face.MaterialIndex + materialIndexOffset;

                faces1.Add(faceCopy);
            }

            //uvs

            for (int i = 0; i < geometry.FaceVertexUvs.Count; i++)
            {
                var faceVertexUvs2 = geometry.FaceVertexUvs[i];

                for (int j = 0; j < faceVertexUvs2.Count; j++)
                {
                    var uvs2 = faceVertexUvs2[j];
                    List<Vector2> uvsCopy = new List<Vector2>();
                    for (int k = 0; k < uvs2.Count; k++)
                    {
                        uvsCopy.Add((Vector2)uvs2[k].Clone());

                    }
                    if ((FaceVertexUvs.Count - 1) < i)
                        FaceVertexUvs.Add(new List<List<Vector2>>());
                    this.FaceVertexUvs[i].Add(uvsCopy);
                }

            }
        }

        public virtual void MergeMesh(Mesh mesh)
        {
            if (mesh.MatrixAutoUpdate) mesh.UpdateMatrix();
            this.Merge(mesh.Geometry, mesh.Matrix);
        }

        /*
         * Checks for duplicate vertices with hashmap.
         * Duplicated vertices are removed
         * and faces' vertices are updated.
         */
        public virtual int MergeVertices()
        {
            var verticesMap = new Dictionary<string,int>();
            List<Vector3> unique = new List<Vector3>();
            List<int> changes = new List<int>();
            string key;

            int precisionPoints = 4;
            float precision = (float)System.Math.Pow(10, precisionPoints);

            for (int i = 0; i < this.Vertices.Count; i++)
            {
                var v = this.Vertices[i];
                key = System.Math.Round(v.X * precision) + "_" + System.Math.Round(v.Y * precision) + "_" + System.Math.Round(v.Z * precision);

                var value = 0;
                if (!verticesMap.TryGetValue(key,out value))
                {
                    verticesMap[key] = i;
                    unique.Add(v);
                    changes.Add(unique.Count - 1);

                }
                else
                {
                    int idx = verticesMap[key];
                    changes.Add(changes[idx]);
                }
            }


            // if faces are completely degenerate after merging vertices, we
            // have to remove them from the geometry.
            List<int> faceIndicesToRemove = new List<int>();
            //int cLen = changes.Count - 1;
            for (int i = 0; i < this.Faces.Count; i++)
            {
                var face = this.Faces[i];

               
                face.a = changes[face.a];
                face.b = changes[face.b];
                //if (face.c > cLen) continue;
                face.c = changes[face.c];

                var indices = new int[] { face.a, face.b, face.c };
               

                for (int n = 0; n < 3; n++)
                {
                    if (indices[n] == indices[(n + 1) % 3])
                    {
                        faceIndicesToRemove.Add(i);
                        break;
                    }
                }
            }

            //foreach(var idx in faceIndicesToRemove)
            //{

            //    this.Faces.RemoveAt(idx);

            //    for (int j = 0; j < this.FaceVertexUvs.Count; j++)
            //    {
            //        this.FaceVertexUvs[j].RemoveAt(idx);
            //    }
            //}

            for (int i = faceIndicesToRemove.Count - 1; i >= 0; i--)
            {

                var idx = faceIndicesToRemove[i];

                this.Faces.RemoveAt(idx);

                for (int j = 0; j < this.FaceVertexUvs.Count; j++)
                {

                    this.FaceVertexUvs[j].RemoveAt(idx);
                }
                

            }
            var diff = this.Vertices.Count - unique.Count;
            this.Vertices = unique;

            return diff;
        }

        public Geometry SetFromPoints(List<Vector3> points)
        {
            this.Vertices.Clear();
            for(int i = 0; i < points.Count; i++)
            {
                Vector3 point = points[i];
                this.Vertices.Add(new Vector3(point.X, point.Y, point.Z));
            }

            return this;
        }

        #region IDisposable Members

        public event EventHandler<EventArgs> Disposed;
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    this.RaiseDisposed();
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion


    }
}
