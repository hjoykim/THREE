using System.Collections.Generic;


namespace THREE
{
    public class Path : CurvePath
    {
        public Vector3 CurrentPoint = new Vector3();

        public Path(List<Vector3> points=null)
        {
            if(points!=null)
            {
                this.SetFromPoints(points);
            }
        }

		protected Path(Path source)
		{
			this.CurrentPoint.Copy(source.CurrentPoint);
		}

		public new object Clone()
		{
			return new Path(this);
		}
        public Path SetFromPoints(List<Vector3> points)
        {
            this.MoveTo(points[0].X, points[0].Y, points[0].Z);

            for (var i = 1;i< points.Count; i++)
            {

                this.LineTo(points[i].X, points[i].Y,points[i].Z);

            }

            return this;
        }

        public Path MoveTo(float x,float y,float z)
        {
            this.CurrentPoint.Set(x, y, z);

            return this;
        }
		public Path MoveTo(float x,float y)
        {
			this.CurrentPoint.Set(x, y, 0);
			return this;
        }
        public Path LineTo(float x, float y,float z )
        {

            var curve = new LineCurve3(this.CurrentPoint.Clone() as Vector3, new Vector3(x, y,z));
            this.Curves.Add(curve);

            this.CurrentPoint.Set(x, y,z);

            return this;
        }
		public Path LineTo(float x,float y)
        {
			var curve = new LineCurve3(this.CurrentPoint.Clone() as Vector3, new Vector3(x, y, 0));
			this.Curves.Add(curve);

			this.CurrentPoint.Set(x, y, 0);

			return this;
		}
		public Path QuadraticCurveTo(float aCPx, float aCPy, float aX, float aY )
		{

			var curve = new QuadraticBezierCurve3(
				this.CurrentPoint.Clone() as Vector3,
				new Vector3(aCPx, aCPy,0),
				new Vector3(aX, aY,0)
			);

			this.Curves.Add(curve);

			this.CurrentPoint.Set(aX, aY,0);

			return this;

		}

		public Path BezierCurveTo(float aCP1x, float aCP1y, float aCP2x, float aCP2y, float aX, float aY )
		{

			var curve = new CubicBezierCurve3(
				this.CurrentPoint.Clone() as Vector3,
				new Vector3(aCP1x, aCP1y,0),
				new Vector3(aCP2x, aCP2y,0),
				new Vector3(aX, aY,0)
			);

			this.Curves.Add(curve);

			this.CurrentPoint.Set(aX, aY,0);

			return this;

		}

		public Path SplineThru(List<Vector3> pts /*Array of Vector*/ )
		{

			var npts = new List<Vector3>() { this.CurrentPoint.Clone() as Vector3 };
			npts.AddRange(pts);
			//[this.currentPoint.clone()].concat(pts);

			var curve = new SplineCurve(npts);
			this.Curves.Add(curve);

			this.CurrentPoint.Copy(pts[pts.Count - 1]);

			return this;

		}

		public Path Arc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise )
		{

			var x0 = this.CurrentPoint.X;
			var y0 = this.CurrentPoint.Y;

			this.AbsArc(aX + x0, aY + y0, aRadius,
				aStartAngle, aEndAngle, aClockwise);

			return this;

		}

		public Path AbsArc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise )
		{

			this.AbsEllipse(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, aClockwise);

			return this;

		}

		public Path Ellipse(float aX, float aY, float xRadius, float yRadius, float aStartAngle, float aEndAngle, bool aClockwise, float aRotation )
		{

			var x0 = this.CurrentPoint.X;
			var y0 = this.CurrentPoint.Y;

			this.AbsEllipse(aX + x0, aY + y0, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);

			return this;

		}

		public Path AbsEllipse(float aX, float aY, float xRadius, float yRadius, float aStartAngle, float aEndAngle, bool aClockwise, float? aRotation=null )
		{

			var curve = new EllipseCurve(aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);

			if (this.Curves.Count > 0)
			{

				// if a previous curve is present, attempt to join
				var firstPoint = curve.GetPoint(0);

				if (!firstPoint.Equals(this.CurrentPoint))
				{

					this.LineTo(firstPoint.X, firstPoint.Y,0);

				}

			}

			this.Curves.Add(curve);

			var lastPoint = curve.GetPoint(1);
			this.CurrentPoint.Copy(lastPoint);

			return this;

		}

	}
}
