using System.Collections.Generic;

namespace THREE
{
   
    public class CurvePath : Curve
    {
        public List<Curve> Curves = new List<Curve>();

        public bool AutoClose = false;

        private List<float> cacheLengths;
        public CurvePath() : base()
        {

        }

        protected CurvePath(CurvePath source) : this()
        {
            
            this.Curves = new List<Curve>();

            for (int i = 0; i< source.Curves.Count; i++)
            {

                var curve = source.Curves[i];

                this.Curves.Add((Curve)curve.Clone());

            }

            this.AutoClose = source.AutoClose;
        }

        public new object Clone()
        {
            return new CurvePath(this);
        }

        public void Add(Curve curve)
        {
            Curves.Add(curve);
        }

        public void ClosePath()
        {
            var startPoint = this.Curves[0].GetPoint(0);
            var endPoint = this.Curves[this.Curves.Count - 1].GetPoint(1);

            if(!startPoint.Equals(endPoint))
            {
                this.Curves.Add(new LineCurve3(endPoint, startPoint));
            }
        }
        public override Vector3 GetPoint(float t, Vector3 optionalTarget = null)
        {
            var d = t * this.GetLength();
            var curveLengths = this.GetCurveLengths();
            var i = 0;

            // To think about boundaries points.

            while (i < curveLengths.Count)
            {

                if (curveLengths[i] >= d)
                {

                    var diff = curveLengths[i] - d;
                    var curve = this.Curves[i];

                    var segmentLength = curve.GetLength();
                    var u = segmentLength == 0 ? 0 : 1 - diff / segmentLength;



                    return curve.GetPointAt(u);

                }

                i++;

            }

            return null;
        }

        public override float GetLength()
        {
            var lens = this.GetCurveLengths();
            return lens[lens.Count-1];
        }

        public List<float> GetCurveLengths()
        {
            // We use cache values if curves and cache array are same length

            if (this.cacheLengths != null && this.cacheLengths.Count == this.Curves.Count)
            {

                return this.cacheLengths;

            }

            // Get length of sub-curve
            // Push sums into cached array

            var lengths = new List<float>();
            var sums = 0.0f;

            for (int i = 0;i<this.Curves.Count;i++)
            {

                sums += this.Curves[i].GetLength();
                lengths.Add(sums);

            }

            this.cacheLengths = lengths;

            return lengths;
        }

        public override List<Vector3> GetSpacedPoints(float? divisions=null)
        {
            if (divisions == null) divisions = 40;

            var points = new List<Vector3>();

            for (var i = 0; i <= divisions; i++)
            {

                points.Add(this.GetPoint(i / divisions.Value));

            }

            if (this.AutoClose)
            {

                points.Add(points[0]);

            }

            return points;
        }

        public override List<Vector3> GetPoints(float? divisions=null)
        {
            divisions = divisions != null ? divisions : 12;

            var points = new List<Vector3>();
            Vector3 last = null;

            for (int i = 0; i < Curves.Count; i++)
            {

                var curve = Curves[i];
                var resolution = (curve!=null && curve is EllipseCurve) ? divisions * 2
                    : (curve!=null && (curve is LineCurve || curve is LineCurve3)) ? 1
                        : (curve!=null && curve is SplineCurve) ? divisions * (curve as SplineCurve).Points.Count
                            : divisions;

                List<Vector3> pts = null;

                pts = curve.GetPoints(resolution);
                for (var j = 0; j < pts.Count; j++)
                {

                    var point = pts[j];

                    if (last!=null && last.Equals(point)) continue; // ensures no consecutive points are duplicates

                    points.Add(point);
                    last = point;

                }

            }

            if (this.AutoClose && points.Count > 1 && !points[points.Count - 1].Equals(points[0]))
            {

                points.Add(points[0]);

            }

            return points;
        }
        public override void UpdateArcLengths()
        {
            this.NeedsUpdate = true;

            this.cacheLengths = null;

            this.GetCurveLengths();
        }
    }
}
