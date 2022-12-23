using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using THREE;

namespace THREE
{
    public struct DrawRange
    {
        public float Start;
        public int MaterialIndex;
        public float Count;
    }

    public class BufferGeometry : Geometry
    {

        protected static int BufferGeometryIdCount=0;

        public GLAttributes Attributes;

        //public List<string> AttributesKeys { get; set; }

        //public IList<DrawRange> Drawcalls = new List<DrawRange>();

        //public IList<DrawRange> Offsets;

        public BufferAttribute<int> Index = null;

        public Hashtable MorphAttributes;

        public bool MorphTargetsRelative = false;

        public DrawRange DrawRange = new DrawRange { Start = 0, MaterialIndex = -1, Count = float.PositiveInfinity };

        private Box3 _box = new Box3();

        private Box3 _boxMorphTargets = new Box3();

        private Vector3 _vector = Vector3.Zero();

        private Vector3 _offset = Vector3.Zero();

        private Object3D _obj = new Object3D();

        public Hashtable UserData = new Hashtable();

        public BufferGeometry()
        {
            Id = BufferGeometryIdCount;
            BufferGeometryIdCount += 2;

            this.type = "BufferGeometry";

            this.Attributes = new GLAttributes();

            this.MorphAttributes = new Hashtable();

            //this.Offsets = this.Drawcalls;

            this.BoundingBox = null;

            this.BoundingSphere = null;

            this.IsBufferGeometry = true;

            //AttributesKeys = new List<string>();

        }
        protected BufferGeometry(BufferGeometry source)
        {
            Copy(source);
        }
        public new BufferGeometry Clone()
        {
            return new BufferGeometry(this);
        }
        public BufferGeometry Copy(BufferGeometry source) 
        {
            this.Index = null;
            this.Attributes = new GLAttributes();

            this.MorphAttributes = (Hashtable)source.MorphAttributes.Clone();

            this.Groups = new List<DrawRange>(source.Groups);
            this.BoundingBox = null;
            this.BoundingSphere = null;

            // used for storing cloned, shared data


            // name

            this.Name = source.Name;

            // index

            var index = source.Index;

            if (index != null)
            {

                this.Index =source.Index.Clone();

            }

            // attributes

            var attributes = source.Attributes;
            foreach(var entry in attributes)
            {
                if(entry.Value is BufferAttribute<float>)
                    Attributes.Add(entry.Key, (entry.Value as BufferAttribute<float>).Clone());
                if (entry.Value is BufferAttribute<int>)
                    Attributes.Add(entry.Key, (entry.Value as BufferAttribute<int>).Clone());
                if (entry.Value is BufferAttribute<byte>)
                    Attributes.Add(entry.Key, (entry.Value as BufferAttribute<byte>).Clone());
            }
            //for ( const name in attributes ) {

            //    const attribute = attributes[name];
            //    this.setAttribute(name, attribute.clone(data));

            //}

            // morph attributes


            //for ( const name in morphAttributes ) {

            //    const array = [];
            //    const morphAttribute = morphAttributes[name]; // morphAttribute: array of Float32BufferAttributes

            //    for (let i = 0, l = morphAttribute.length; i < l; i++)
            //    {

            //        array.push(morphAttribute[i].clone(data));

            //    }

            //    this.morphAttributes[name] = array;

            //}

            this.MorphTargetsRelative = source.MorphTargetsRelative;

            // groups
            //const groups = source.groups;

            //for (let i = 0, l = groups.length; i < l; i++)
            //{

            //    const group = groups[i];
            //    this.addGroup(group.start, group.count, group.materialIndex);

            //}

            // bounding box

            var boundingBox = source.BoundingBox;

            if (boundingBox != null)
            {

                this.BoundingBox = (Box3)boundingBox.Clone();

            }

            // bounding sphere

            var boundingSphere = source.BoundingSphere;

            if (boundingSphere != null)
            {

                this.BoundingSphere = (Sphere)boundingSphere.Clone();

            }

            // draw range

            this.DrawRange.Start = source.DrawRange.Start;
            this.DrawRange.Count = source.DrawRange.Count;

            // user data

            this.UserData = (Hashtable)source.UserData.Clone();

            return this;
        }
        public BufferAttribute<int> GetIndex()
        {
            return this.Index;
        }
        public void SetIndex(List<int> index, int itemSize=1)
        {
            this.Index = new BufferAttribute<int>(index.ToArray<int>(), itemSize);
        }
        public void SetIndex(BufferAttribute<int> index)
        {
            this.Index = index;
        }
        public GLAttribute GetAttribute<T>(string name)
        {
            return this.Attributes[name] as GLAttribute;
        }

        public BufferGeometry SetAttribute(string name, GLAttribute attribute)
        {
            this.Attributes[name] = attribute;
            //if (!AttributesKeys.Contains(name))
            //    this.AttributesKeys.Add(name);

            return this;

        }

        public void deleteAttribute(string name)
        {
            this.Attributes.Remove(name);
        }


        //public void AddAttribute(string name, Renderers.Shaders.Attribute attribute)
        //{
        //    if (attribute is IBufferAttribute == false)
        //    {
        //        Trace.TraceWarning("BufferGeometry: .addAttribute() now expects ( name, attribute ).");
        //    }

        //    this.Attributes[name] = attribute;

        //    this.AttributesKeys = new List<string>();
        //    foreach (var entry in this.Attributes)
        //    {
        //        this.AttributesKeys.Add(entry.Key);
        //    }
        //}

        public virtual void AddGroup(int start, int count, int materialIndex)
        {
            this.Groups.Add(new DrawRange { Start = start, Count = count, MaterialIndex = materialIndex });
        }

        public void ClearGroups()
        {
            this.Groups.Clear();
        }

        public void SetDrawRange(int start, int count)
        {
            this.DrawRange.Start = start;
            this.DrawRange.Count = count;
        }

        public BufferGeometry ApplyMatrix(Matrix4 matrix)
        {
            if (this.Attributes.ContainsKey("position"))
            {
                BufferAttribute<float> position = (BufferAttribute<float>)this.Attributes["position"];

                if (position != null)
                {
                    position = matrix.ApplyToBufferAttribute(position);
                    position.NeedsUpdate = true;
                }
            }
            if (this.Attributes.ContainsKey("normal"))
            {
                BufferAttribute<float> normal = (BufferAttribute<float>)this.Attributes["normal"];

                if (normal != null)
                {
                    Matrix3 normalMatrix = new Matrix3().GetNormalMatrix(matrix);

                    normal = normalMatrix.ApplyToBufferAttribute(normal);
                    normal.NeedsUpdate = true;
                }
            }

            if (this.Attributes.ContainsKey("tangent"))
            {
                BufferAttribute<float> tangent = (BufferAttribute<float>)this.Attributes["tangent"];

                if (tangent != null)
                {
                    Matrix3 normalMatrix = new Matrix3().GetNormalMatrix(matrix);
                    tangent = normalMatrix.ApplyToBufferAttribute(tangent);
                    tangent.NeedsUpdate = true;
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
            return this;
        }

        public new BufferGeometry RotateX(float angle)
        {
            Matrix4 m = Matrix4.Identity().MakeRotationX(angle);

            this.ApplyMatrix(m);

            return this;
        }

        public new BufferGeometry RotateY(float angle)
        {
            Matrix4 m = Matrix4.Identity().MakeRotationY(angle);

            this.ApplyMatrix(m);

            return this;
        }

        public new BufferGeometry RotateZ(float angle)
        {
            Matrix4 m = Matrix4.Identity().MakeRotationZ(angle);

            this.ApplyMatrix(m);

            return this;
        }

        public new BufferGeometry Translate(float x, float y, float z)
        {
            Matrix4 m = Matrix4.CreateTranslation(x, y, z);

            this.ApplyMatrix(m);

            return this;
        }

        public new BufferGeometry Scale(float x, float y, float z)
        {
            Matrix4 m = Matrix4.Identity().MakeScale(x,y,z);

            this.ApplyMatrix(m);

            return this;
        }

        public new BufferGeometry LookAt(Vector3 vector)
        {
            _obj.LookAt(vector);

            _obj.UpdateMatrix();

            this.ApplyMatrix(_obj.Matrix);

            return this;

        }
        public new BufferGeometry Center()
        {
            this.ComputeBoundingBox();

            this.BoundingBox.GetCenter(_offset).Negate();

            this.Translate(_offset.X, _offset.Y, _offset.Z);

            return this;
        }
        public BufferGeometry SetFromObject(Object3D object3D)
        {
            Geometry geometry = object3D.Geometry;

            if (object3D is Points || object3D is Line)
            {
                var positions = new BufferAttribute<float>();//(geometry.Vertices.ToArray().ToArray<float>(), geometry.Vertices.Count * 3, 3);
                positions.ItemSize = 3;
                positions.Type = typeof(float);

                var colors = new BufferAttribute<float>();
                colors.ItemSize = 3;
                colors.Type = typeof(float);

                //TODO change Vector3 List not Vector3 to float[]
                this.SetAttribute("position", positions.CopyVector3sArray(geometry.Vertices.ToArray()));
                this.SetAttribute("color", colors.CopyColorsArray(geometry.Colors.ToArray<Color>()));

                if (geometry.LineDistances != null && geometry.LineDistances.Count == geometry.Vertices.Count)
                {
                    BufferAttribute<float> lineDistances = new BufferAttribute<float>();
                    lineDistances["type"] = typeof(float);
                    lineDistances["array"] = new float[geometry.LineDistances.Count];
                    lineDistances.ItemSize = 1;

                    Array.Copy(geometry.LineDistances.ToArray(), lineDistances.Array, (long)geometry.LineDistances.Count);

                    this.SetAttribute("lineDistance", lineDistances);
                }

                if (geometry.BoundingSphere != null)
                {
                    this.BoundingSphere = (Sphere)geometry.BoundingSphere.Clone();
                }

                if (geometry.BoundingBox != null)
                {
                    this.BoundingBox = (Box3)geometry.BoundingBox.Clone();
                }
            }
            else if (object3D is Mesh)
            {
                if (geometry != null && !geometry.IsBufferGeometry)
                {
                    this.FromGeometry(geometry);
                }
            }
            return this;
        }
        public new BufferGeometry SetFromPoints(List<Vector3> points)
        {
            List<float> position = new List<float>();

            for(int i = 0; i < points.Count; i++)
            {
                Vector3 point = points[i];
                position.Add(point.X, point.Y, point.Z);

            }
            this.SetAttribute("position", new BufferAttribute<float>(position.ToArray(), 3));

            return this;
        }
        public BufferGeometry SetFromPoints(Vector3[] points)
        {
            List<float> position = new List<float>();

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = points[i];

                position.Add(point.X);
                position.Add(point.Y);
                position.Add(point.Z);
            }

            this.SetAttribute("position", new BufferAttribute<float>(position.ToArray(), 3));

            return this;
        }

        public BufferGeometry UpdateFromObject(Object3D object3D)
        {
            Geometry geometry = object3D.Geometry as Geometry;

            if (object3D is Mesh)
            {
                DirectGeometry direct = geometry.__directGeometry;

                if (geometry.ElementsNeedUpdate == true)
                {
                    direct = null;
                    geometry.ElementsNeedUpdate = false;
                }

                if (direct == null)
                {
                    return this.FromGeometry(geometry);
                }

                direct.VerticesNeedUpdate = geometry.VerticesNeedUpdate;
                direct.NormalsNeedUpdate = geometry.NormalsNeedUpdate;
                direct.ColorsNeedUpdate = geometry.ColorsNeedUpdate;
                direct.UvsNeedUpdate = geometry.UvsNeedUpdate;
                direct.GroupsNeedUpdate = geometry.GroupsNeedUpdate;

                geometry.VerticesNeedUpdate = false;
                geometry.NormalsNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;
                geometry.UvsNeedUpdate = false;
                geometry.GroupsNeedUpdate = false;

                geometry = direct;
            }

            object attribute = null;

            if (geometry.VerticesNeedUpdate == true)
            {
                if (this.Attributes.TryGetValue("position", out attribute))
                {
                    if (attribute != null)
                    {
                        (attribute as BufferAttribute<float>).CopyVector3sArray(geometry.Vertices.ToArray());
                        (attribute as BufferAttribute<float>).NeedsUpdate = true;
                    }
                    geometry.VerticesNeedUpdate = false;
                }
            }

            if (geometry.NormalsNeedUpdate == true)
            {

                if (this.Attributes.TryGetValue("normal", out attribute))
                {
                    if (attribute != null)
                    {
                        (attribute as BufferAttribute<float>).CopyVector3sArray(geometry.Normals.ToArray());
                        (attribute as BufferAttribute<float>).NeedsUpdate = true;
                    }
                    geometry.NormalsNeedUpdate = false;
                }
            }

            if (geometry.ColorsNeedUpdate == true)
            {
                if (this.Attributes.TryGetValue("color", out attribute))
                {
                    if (attribute != null)
                    {
                        (attribute as BufferAttribute<float>).CopyColorsArray(geometry.Colors.ToArray());
                        (attribute as BufferAttribute<float>).NeedsUpdate = true;
                    }
                    geometry.ColorsNeedUpdate = false;
                }
            }

            if (geometry.UvsNeedUpdate == true)
            {

                if (this.Attributes.TryGetValue("uv", out attribute))
                {
                    if (attribute != null)
                    {
                        (attribute as BufferAttribute<float>).CopyVector2sArray(geometry.Uvs.ToArray());
                        (attribute as BufferAttribute<float>).NeedsUpdate = true;
                    }
                    geometry.UvsNeedUpdate = false;
                }
            }

            if (geometry.LineDistancesNeedUpdate == true)
            {
                if (this.Attributes.TryGetValue("lineDistance", out attribute))
                {
                    if (attribute != null)
                    {
                        (attribute as BufferAttribute<float>).CopyArray(geometry.LineDistances.ToArray());
                        (attribute as BufferAttribute<float>).NeedsUpdate = true;
                    }

                    geometry.LineDistancesNeedUpdate = false;
                }
            }

            if (geometry.GroupsNeedUpdate == true)
            {
                DirectGeometry directGeometry = geometry as DirectGeometry;

                directGeometry.ComputeGroups(geometry);

                this.Groups = directGeometry.Groups;

                directGeometry.GroupsNeedUpdate = false;
            }

            return this;
        }

        public BufferGeometry FromGeometry(Geometry geometry)
        {
            geometry.__directGeometry = new DirectGeometry().FromGeometry(geometry);

            return this.FromDirectGeometry(geometry.__directGeometry);
        }

        public BufferGeometry FromDirectGeometry(DirectGeometry geometry)
        {
            float[] positions = new float[geometry.Vertices.Count * 3];

            this.SetAttribute("position", new BufferAttribute<float>(positions, 3).CopyVector3sArray(geometry.Vertices.ToArray()));

            if (geometry.Normals.Count > 0)
            {
                float[] normals = new float[geometry.Normals.Count * 3];
                this.SetAttribute("normal", new BufferAttribute<float>(normals, 3).CopyVector3sArray(geometry.Normals.ToArray()));
            }

            if (geometry.Colors.Count > 0)
            {
                float[] colors = new float[geometry.Colors.Count * 3];
                this.SetAttribute("color", new BufferAttribute<float>(colors, 3).CopyColorsArray(geometry.Colors.ToArray()));
            }

            if (geometry.Uvs.Count > 0)
            {
                float[] uvs = new float[geometry.Uvs.Count * 2];
                this.SetAttribute("uv", new BufferAttribute<float>(uvs, 2).CopyVector2sArray(geometry.Uvs.ToArray()));
            }

            if (geometry.Uvs2.Count > 0)
            {
                float[] uvs2 = new float[geometry.Uvs2.Count * 2];
                this.SetAttribute("uv2", new BufferAttribute<float>(uvs2, 2).CopyVector2sArray(geometry.Uvs2.ToArray()));
            }

            // groups
            this.Groups = geometry.Groups;

            // morphs

            foreach (string name in geometry.MorphTargets.Keys)
            {
                List<BufferAttribute<float>> array = new List<BufferAttribute<float>>();

                var morphTargets = (List<MorphTarget>)geometry.MorphTargets[name];

                for (int i = 0; i < morphTargets.Count; i++)
                {
                    var morphTarget = morphTargets[i];
                    float[] values = new float[morphTarget.Data.Count * 3];
                    var attribute = new BufferAttribute<float>(values, 3);
                    attribute.Name = morphTarget.Name;

                    array.Add(attribute.CopyVector3sArray(morphTarget.Data.ToArray()));
                }
                this.MorphAttributes.Add(name, array);

            }

            //skinning

            if (geometry.SkinIndices.Count > 0)
            {
                float[] skinBuffer = new float[geometry.SkinIndices.Count * 4];
                var skinIndices = new BufferAttribute<float>(skinBuffer, 4);
                this.SetAttribute("skinIndex", skinIndices.CopyVector4sArray(geometry.SkinIndices.ToArray()));
            }

            if (geometry.SkinWeights.Count > 0)
            {
                float[] skinBuffer = new float[geometry.SkinWeights.Count * 4];
                var skinWeights = new BufferAttribute<float>(skinBuffer, 4);
                this.SetAttribute("skinWeight", skinWeights.CopyVector4sArray(geometry.SkinWeights.ToArray()));
            }

            if (geometry.BoundingSphere != null)
            {
                this.BoundingSphere = (Sphere)geometry.BoundingSphere.Clone();
            }

            if (geometry.BoundingBox != null)
            {
                this.BoundingBox = (Box3)geometry.BoundingBox.Clone();
            }
            return this;
        }
        public override void ComputeBoundingBox()
        {
            if (this.BoundingBox == null)
            {
                this.BoundingBox = new Box3();
            }

            //var position = this.Attributes["position"];
            //var morphAttributesPosition = this.MorphAttributes["position"] as List<BufferAttribute<float>>;

            BufferAttribute<float> position = null;

            if (this.Attributes.ContainsKey("position"))
                position = (BufferAttribute<float>)this.Attributes["position"];

            List<BufferAttribute<float>> morphAttributesPosition = null;
            if (this.MorphAttributes.ContainsKey("position"))
                morphAttributesPosition = this.MorphAttributes["position"] as List<BufferAttribute<float>>;

            if (position != null)
            {
                this.BoundingBox.SetFromBufferAttribute(position as BufferAttribute<float>);

                // process morph attributes if present

                if (morphAttributesPosition != null)
                {
                    for (int i = 0; i < morphAttributesPosition.Count; i++)
                    {
                        BufferAttribute<float> morphAttribute = morphAttributesPosition[i];
                        _box.SetFromBufferAttribute(morphAttribute);

                        if (this.MorphTargetsRelative)
                        {

                            _vector.AddVectors(this.BoundingBox.Min, _box.Min);
                            this.BoundingBox.ExpandByPoint(_vector);

                            _vector.AddVectors(this.BoundingBox.Max, _box.Max);
                            this.BoundingBox.ExpandByPoint(_vector);

                        }
                        else
                        {

                            this.BoundingBox.ExpandByPoint(_box.Min);
                            this.BoundingBox.ExpandByPoint(_box.Max);

                        }

                    }
                }
            }
            else
            {
                this.BoundingBox.MakeEmpty();
            }

            if (float.IsNaN(this.BoundingBox.Min.X) || float.IsNaN(this.BoundingBox.Min.Y) || float.IsNaN(this.BoundingBox.Min.Z))
            {
                Trace.TraceError("THREE.Core.BufferGeometry.ComputeBoundingBox : Compute min/max have Nan values. The \"Position\" attribute is likely to have NaN values.");
            }
        }
        public override void ComputeBoundingSphere()
        {

            if (this.BoundingSphere == null)
            {
                this.BoundingSphere = new Sphere();
            }

            BufferAttribute<float> position = null;

            if (this.Attributes.ContainsKey("position") && this.Attributes["position"] is GLBufferAttribute)
            {
                BoundingSphere.Set(new Vector3(), float.PositiveInfinity);
                return;
            }

            if (this.Attributes.ContainsKey("position"))
                position = (BufferAttribute<float>)this.Attributes["position"];

            List<BufferAttribute<float>> morphAttributesPosition = null;
            if(this.MorphAttributes.ContainsKey("position"))
                morphAttributesPosition = this.MorphAttributes["position"] as List<BufferAttribute<float>>;

            if (position != null)
            {
                var center = this.BoundingSphere.Center;

                if(position is InterleavedBufferAttribute<float>)
                    _box.SetFromBufferAttribute(position as InterleavedBufferAttribute<float>);
                else
                    _box.SetFromBufferAttribute(position as BufferAttribute<float>);

                if (morphAttributesPosition != null)
                {
                    for (int i = 0; i < morphAttributesPosition.Count; i++)
                    {
                        BufferAttribute<float> morphAttribute = morphAttributesPosition[i];

                        _boxMorphTargets.SetFromBufferAttribute(morphAttribute);

                        if (this.MorphTargetsRelative)
                        {
                            _vector.AddVectors(_box.Min, _boxMorphTargets.Min);
                            _box.ExpandByPoint(_vector);

                            _vector.AddVectors(_box.Max, _boxMorphTargets.Max);
                            _box.ExpandByPoint(_vector);
                        }
                        else
                        {
                            _box.ExpandByPoint(_boxMorphTargets.Min);
                            _box.ExpandByPoint(_boxMorphTargets.Max);
                        }
                    }
                }

                center = _box.GetCenter(center);
                this.BoundingSphere.Center = center;

                // second, try to find a boundingSphere with a radius smaller than the
                // boundingSphere of the boundingBox: sqrt(3) smaller in the best case

                float maxRadiusSq = 0;

                for (int i = 0; i <( position is InterleavedBufferAttribute<float> ? (position as InterleavedBufferAttribute<float>).count:position.count); i++)
                {
                    _vector = _vector.FromBufferAttribute(position, i);
                    maxRadiusSq = System.Math.Max(maxRadiusSq, center.DistanceToSquared(_vector));
                }

                // process morph attributes if present
                if (morphAttributesPosition != null)
                {
                    for (int i = 0; i < morphAttributesPosition.Count; i++)
                    {
                        BufferAttribute<float> morphAttribute = morphAttributesPosition[i];

                        for (int j = 0; j < morphAttribute.count; j++)
                        {
                            _vector = _vector.FromBufferAttribute(morphAttribute, j);
                            maxRadiusSq = System.Math.Max(maxRadiusSq, center.DistanceToSquared(_vector));

                        }
                    }
                }

                this.BoundingSphere.Radius = (float)System.Math.Sqrt(maxRadiusSq);

                if (float.IsNaN(this.BoundingSphere.Radius))
                {
                    Trace.TraceError("THREE.Core.BufferGeometry.ComputeBoundingSphere():Computed radius is Nan. The 'Position' attribute is likely to hava Nan values.");
                }
            }

        }
        public new void ComputeVertexNormals(bool? areaWeighted = null)
        {
            var index = this.Index;
            var attributes = this.Attributes;

            BufferAttribute<float> positionsAttribute = (BufferAttribute<float>)this.Attributes["position"];
            BufferAttribute<float> normalAttribute = null;
            if(this.Attributes.ContainsKey("normal"))
                normalAttribute = (BufferAttribute<float>)this.Attributes["normal"];

            var positions = positionsAttribute.Array;

            if (positionsAttribute != null)
            {

                if (!attributes.ContainsKey("normal"))
                {
                    normalAttribute = new BufferAttribute<float>(new float[positions.Length], 3);
                    this.SetAttribute("normal", normalAttribute);
                }
                else
                {

                    var array = normalAttribute.Array;
                    if (normalArray == null) normalArray = new float[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        normalArray[i] = 0;
                    }
                }

                var normals = ((BufferAttribute<float>)attributes["normal"]).Array;

                int vA, vB, vC;
                vA = vB = vC = 0;

                Vector3 pA = new Vector3();
                Vector3 pB = new Vector3();
                Vector3 pC = new Vector3();
                Vector3 cb = new Vector3();
                Vector3 ab = new Vector3(); ;



                if (index != null)
                {
                    var indices = index.Array;

                    for (int i = 0; i < index.count; i += 3)
                    {
                        vA = indices[i + 0] * 3;
                        vB = indices[i + 1] * 3;
                        vC = indices[i + 2] * 3;

                        pA = Vector3.Zero().FromArray(positions, vA);
                        pB = Vector3.Zero().FromArray(positions, vB);
                        pC = Vector3.Zero().FromArray(positions, vC);

                        cb.SubVectors(pC, pB);
                        ab.SubVectors(pA, pB);

                        cb.Cross(ab);

                        normals[vA] += cb.X;
                        normals[vA + 1] += cb.Y;
                        normals[vA + 2] += cb.Z;

                        normals[vB] += cb.X;
                        normals[vB + 1] += cb.Y;
                        normals[vB + 2] += cb.Z;

                        normals[vC] += cb.X;
                        normals[vC + 1] += cb.Y;
                        normals[vC + 2] += cb.Z;
                    }
                }
                else
                {
                    var pLen = positions.Length-1;
                    for (int i = 0; i < positions.Length; i += 9)
                    {
                        pA = Vector3.Zero().FromArray(positions, i);
                        pB = Vector3.Zero().FromArray(positions, i + 3);
                        pC = Vector3.Zero().FromArray(positions, i + 6);

                        cb = pC - pB;
                        ab = pA - pB;
                        cb.Cross(ab);

                        normals[i] = cb.X;
                        normals[i + 1] = cb.Y;
                        normals[i + 2] = cb.Z;

                        normals[i + 3] = cb.X;
                        normals[i + 4] = cb.Y;
                        normals[i + 5] = cb.Z;

                        if ((i + 6) <= pLen)
                        {
                            normals[i + 6] =cb.X;
                            normals[i + 7] =cb.Y;
                            normals[i + 8] =cb.Z;
                        }

                    }
                }

                this.NormalizeNormals();

                normalAttribute.NeedsUpdate = true;
            }
        }

        public BufferGeometry Merge(BufferGeometry geometry, int offset)
        {
            if (offset == 0)
            {
                Trace.TraceWarning("THREE.Core.BufferGeometry.Merge():Overwriting original geometry, starting at offset=0. Use BufferGeomeryUtils.mergeBufferGeometries() for lossless merge.");
            }

            var attributes = this.Attributes;

            foreach (string key in attributes.Keys)
            {
                if (geometry.Attributes[key] == null) continue;

                BufferAttribute<float> attribute1 = (BufferAttribute<float>)attributes[key];
                var attributeArray1 = attribute1.Array;

                BufferAttribute<float> attribute2 = (BufferAttribute<float>)geometry.Attributes[key];
                var attributeArray2 = attribute2.Array;

                var attributeOffset = attribute2.ItemSize * offset;
                var length = System.Math.Min(attributeArray2.Length, attributeArray1.Length - attributeOffset);

                for (int i = 0,j=attributeOffset; i < length; i++,j++)
                {
                    attributeArray1[j] = attributeArray2[i];
                }
            }
            return this;
        }

        public void NormalizeNormals()
        {
            BufferAttribute<float> normals = (BufferAttribute<float>)this.Attributes["normal"];

            for (int i = 0; i < normals.count; i++)
            {
                _vector.X = normals.getX(i);
                _vector.Y = normals.getY(i);
                _vector.Z = normals.getZ(i);

                _vector.Normalize();

                normals.setXYZ(i, _vector.X, _vector.Y, _vector.Z);
            }

        }

        public new void ComputeFaceNormals()
        {
            //backwards compatibility
        }

        public BufferAttribute<float> ConvertBufferAttribute(BufferAttribute<float> attribute, int[] indices)
        {
            var array = attribute.Array;
            var itemSize = attribute.ItemSize;

            float[] array2 = new float[indices.Length * itemSize];

            int index = 0;
            int index2 = 0;

            for (int i = 0; i < indices.Length; i++)
            {
                index = indices[i] * itemSize;
                for (int j = 0; j < itemSize; j++)
                {
                    array2[index2++] = array[index++];
                }
            }

            return new BufferAttribute<float>(array2, itemSize);

        }
        public BufferGeometry ToNonIndexed()
        {
            if (this.Index == null)
            {
                Trace.TraceError("THREE.Core.BufferGeometry.ToNonIndexed:Geometry is already non-indexed.");
                return this;
            }

            var geometry2 = new BufferGeometry();

            var indices = this.Index.Array;
            var attributes = this.Attributes;

            foreach (string name in attributes.Keys)
            {
                BufferAttribute<float> attribute = (BufferAttribute<float>)attributes[name];

                BufferAttribute<float> newAttribute = ConvertBufferAttribute(attribute, indices);

                geometry2.SetAttribute(name, newAttribute );
            }

            var morphAttributes = this.MorphAttributes;

            foreach(string name in MorphAttributes.Keys)
            {
                List<BufferAttribute<float>> morphArray = new List<BufferAttribute<float>>();
                BufferAttribute<float>[] morphAttribute = (BufferAttribute<float>[])morphAttributes[name];

                for (int i = 0; i < morphAttribute.Length; i++)
                {
                    BufferAttribute<float> attribute = morphAttribute[i];
                    var newAttribute = ConvertBufferAttribute(attribute, indices);
                    morphArray.Add(newAttribute);
                }

                geometry2.MorphAttributes.Add(name,morphArray);
            }

            geometry2.MorphTargetsRelative = this.MorphTargetsRelative;

            var groups = this.Groups;

            for(int i=0;i<groups.Count;i++)
            {
                var group = groups[i];
                geometry2.AddGroup((int)group.Start,(int)group.Count,group.MaterialIndex);
            }

            return geometry2;
        }
        //TODO
        // aplyMatrix,

    }
}
