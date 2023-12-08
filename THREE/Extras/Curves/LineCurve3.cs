
namespace THREE
{
    public class LineCurve3 : Curve
    {
        public Vector3 V1;

        public Vector3 V2;

        public LineCurve3(Vector3 v1=null,Vector3 v2=null) : base()
        {
            V1 = v1 != null ? v1 : new Vector3();
            V2 = v2 != null ? v2 : new Vector3();
        }
        protected LineCurve3(LineCurve3 other)
        {
            this.V1.Copy(other.V1);
            this.V2.Copy(other.V2);

        }
        public new object Clone()
        {
            return new LineCurve3(this);
        }
        public override Vector3 GetPoint(float t,Vector3 optionalTarget=null)
        {
            var point = optionalTarget != null ? optionalTarget : new Vector3();

            if(t==1)
            {
                point.Copy(this.V2);
            }
            else
            {
                point.Copy(this.V2).Sub(this.V1);
                point.MultiplyScalar(t).Add(this.V1);
            }

            return point;
        }

        public override Vector3 GetPointAt(float u, Vector3 optionalTarget = null)
        {
            return this.GetPoint(u, optionalTarget);
        }
    }
}
