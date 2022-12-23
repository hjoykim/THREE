using System;
using System.Collections.Generic;

namespace THREE
{
    public class Sphere : ICloneable
    {
        public Vector3 Center = new Vector3();

        public float Radius = 0;

        public Sphere()
        {
        }

        public Sphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        protected Sphere(Sphere other)
        {
            Center = other.Center;
            Radius = other.Radius;

        }

        public Sphere Copy(Sphere sphere)
        {
            this.Center.Copy(sphere.Center);
            this.Radius = sphere.Radius;

            return this;
        }
        public Sphere Set(Vector3 center,float radius)
        {
            Center.Copy(center);
            Radius = radius;
            return this;
        }
        public Sphere SetFromPoints(List<Vector3> points, Vector3 optionalCenter = null)
        {
            Vector3 center = this.Center;

            var box = new Box3();

            if (optionalCenter != null)
            {
                center.Copy(optionalCenter);
            }
            else
            {
                center = box.SetFromPoints(points).GetCenter(center); ;
            }

            float maxRadiusSq = 0;

            for (int i = 0; i < points.Count; i++)
            {
                maxRadiusSq = System.Math.Max(maxRadiusSq, center.DistanceToSquared(points[i]));
            }

            this.Radius = (float)System.Math.Sqrt(maxRadiusSq);

            return this;
        }

        public bool IsEmpty()
        {
            return this.Radius <= 0;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.DistanceToSquared(this.Center) <= (this.Radius * this.Radius);
        }

        public float DistanceToPoint(Vector3 point)
        {
            return point.DistanceTo(this.Center) - this.Radius;
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            var radiusSum = this.Radius + sphere.Radius;

            return sphere.Center.DistanceToSquared(this.Center) <= (radiusSum * radiusSum);
        }

        public bool IntersectBox(Box3 box)
        {
            return box.IntersectsSphere(this);
        }

        public bool IntersectsPlane(Plane plane)
        {
            return true;
            // return System.Math.Abs(plane.DistanceToPoint(this.Center))<=this.Radius;
        }

        public Vector3 ClampPoint(Vector3 point)
        {
            float deltaLengthSq = this.Center.DistanceToSquared(point);

            Vector3 target = point;

            if (deltaLengthSq > (this.Radius * this.Radius))
            {
                target.Sub(this.Center).Normalize();
                target.MultiplyScalar(this.Radius).Add(this.Center);
            }

            return target;
        }

        public Box3 GetBoundingBox()
        {
            Box3 target = new Box3(this.Center,this.Center);

            target.ExpandByScalar(this.Radius);

            return target;
        }

        public Sphere ApplyMatrix4(Matrix4 matrix)
        {
            this.Center = this.Center.ApplyMatrix4(matrix);
            this.Radius = this.Radius * matrix.GetMaxScaleOnAxis();

            return this;
        }

        public Sphere Translate(Vector3 offset)
        {
            this.Center = this.Center + offset;

            return this;
        }

        public override bool Equals(object obj)
        {
            Sphere sphere = obj as Sphere;
            return sphere.Center.Equals(this.Center) && (sphere.Radius == this.Radius);
        }

        public object Clone()
        {
            return new Sphere(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
               
    }
}
