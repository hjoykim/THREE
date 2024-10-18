using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class PointLight : Light
    {
        public new float Power
        {
            get
            {
                return (float)(this.Intensity * 4 * System.Math.PI);
            }
            set
            {
                this.Intensity = (float)(value / (4 * System.Math.PI));
            }
        }

        public PointLight() : base() 
        {
            this.Distance = 0;
            this.Decay =  1;
            this.Shadow = new PointLightShadow();
            this.type = "PointLight";
        }
        public PointLight(Color color, float? intensity = null, float? distance = null, float? decay = null)
            : base(color, intensity)
        {
            this.Distance = distance != null ? (float)distance : 0;
            this.Decay = decay != null ? (float)decay : 1;

            this.Shadow = new PointLightShadow();

            this.type = "PointLight";
        }

        public PointLight(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public PointLight(int color, float? intensity = null, float? distance = null, float? decay = null) : this(Color.Hex(color), intensity, distance, decay) { }
    }
}
