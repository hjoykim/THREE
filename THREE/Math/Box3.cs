using System;
using System.Collections.Generic;
using System.Linq;

namespace THREE
{
    public class Box3 : ICloneable
    {
        public Vector3 Min = new Vector3();
        public Vector3 Max = new Vector3();

        private Vector3 _vector = Vector3.Zero();

        private List<Vector3> _points = new List<Vector3>() {
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3()
        };

        // triangle centered vertices

        private Vector3 _v0 = new Vector3();
        private Vector3 _v1 = new Vector3();
        private Vector3 _v2 = new Vector3();

        // triangle edge vectors

        private Vector3 _f0 = new Vector3();
        private Vector3 _f1 = new Vector3();
        private Vector3 _f2 = new Vector3();

        private Vector3 _center = new Vector3();
        private Vector3 _extents = new Vector3();
        private Vector3 _triangleNormal = new Vector3();
        private Vector3 _testAxis = new Vector3();

        public Box3()
        {
            Min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        }

        public Box3(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }
        public Box3 Copy(Box3 box)
        {
            this.Min.Copy(box.Min);
            this.Max.Copy(box.Max);

            return this;
        }
        public void SetFromArray(float[] array)
        {
            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var minZ = float.PositiveInfinity;

            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;
            var maxZ = float.NegativeInfinity;

            for (int i = 0; i < array.Length; i += 3)
            {
                var x = array[i];
                var y = array[i + 1];
                var z = array[i + 2];

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (z > maxZ) maxZ = z;
            }
            this.Min.X = minX;
            this.Min.Y = minY;
            this.Min.Z = minZ;

            this.Max.X = maxX;
            this.Max.Y = maxY;
            this.Max.Z = maxZ;
        }

        public void SetFromBufferAttribute(BufferAttribute<float> attribute)
        {          

            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var minZ = float.PositiveInfinity;

            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;
            var maxZ = float.NegativeInfinity;

            for (int i = 0; i < attribute.count; i++)
            {
                var x = attribute.getX(i);
                var y = attribute.getY(i);
                var z = attribute.getZ(i);

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (z > maxZ) maxZ = z;

            }

            this.Min.X = minX;
            this.Min.Y = minY;
            this.Min.Z = minZ;

            this.Max.X = maxX;
            this.Max.Y = maxY;
            this.Max.Z = maxZ;
        }
        public void SetFromBufferAttribute(InterleavedBufferAttribute<float> attribute)
        {

            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var minZ = float.PositiveInfinity;

            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;
            var maxZ = float.NegativeInfinity;

            for (int i = 0; i < attribute.count; i++)
            {
                var x = attribute.getX(i);
                var y = attribute.getY(i);
                var z = attribute.getZ(i);

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (z < minZ) minZ = z;

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (z > maxZ) maxZ = z;

            }

            this.Min.X = minX;
            this.Min.Y = minY;
            this.Min.Z = minZ;

            this.Max.X = maxX;
            this.Max.Y = maxY;
            this.Max.Z = maxZ;
        }

        public Box3 SetFromPoints(List<Vector4> points)
        {
            this.MakeEmpty();
            for (int i = 0; i < points.Count; i++)
            {
                this.ExpandByPoint(points[i]);
            }
            return this;
        }
        public Box3 SetFromPoints(List<Vector3> points)
        {
            this.MakeEmpty();

            for (int i = 0; i < points.Count; i++)
            {
                this.ExpandByPoint(points[i]);
            }

            return this;
        }

        public Box3 SetFromCenterAndSize(Vector3 center, Vector3 size)
        {
            Vector3 HalfSize = _vector.Copy(size).MultiplyScalar(0.5f);

            this.Min.Copy(center).Sub(HalfSize);
            this.Max.Copy(center).Add(HalfSize);

            return this;
        }

        public Box3 SetFromObject(Object3D obj)
        {
            this.MakeEmpty();

            return this.ExpandByObject(obj);
        }

        public void MakeEmpty()
        {
            this.Min.X = this.Min.Y = this.Min.Z = float.PositiveInfinity;
            this.Max.X = this.Max.Y = this.Max.Z = float.NegativeInfinity;
        }

        public bool IsEmpty()
        {
            return (this.Max.X < this.Min.X) || (this.Max.Y < this.Min.Y) || (this.Max.Z < this.Min.Z);
        }

        public Vector3 GetCenter(Vector3 target)
        {
            if (this.IsEmpty())
                target = new Vector3(0, 0, 0);
            else
                target.AddVectors(this.Min, this.Max).MultiplyScalar(0.5f);

            return target;
        }

        public Vector3 GetSize()
        {
            return this.IsEmpty() ? Vector3.Zero() : this.Max - this.Min;
        }
        public Box3 ExpandByPoint(Vector4 point)
        {
            _vector.Set(point.X, point.Y, point.Z);
            this.Min.Min(_vector);
            this.Max.Max(_vector);
            return this;
        }
        public Box3 ExpandByPoint(Vector3 point)
        {
            this.Min.Min(point);
            this.Max.Max(point);

            return this;
        }

        public Box3 ExpandByVector(Vector3 vector)
        {
            this.Min = this.Min - vector;
            this.Max = this.Max - vector;

            return this;
        }

        public Box3 ExpandByScalar(float scalar)
        {
            this.Min = this.Min.AddScalar(-scalar);
            this.Max = this.Min.AddScalar(scalar);

            return this;
        }

        public Box3 ExpandByObject(Object3D object3D)
        {
            object3D.UpdateWorldMatrix(false, false);

            var geometry = object3D.Geometry as Geometry;

            if (geometry != null)
            {
                if (geometry.BoundingBox == null)
                {
                    geometry.ComputeBoundingBox();
                }
                Box3 _box = new Box3();
                _box.Copy(geometry.BoundingBox);
                _box.ApplyMatrix4(object3D.MatrixWorld);

                this.ExpandByPoint(_box.Min);
                this.ExpandByPoint(_box.Max);
            }

            var children = object3D.Children;

            for (int i = 0; i < children.Count; i++)
            {
                this.ExpandByObject(children[i]);
            }

            return this;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.X < this.Min.X || point.X > this.Max.X ||
            point.Y < this.Min.Y || point.Y > this.Max.Y ||
            point.Z < this.Min.Z || point.Z > this.Max.Z ? false : true;
        }

        public bool ContainsBox(Box3 box)
        {
            return this.Min.X <= box.Min.X && box.Max.X <= this.Max.X &&
            this.Min.Y <= box.Min.Y && box.Max.Y <= this.Max.Y &&
            this.Min.Z <= box.Min.Z && box.Max.Z <= this.Max.Z;
        }

        public Vector3 GetParameter(Vector3 point)
        {
            Vector3 target = Vector3.Zero();

            target.X = (point.X - this.Min.X) / (this.Max.X - this.Min.X);
            target.Y = (point.Y - this.Min.Y) / (this.Max.Y - this.Min.Y);
            target.Z = (point.Z - this.Min.Z) / (this.Max.Z - this.Min.Z);

            return target;
        }

        public bool IntersectsBox(Box3 box)
        {
            // using 6 splitting planes to rule out intersections.
            return box.Max.X < this.Min.X || box.Min.X > this.Max.X ||
                box.Max.Y < this.Min.Y || box.Min.Y > this.Max.Y ||
                box.Max.Z < this.Min.Z || box.Min.Z > this.Max.Z ? false : true;

        }

        public bool IntersectsSphere(Sphere sphere)
        {
            this.ClampPoint(sphere.Center, _vector);

            return _vector.DistanceToSquared(sphere.Center) <= (sphere.Radius * sphere.Radius);
        }

        public bool IntersectPlane(Plane plane)
        {
            return true;
        }
        public bool IntersectsTriangle(Triangle triangle)
        {

            if (this.IsEmpty())
            {

                return false;

            }

            // compute box center and extents
            this.GetCenter(_center);
            _extents.SubVectors(this.Max, _center);

            // translate triangle to aabb origin
            _v0.SubVectors(triangle.a, _center);
            _v1.SubVectors(triangle.b, _center);
            _v2.SubVectors(triangle.c, _center);

            // compute edge vectors for triangle
            _f0.SubVectors(_v1, _v0);
            _f1.SubVectors(_v2, _v1);
            _f2.SubVectors(_v0, _v2);

            // test against axes that are given by cross product combinations of the edges of the triangle and the edges of the aabb
            // make an axis testing of each of the 3 sides of the aabb against each of the 3 sides of the triangle = 9 axis of separation
            // axis_ij = u_i x f_j (u0, u1, u2 = face normals of aabb = x,y,z axes vectors since aabb is axis aligned)
            float[] axes = new float[]{
                0, -_f0.Z, _f0.Y, 0, -_f1.Z, _f1.Y, 0, -_f2.Z, _f2.Y,
                _f0.Z, 0, -_f0.X, _f1.Z, 0, -_f1.X, _f2.Z, 0, -_f2.X,
                -_f0.Y, _f0.X, 0, -_f1.Y, _f1.X, 0, -_f2.Y, _f2.X, 0
            };

            if (!SatForAxes(axes, _v0, _v1, _v2, _extents))
            {

                return false;

            }

            // test 3 face normals from the aabb
            axes = new float[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
            if (!SatForAxes(axes, _v0, _v1, _v2, _extents))
            {

                return false;

            }

            // finally testing the face normal of the triangle
            // use already existing triangle edge vectors here
            _triangleNormal.CrossVectors(_f0, _f1);
            axes = new float[]{ _triangleNormal.X, _triangleNormal.Y, _triangleNormal.Z };

            return SatForAxes(axes, _v0, _v1, _v2, _extents);

        }
        public Vector3 ClampPoint(Vector3 point,Vector3 target)
        {
            return target.Copy(point).Clamp(this.Min, this.Max);
        }

        public float DistanceToPoint(Vector3 point)
        {
            var clampedPoint = _vector.Copy(point).Clamp(this.Min, this.Max);

            return clampedPoint.Sub(point).Length();
        }

        public Sphere GetBoundingSphere(Sphere target)
        {
            target.Center = this.GetCenter(target.Center);
            target.Radius = this.GetSize().Length() * 0.5f;

            return target;
        }

        public Box3 Intersect(Box3 box)
        {
            this.Min.Max( box.Min);
            this.Max.Min(box.Max);

            if (this.IsEmpty()) this.MakeEmpty();

            return this;
        }

        public Box3 Union(Box3 box)
        {
            this.Min.Min(box.Min);
            this.Max.Max(box.Max);

            return this;
        }

        public Box3 ApplyMatrix4(Matrix4 matrix)
        {
            // transform of empty box is an empty box.
            if (this.IsEmpty()) return this;

            // NOTE: I am using a binary pattern to specify all 2^3 combinations below
            _points[0].Set(this.Min.X, this.Min.Y, this.Min.Z).ApplyMatrix4(matrix); // 000
            _points[1].Set(this.Min.X, this.Min.Y, this.Max.Z).ApplyMatrix4(matrix); // 001
            _points[2].Set(this.Min.X, this.Max.Y, this.Min.Z).ApplyMatrix4(matrix); // 010
            _points[3].Set(this.Min.X, this.Max.Y, this.Max.Z).ApplyMatrix4(matrix); // 011
            _points[4].Set(this.Max.X, this.Min.Y, this.Min.Z).ApplyMatrix4(matrix); // 100
            _points[5].Set(this.Max.X, this.Min.Y, this.Max.Z).ApplyMatrix4(matrix); // 101
            _points[6].Set(this.Max.X, this.Max.Y, this.Min.Z).ApplyMatrix4(matrix); // 110
            _points[7].Set(this.Max.X, this.Max.Y, this.Max.Z).ApplyMatrix4(matrix); // 111

            this.SetFromPoints(_points);

            return this;
        }
        public Box3 Translate(Vector3 offset)
        {
            this.Min = this.Min + offset;
            this.Max = this.Max + offset;

            return this;
        }
        protected Box3(Box3 source)
        {
            this.Min.Copy(source.Min);
            this.Max.Copy(source.Max);
            
        }
        public object Clone()
        {
            return new Box3(this);
        }

        public bool SatForAxes(float[] axes, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 extents )
        {

            int i, j;

            for (i = 0, j = axes.Length - 3; i <= j; i += 3)
            {

                _testAxis.FromArray(axes, i);
                // project the aabb onto the seperating axis
                var r = extents.X * System.Math.Abs(_testAxis.X) + extents.Y * System.Math.Abs(_testAxis.Y) + extents.Z * System.Math.Abs(_testAxis.Z);
                // project all 3 vertices of the triangle onto the seperating axis
                var p0 = v0.Dot(_testAxis);
                var p1 = v1.Dot(_testAxis);
                var p2 = v2.Dot(_testAxis);
                // actual test, basically see if either of the most extreme of the triangle points intersects r
                if (System.Math.Max(-System.Math.Max(p0, System.Math.Max(p1, p2)), System.Math.Min(p0, System.Math.Min(p1, p2))) > r)
                {

                    // points of the projected triangle are outside the projected half-length of the aabb
                    // the axis is seperating and we can exit
                    return false;

                }

            }

            return true;

        }
    }
}
