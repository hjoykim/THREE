
namespace THREE
{
    public class Line3
    {
        public Vector3 Start = Vector3.Zero();

        public Vector3 End = Vector3.Zero();

        public Line3()
        {
        }

        public Line3(Vector3 start, Vector3 end)
        {
            this.Start = start;

            this.End = end;
        }

        public void Set(Vector3 start, Vector3 end)
        {
            this.Start = start;

            this.End = end;
        }

        public Vector3 GetCenter()
        {
            return (this.Start + this.End) * 0.5f;
        }

        public Vector3 Delta(Vector3 target=null)
        {
            if (target == null)
            {
                target = new Vector3();
            }
            return target.SubVectors(this.End,this.Start);
        }

        public float DistanceSq()
        {
            return this.Start.DistanceToSquared(this.End);
        }

        public float Distance()
        {
            return this.Start.DistanceTo(this.End);
        }

        public Vector3 At(float t)
        {
            return (this.Delta() * t) + this.Start;
        }

        public float ClosestPointToPointParameter(Vector3 point, bool clampToLine=false)
        {
            var startP = point - this.Start;
            var startEnd = this.End - this.Start;

            var startEnd2 = Vector3.Dot(startEnd, startEnd);
            var startEnd_startP = Vector3.Dot(startEnd, startP);

            var t = startEnd_startP / startEnd2;

            if (clampToLine == true)
            {
                t = t.Clamp(0, 1);
            }
            return t;
        }

        public Vector3 ClosestPointToPoint(Vector3 point, bool clampToLine=false,Vector3 target=null)
        {
            var t = this.ClosestPointToPointParameter(point, clampToLine);

            if (target == null)
            {
                target = new Vector3();
            }
            return this.Delta(target).MultiplyScalar(t).Add(this.Start);
        }

        public void ApplyMatrix4(Matrix4 matrix)
        {
            this.Start.ApplyMatrix4(matrix);
            this.End.ApplyMatrix4(matrix);
        }

        public override bool Equals(object obj)
        {
            Line3 line = obj as Line3;

            return line.Start.Equals(this.Start) && line.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
