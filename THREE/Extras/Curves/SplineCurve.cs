using System.Collections.Generic;

namespace THREE
{
    public class SplineCurve : Curve
    {
        public List<Vector3> Points;

        public SplineCurve(List<Vector3> points=null) : base()
        {
            this.Points = points != null ? points : new List<Vector3>();
        }

        protected SplineCurve(SplineCurve source)
        {
            this.Points = new List<Vector3>();

            for(int i = 0; i < source.Points.Count; i++)
            {
                var point = source.Points[i];
                this.Points.Add((Vector3)point.Clone());
            }
        }

        public new object Clone()
        {
            return new SplineCurve(this);
        }
        public override Vector3 GetPoint(float t, Vector3 optionalTarget = null)
        {
            var point = optionalTarget != null ? optionalTarget : new Vector3();

            var points = this.Points;
            var p = (points.Count - 1) * t;

            var intPoint = (int)System.Math.Floor(p);
            var weight = p - intPoint;

            var p0 = points[intPoint == 0 ? intPoint : intPoint - 1];
            var p1 = points[intPoint];
            var p2 = points[intPoint > points.Count - 2 ? points.Count - 1 : intPoint + 1];
            var p3 = points[intPoint > points.Count - 3 ? points.Count - 1 : intPoint + 2];

            point.Set(
                Interpolations.CatmullRom(weight, p0.X, p1.X, p2.X, p3.X),
                Interpolations.CatmullRom(weight, p0.Y, p1.Y, p2.Y, p3.Y),
                0
            );

            return point;
        }
    }
}
