using System;
using System.Linq;

namespace THREE
{
    public class Box2 : ICloneable
    {
        public Vector2 Max;

        public Vector2 Min;

        public Box2(Vector2 min = null, Vector2 max = null)
        {
            this.Min = ( min != null) ? min : new Vector2(float.PositiveInfinity,float.PositiveInfinity);

	        this.Max = ( max != null ) ? max : new Vector2(float.NegativeInfinity,float.NegativeInfinity );
        }

        protected Box2(Box2 source)
        {
            this.Min.Copy(source.Min);

            this.Max.Copy(source.Max);
        }

        public Box2 Set(Vector2 min, Vector2 max)
        {
            this.Min.Copy(min);

            this.Max.Copy(max);

            return this;
        }

        public Box2 SetFromPoints(Vector2[] points)
        {
            MakeEmpthy();

            for (int i = 0; i< points.Length; i++)
            {

                this.ExpandByPoint(points[i]);

            }

            return this;
        }

        public Box2 SetFromCenterAndSize(Vector2 center, Vector2 size)
        {
            Vector2 _vector = new Vector2();

            var halfSize = _vector.Copy(size).MultiplyScalar(0.5f);
            this.Min.Copy(center).Sub(halfSize);
            this.Max.Copy(center).Add(halfSize);

            return this;
        }

        public object Clone()
        {
            return new Box2(this);
        }

        public Box2 Copy(Box2 other)
        {
            this.Min.Copy(other.Min);
            this.Max.Copy(other.Max);

            return this;
        }

        public Box2 MakeEmpthy()
        {
            this.Min.X = this.Min.Y = float.PositiveInfinity;
            this.Max.X = this.Max.Y = float.NegativeInfinity;

            return this;
        }

        public bool IsEmpty()
        {
            return (this.Max.X < this.Min.X) || (this.Max.Y < this.Min.Y);
        }

        public Vector2 GetCenter(Vector2 target)
        {
             if(this.IsEmpty())
                 target.Set(0, 0);
             else
                 target.AddVectors(this.Min, this.Max).MultiplyScalar(0.5f);

             return target;
        }

        public Vector2 GetSize(Vector2 target)
        {
            if(IsEmpty())
                target.Set(0,0);
            else
                target.SubVectors(this.Max,this.Min);

            return target;
        }

        public Box2 ExpandByPoint(Vector2 point)
        {
            this.Min.Min(point);
            this.Max.Max(point);

            return this;
        }

        public Box2 ExpandByVector(Vector2 vector)
        {
            this.Min.Sub(vector);
            this.Max.Add(vector);

            return this;
        }

        public Box2 ExpandByScalar(float scalar)
        {
            this.Min.AddScalar(-scalar);
            this.Max.AddScalar(scalar);

            return this;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.X < this.Min.X || point.X > this.Max.X ||
            point.Y < this.Min.Y || point.Y > this.Max.Y ? false : true;
        }

        public bool ContainBox(Box2 box)
        {
            return this.Min.X <= box.Min.X && box.Max.X <= this.Max.X &&
            this.Min.Y <= box.Min.Y && box.Max.Y <= this.Max.Y;
        }

        public Vector2 GetParameter(Vector2 point,Vector2 target)
        {
            return target.Set(
            (point.X - this.Min.X) / (this.Max.X - this.Min.X),
            (point.Y - this.Min.Y) / (this.Max.Y - this.Min.Y)
        );
        }

        public bool IntersectsBox(Box2 box)
        {
            return box.Max.X < this.Min.X || box.Min.X > this.Max.X ||
            box.Max.Y < this.Min.Y || box.Min.Y > this.Max.Y ? false : true;
        }

        public Vector2 ClampPoint(Vector2 point, Vector2 target)
        {
            return target.Copy(point).Clamp(this.Min, this.Max);
        }

        public float DistanceToPoint(Vector2 point)
        {
            Vector2 _vector = new Vector2();
            var clampedPoint = _vector.Copy(point).Clamp(this.Min, this.Max);
            return clampedPoint.Sub(point).Length();
        }

        public Box2 Intersect(Box2 box)
        {
            this.Min.Max(box.Min);
            this.Max.Min(box.Max);

            return this;
        }

        public Box2 Union(Box2 box)
        {
            this.Min.Min(box.Min);
            this.Max.Max(box.Max);

            return this;
        }

        public Box2 Translate(Vector2 offset)
        {
            this.Min.Add(offset);
            this.Max.Add(offset);

            return this;
        }

        public bool Equals(Box2 box)
        {
            return box.Min.Equals(this.Min) && box.Max.Equals(this.Max);
        }
    }
}
