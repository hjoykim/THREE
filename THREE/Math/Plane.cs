using System;

namespace THREE
{
    public class Plane : ICloneable
    {
        public Vector3 Normal = new Vector3(1,0,0);
        public float Constant = 0;

        private Vector3 _vector1 = new Vector3();

        public Plane()
        {
        }
        protected Plane(Plane other)
        {
            this.Normal = other.Normal;
            this.Constant = other.Constant;
        }

        public object Clone()
        {
            return new Plane(this);
        }
        public Plane Copy(Plane source)
        {
            this.Normal.Copy(source.Normal);
            this.Constant = source.Constant;
            return this;
        }
        public Plane(Vector3 normal, float constant)
        {
            this.Normal = normal;
            this.Constant = constant;
        }

        public Plane Set(Vector3 normal, float constant)
        {
            this.Normal = normal;
            this.Constant = constant;

            return this;
        }

        public Plane SetComponents(float x, float y, float z, float w)
        {
            this.Normal = new Vector3(x, y, z);
            this.Constant = w;

            return this;
        }

        public Plane SetFromNormalAndCoplanarPoint(Vector3 normal, Vector3 point)
        {
            this.Normal = normal;
            this.Constant = -Vector3.Dot(point, this.Normal);

            return this;
        }
        private Vector3 _vector2 = new Vector3();

        public Plane SetFromCoplanarPoints(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 normal = _vector1.SubVectors(c, b).Cross(_vector2.SubVectors(a, b)).Normalize();

            this.SetFromNormalAndCoplanarPoint(normal, a);

            return this;
        }

        public Plane Normalize()
        {
            var inverseNormalLength = 1.0f / this.Normal.Length();
            this.Normal.MultiplyScalar(inverseNormalLength);
            this.Constant *= inverseNormalLength;

            return this;
        }

        public Plane Negate()
        {
            this.Constant *= -1;
            this.Normal = this.Normal.Negate();

            return this;
        }

        public float DistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(this.Normal, point) + this.Constant;
        }

        public float DistanceToSphere(Sphere sphere)
        {
            return this.DistanceToPoint(sphere.Center) - sphere.Radius;
        }

        public Vector3 ProjectPoint(Vector3 point)
        {
            return this.Normal * (-this.DistanceToPoint(point)) + point;
        }

        public bool IntersectLine(Line line)
        {
            throw new NotImplementedException();
        }

        public bool IntersectsLine(Line line)
        {
            throw new NotImplementedException();
        }

        public bool IntersectsBox(Box3 box)
        {
            return box.IntersectPlane(this);
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            return sphere.IntersectsPlane(this);
        }

        public Vector3 CoplanarPoint()
        {
            return this.Normal * (-this.Constant);
        }

        public Plane ApplyMatrix4(Matrix4 matrix, Matrix3 optionalNormalMatrix = null)
        {
            Matrix3 normalMatrix = new Matrix3();

            if (optionalNormalMatrix != null)
                normalMatrix = (Matrix3)optionalNormalMatrix;
            else
                normalMatrix = normalMatrix.GetNormalMatrix(matrix);

            Vector3 referencePoint = this.CoplanarPoint().ApplyMatrix4(matrix);

            Vector3 normal = this.Normal.ApplyMatrix3(normalMatrix).Normalize();

            this.Constant = -Vector3.Dot(referencePoint, normal);

            return this;
        }

        public Plane translate(Vector3 offset)
        {
            this.Constant -= Vector3.Dot(offset, Normal);

            return this;
        }

        public override bool Equals(object obj)
        {
            Plane plane = obj as Plane;
            return plane.Normal.Equals(this.Normal) && (plane.Constant == this.Constant);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
