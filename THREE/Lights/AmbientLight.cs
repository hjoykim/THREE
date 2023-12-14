using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class AmbientLight : Light
    {
        public AmbientLight(Color color, float? intensity = null)
            : base(color, intensity)
        {
            this.type = "AmbientLight";
        }
        public AmbientLight(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public AmbientLight(int color, float? intensity = null) : this(Color.Hex(color), intensity) { }

    }
}