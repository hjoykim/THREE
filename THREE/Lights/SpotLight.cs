namespace THREE
{
    public class SpotLight : Light
    {
        public new float Power
        {
            get
            {
                return (float)(this.Intensity*System.Math.PI);
            }
            set
            {
                this.Intensity = (float)(value/System.Math.PI);
            }
        }

        public SpotLight(Color color, float? intensity = null,float? distance=null,float? angle=null,float? penumbra=null,float? decay=null)
            : base(color, intensity)
        {
            this.Position.Copy(Object3D.DefaultUp);
            this.UpdateMatrix();

            this.Target = new Object3D();

            this.Distance = distance != null ? (float)distance : 0;

            this.Angle = angle != null ? (float)angle : (float)(System.Math.PI / 3);

            this.Penumbra = penumbra != null ? (float)penumbra : 0;

            this.Decay = decay != null ? (float)decay : 1;

            this.Shadow = new SpotLightShadow();

            this.type = "SpotLight";
        }
        public SpotLight(int color, float? intensity = null, float? distance = null, float? angle = null, float? penumbra = null, float? decay = null) :
          this(Color.Hex(color), intensity, distance, angle, penumbra, decay) { }
        protected SpotLight(SpotLight other) : base(other)
        {
            this.Distance = other.Distance;

            this.Angle = other.Angle;

            this.Penumbra = other.Penumbra;

            this.Decay = other.Decay;

            this.Target = other.Target;

            this.Shadow = (SpotLightShadow)other.Shadow.Clone();

            this.type = "SpotLight";
        }

    }
}
