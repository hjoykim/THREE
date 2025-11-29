using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using THREE;

namespace THREE
{
    [Serializable]
    public class Object3D : BasicObject
    {
        public static Vector3 DefaultUp = new Vector3(0, 1, 0);


        private static int object3DIdCount;

        public int Id = object3DIdCount++;

        public string Uuid = Guid.NewGuid().ToString();

        public string Name = "";

        public Object3D? Parent = null;

        public List<Object3D> Children = new List<Object3D>();

        public Vector3 Up = DefaultUp.Clone();

        public Vector3 Position = Vector3.Zero();

        public Euler Rotation = new Euler();

        public Quaternion Quaternion = Quaternion.Identity();

        public Vector3 Scale = Vector3.One();

        public Matrix4 ModelViewMatrix = Matrix4.Identity();

        public Matrix3 NormalMatrix = new Matrix3();

        public Matrix4 Matrix = Matrix4.Identity();

        public Matrix4 MatrixWorld = Matrix4.Identity();

        public static bool DefaultMatrixAutoUpdate = true;

        public bool MatrixAutoUpdate = DefaultMatrixAutoUpdate;

        public bool MatrixWorldNeedsUpdate = false;

        public Layers Layers = new Layers();

        public bool Visible = true;

        public bool CastShadow = false;

        public bool ReceiveShadow = false;

        public bool FrustumCulled = true;

        public int RenderOrder = 0;

        public Dictionary<string, object> UserData = new Dictionary<string, object>();

        public string type = "Object3D";

        public virtual Geometry? Geometry { get { return null; } set {  } }
        public virtual Material? Material  { get { return null; } set {  } }
        public virtual List<Material>? Materials { get { return null; } set { } }

        public List<float> MorphTargetInfluences = new List<float>() { 0, 0, 0, 0, 0, 0, 0, 0 };

        public Hashtable MorphTargetDictionary = new Hashtable();

        public Action<IGLRenderer, Object3D, Camera, Geometry, Material, DrawRange?, GLRenderTarget>? OnBeforeRender;
        public Action<IGLRenderer, Object3D, Camera>? OnAfterRender;

        Vector3 _v1 = new Vector3();
        Quaternion _q1 = new Quaternion();
        Matrix4 _m1 = new Matrix4();
        Vector3 _target = new Vector3();

        Vector3 _position = new Vector3();
        Vector3 _scale = new Vector3();
        Quaternion _quaternion = new Quaternion();

        Vector3 _xAxis = new Vector3(1, 0, 0);
        Vector3 _yAxis = new Vector3(0, 1, 0);
        Vector3 _zAxis = new Vector3(0, 0, 1);

        public Object3D()
        {
            this.Rotation.PropertyChanged += OnRotationChanged;
            this.Quaternion.PropertyChanged += OnQuaternionChanged;
        }

        public Object3D(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        protected Object3D(Object3D source, bool recursive = true) : this()
        {
            this.Name = source.Name;

            this.Up.Copy(source.Up);

            this.Position.Copy(source.Position);
            this.Quaternion.Copy(source.Quaternion);
            this.Scale.Copy(source.Scale);

            this.Matrix.Copy(source.Matrix);
            this.MatrixWorld.Copy(source.MatrixWorld);

            this.MatrixAutoUpdate = source.MatrixAutoUpdate;
            this.MatrixWorldNeedsUpdate = source.MatrixWorldNeedsUpdate;

            this.Layers.Mask = source.Layers.Mask;
            this.Visible = source.Visible;

            this.CastShadow = source.CastShadow;
            this.ReceiveShadow = source.ReceiveShadow;

            this.FrustumCulled = source.FrustumCulled;
            this.RenderOrder = source.RenderOrder;

            this.UserData = source.UserData;
                       

            if (recursive == true)
            {
                for (var i = 0; i < source.Children.Count; i++)
                {

                    var child = source.Children[i];
                    this.Add((Object3D)child.Clone());
                }
            }

        }

        private void OnRotationChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Quaternion.SetFromEuler((sender as Euler), false);
        }

        private void OnQuaternionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Rotation.SetFromQuaternion((sender as Quaternion), null, false);
        }



        public void ApplyMatrix4(Matrix4 matrix)
        {
            if (this.MatrixAutoUpdate) this.UpdateMatrix();

            this.Matrix.PreMultiply(matrix);

            this.Matrix.Decompose(this.Position, this.Quaternion, this.Scale);

        }


        public Object3D ApplyQuaternion(Quaternion q)
        {
            this.Quaternion.PreMultiply(q);

            return this;
        }

        public void SetRotationFromAxisAngle(Vector3 axis, float angle)
        {
            this.Quaternion.SetFromAxisAngle(axis, angle);
        }

        public void SetRotationFromEuler(Euler euler)
        {
            this.Quaternion.SetFromEuler(euler, true);
        }

        public void SetRotationFromMatrix(Matrix4 m)
        {
            this.Quaternion.SetFromRotationMatrix(m);
        }

        public void SetRotationFromQuaternion(Quaternion q)
        {
            this.Quaternion.Copy(q);
        }

        public Object3D RotateOnAxis(Vector3 axis, float angle)
        {

            _q1.SetFromAxisAngle(axis, angle);

            this.Quaternion.Multiply(_q1);

            return this;
        }

        public Object3D RotateOnWorldAxis(Vector3 axis, float angle)
        {

            _q1.SetFromAxisAngle(axis, angle);

            this.Quaternion.PreMultiply(_q1);

            return this;
        }

        public Object3D RotateX(float angle)
        {
            return this.RotateOnAxis(_xAxis, angle);
        }

        public Object3D RotateY(float angle)
        {
            return this.RotateOnAxis(_yAxis, angle);
        }

        public Object3D RotateZ(float angle)
        {
            return this.RotateOnAxis(_zAxis, angle);
        }

        public Object3D TranslateOnAxis(Vector3 axis, float distance)
        {
            _v1.Copy(axis).ApplyQuaternion(this.Quaternion);

            this.Position.Add(_v1.MultiplyScalar(distance));

            return this;
        }

        public Object3D TranslateX(float distance)
        {
            return this.TranslateOnAxis(_xAxis, distance);
        }

        public Object3D TranslateY(float distance)
        {
            return this.TranslateOnAxis(_yAxis, distance);
        }

        public Object3D TranslateZ(float distance)
        {
            return this.TranslateOnAxis(_zAxis, distance);
        }

        public Vector3 LocalToWorld(Vector3 vector)
        {
            return vector.ApplyMatrix4(this.MatrixWorld);
        }

        public Vector3 WorldToLocal(Vector3 vector)
        {
            return vector.ApplyMatrix4(_m1.Copy(this.MatrixWorld).Invert());
        }


        public virtual void LookAt(float x, float y, float z)
        {
            Vector3 target = new Vector3(x, y, z);

            this.LookAt(target);

        }

        public virtual void LookAt(Vector3 target)
        {
            
            this.UpdateWorldMatrix(true, false);

            _position.SetFromMatrixPosition(this.MatrixWorld);


            if (this is Camera || this is Light)
                _m1 = _m1.LookAt(_position, target, this.Up);
            else
                _m1 = _m1.LookAt(target, _position, this.Up);


            this.Quaternion.SetFromRotationMatrix(_m1);

            if (this.Parent != null)
            {
                _m1.ExtractRotation(Parent.MatrixWorld);
                _q1.SetFromRotationMatrix(_m1);
                this.Quaternion.PreMultiply(_q1.Invert());

            }
        }


        public Object3D Add(Object3D object3D)
        {
            if (object3D == this)
            {
                Trace.TraceError("THREE.Core.Object3D.Add:", object3D, "can't be added as a child of itself");
                return this;
            }
            if (object3D is Object3D)
            {
                if (object3D.Parent != null)
                {
                    object3D.Parent.Remove(object3D);
                }

                object3D.Parent = this;

                this.Children.Add(object3D);
                object3D.DispatchEvent(new Event("added"));
            }
            else
            {
                Trace.TraceError("THREE.Core.Object3D.Add: {0} is not an instance of THREE.Core.Object3D.", object3D);
            }
            return this;
        }

        public Object3D Remove(Object3D object3D)
        {

            var index = this.Children.IndexOf(object3D);

            if (index != -1)
            {
                object3D.Parent = null;

                this.Children.RemoveAt(index);

                object3D.DispatchEvent(new Event("removed"));

            }
            return this;

        }

        public virtual Object3D Attach(Object3D object3D)
        {
            this.UpdateWorldMatrix(true, false);

            _m1.GetInverse(this.MatrixWorld);

            if (object3D.Parent != null)
            {
                object3D.Parent.UpdateWorldMatrix(true, false);

                _m1.Multiply(object3D.Parent.MatrixWorld);
            }

            object3D.ApplyMatrix4(_m1);

            object3D.UpdateWorldMatrix(false, false);

            this.Add(object3D);

            return this;
        }


        public Object3D GetObjectById(int id)
        {
            return GetObjectByProperty<int>("Id", id);
            //if (this.Id == id) return this;

            //for (int i = 0; i < this.Children.Count; i++)
            //{
            //    var child = this.Children[i];
            //    var object3D = child.GetObjectById(id);
            //    if (object3D != null)
            //        return object3D;
            //}
            //return null;
        }

        public Object3D GetObjectByName(string name)
        {
            return this.GetObjectByProperty("Name", name);
            //if (this.Name == name) return this;

            //for (int i = 0; i < this.Children.Count; i++)
            //{
            //    var child = this.Children[i];
            //    var object3D = child.GetObjectByName(name);
            //    if (object3D != null)
            //        return object3D;
            //}
            //return null;
        }

        private Object3D GetObjectByProperty<T>(string p, T value)
        {
            var property = this.GetType().GetField(p);
            if (property != null && property.GetValue(this)?.Equals(value) == true)
                return this;

            for (int i = 0; i < this.Children.Count; i++)
            {
                var child = this.Children[i];
                var object3D = child.GetObjectByProperty(p, value);
                if (object3D != null)
                    return object3D;
            }
            return null;
        }

        public Vector3 GetWorldPosition(Vector3 target)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateWorldMatrix(true, false);

            return target.SetFromMatrixPosition(this.MatrixWorld);
        }

        public virtual Quaternion GetWorldQuaternion(Quaternion target)
        {
            if (target == null)
            {
                target = new Quaternion();
            }

            this.UpdateWorldMatrix(true, false);

            this.MatrixWorld.Decompose(_position, target, _scale);

            return target;
        }

        public Vector3 GetWorldScale(Vector3 target = null)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateWorldMatrix(true, false);

            this.MatrixWorld.Decompose(_position, _quaternion, target);

            return target;
        }

        public virtual Vector3 GetWorldDirection(Vector3 target)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateWorldMatrix(true, false);

            var e = this.MatrixWorld.Elements;

            return target.Set(e[8], e[9], e[10]).Normalize();

        }

        public virtual void Raycast(Raycaster raycaster, List<Intersection> intersectionList)
        {
        }

        public void Traverse(Action<Object3D> callback)
        {

            callback(this);

            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].Traverse(callback);
            }
        }

        public void TraverseVisible(Action<Object3D> callback)
        {
            if (this.Visible == false) return;

            callback(this);

            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].TraverseVisible(callback);
            }
        }

        public void TraverseAncestors(Action<Object3D> callback)
        {
            var parent = this.Parent;

            if (parent != null)
            {
                callback(this);
                parent.TraverseAncestors(callback);
            }
        }

        public void UpdateMatrix()
        {
            this.Matrix.Compose(this.Position, this.Quaternion, this.Scale);

            this.MatrixWorldNeedsUpdate = true;
        }

        public virtual void UpdateMatrixWorld(bool force = false)
        {
            if (this.MatrixAutoUpdate)
                this.UpdateMatrix();

            if (this.MatrixWorldNeedsUpdate || force)
            {
                if (this.Parent == null)
                    this.MatrixWorld.Copy(this.Matrix);
                else
                    this.MatrixWorld.MultiplyMatrices(this.Parent.MatrixWorld, this.Matrix);

                this.MatrixWorldNeedsUpdate = false;
                force = true;
            }
            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].UpdateMatrixWorld(force);
            }
        }

        public virtual void UpdateWorldMatrix(bool updateParents, bool updateChildren)
        {
            if (updateParents == true && this.Parent != null)
                this.Parent.UpdateWorldMatrix(true, false);

            if (this.MatrixAutoUpdate) this.UpdateMatrix();

            if (this.Parent == null)
            {
                this.MatrixWorld.Copy(this.Matrix);
            }
            else
            {
                this.MatrixWorld.MultiplyMatrices(this.Parent.MatrixWorld, this.Matrix);
            }

            if (updateChildren == true)
            {
                for (var i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].UpdateWorldMatrix(false, true);
                }
            }
        }

        public override object Clone()
        {            
            //return new Object3D(this);
            return FastDeepCloner.DeepCloner.Clone(this);
        }
        public virtual object Copy(Object3D source, bool recursive = true)
        {
            return new Object3D(source, recursive);
        }
        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
