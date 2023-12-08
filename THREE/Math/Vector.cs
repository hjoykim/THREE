using System;

namespace THREE
{
    public abstract class Vector : ICloneable,IEquatable<Vector>
    {
        public abstract Vector SetComponent(int index, float value);

        public abstract float GetComponent(int index);

        public abstract Vector Set(float x, float y);

        public abstract Vector Set(float x, float y, float z);

        public abstract Vector Set(float x, float y, float z, float w);

        public abstract void SetScalar(float scalar);

        public abstract Vector Copy(Vector v);

        public abstract Vector Add(Vector v);

        public abstract Vector AddVectors(Vector a, Vector b);

        public abstract Vector AddScalar(float s);

        public abstract Vector AddScaledVector(Vector v, float s);

        public abstract Vector Sub(Vector v);

        public abstract Vector SubVectors(Vector a, Vector b);

        public abstract Vector MultiplyScalar(float s);

        public abstract Vector DivideScalar(float s);

        public abstract Vector Negate();

        public abstract float Dot(Vector v);

        public abstract float LengthSq();

        public abstract float Length();

        public abstract Vector Normalize();

        public abstract float DistanceTo(Vector v);

        public abstract float DistanceToSquared(Vector v);

        public abstract Vector ApplyMatrix3(Matrix3 m);

        public abstract Vector Min(Vector v);

        public abstract Vector Max(Vector v);

        public abstract Vector Clamp(Vector min, Vector max);

        public abstract Vector ClampScalar(float minVal, float maxVal);

        public abstract Vector ClampLength(float min, float max);

        public abstract Vector Floor();

        public abstract Vector Ceil();

        public abstract Vector Round();

        public abstract Vector RoundToZero();

        public abstract Vector SetLength(float length);

        public abstract Vector Lerp(Vector v, float alpha);

        public abstract object Clone();

        public abstract bool Equals(Vector other);
        
    }
}
