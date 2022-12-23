namespace THREE
{
    public class QuadraticBezierCurve3 : Curve
    {
        public Vector3 V0, V1, V2;

        public QuadraticBezierCurve3(Vector3 v0 = null, Vector3 v1 = null, Vector3 v2 = null, Vector3 v3 = null)
        {
            this.V0 = v0 != null ? v0 : new Vector3();
            this.V1 = v1 != null ? v1 : new Vector3();
            this.V2 = v2 != null ? v2 : new Vector3();
        }

        protected QuadraticBezierCurve3(QuadraticBezierCurve3 source)
        {
            this.V0.Copy(source.V0);
            this.V1.Copy(source.V1);
            this.V2.Copy(source.V2);
        }
        public new object Clone()
        {
            return new QuadraticBezierCurve3(this);
        }
        public override Vector3 GetPoint(float t, Vector3 optionalTarget = null)
        {
            var point = optionalTarget != null ? optionalTarget : new Vector3();


            point.Set(
                Interpolations.QuadraticBezier(t, V0.X, V1.X, V2.X),
                Interpolations.QuadraticBezier(t, V0.Y, V1.Y, V2.Y),
                Interpolations.QuadraticBezier(t, V0.Z, V1.Z, V2.Z)
            );

            return point;
        }
    }
}
