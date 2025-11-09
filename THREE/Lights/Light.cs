using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class Light : Object3D
    {
        public Color Color;

        public Color GroundColor;

        public float Intensity;

        public float Distance;

        public float Angle;

        public float Exponent;

        public float Decay;

        public float Power { get; set; }

        public float Penumbra;

        public Object3D Target;

        public LightShadow Shadow;

        public SphericalHarmonics3 sh;

        //RectAreaLight

        public int Width;

        public int Height;

        public Light() : base()
        {
            this.IsLight = true;
        }
        public Light(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public Light(Color color, float? intensity = null) : base()
        {
            this.type = "Light";

            this.Color = color;

            this.Intensity = intensity != null ? intensity.Value : 1;

            this.ReceiveShadow = false;

            this.IsLight = true;

        }
        public Light(int color, float? intensity = null) : this(Color.Hex(color), intensity) { }


        public Light Copy(Light source)
        {
            base.Copy(source);
            this.Color = source.Color;
            this.Intensity = source.Intensity;
            this.Distance = source.Distance;
            this.Angle = source.Angle;
            this.Exponent = source.Exponent;
            this.Decay = source.Decay;
            this.Power = source.Power;
            this.Penumbra = source.Penumbra;
            this.Target = source.Target;
            this.Shadow = source.Shadow;
            this.sh = source.sh;
            this.Width = source.Width;
            this.Height = source.Height;
            this.IsLight = source.IsLight;
            this.ReceiveShadow = source.ReceiveShadow;

            return this;

        }
    }
}
