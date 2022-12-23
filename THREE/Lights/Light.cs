namespace THREE
{
    public class Light :Object3D
    {
        public Color Color;

        public Color GroundColor;

        public float Intensity;

        public float Distance;

        public float Angle;

        public float Exponent;

        public float Decay;

        public float Power {get;set;}
        
        public float Penumbra;

        public Object3D Target;

        public LightShadow Shadow;

        public SphericalHarmonics3 sh;

        //RectAreaLight

        public int Width;

        public int Height;

        public Light() :base()
        {
            this.IsLight = true;
        }

        public Light(Color color,float? intensity=null) : base()
        {
            this.type = "Light";
            
            this.Color = color;

            this.Intensity = intensity != null ? intensity.Value : 1;

            this.ReceiveShadow = false;

            this.IsLight = true;

        }
        public Light(int color, float? intensity = null) : this(Color.Hex(color), intensity) { }

        protected Light(Light other) : base(other)
        {
            this.type = "Light";

            this.Color = other.Color;

            this.Intensity = other.Intensity;

            this.IsLight = true;
        }
        public Light Copy(Light source)
        {
            return new Light(source);

        }
    }
}
