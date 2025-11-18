using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class HemisphereLight : Light
    {
        public HemisphereLight(Color skyColor, Color groundColor, float? itensity = null)
            : base(skyColor, itensity)
        {
            this.CastShadow = false;

            this.Position.Copy(Object3D.DefaultUp);

            this.UpdateMatrix();

            this.GroundColor = groundColor;

            this.type = "HemisphereLight";
        }
        public HemisphereLight(int color, int gcolor, float? intensity) : this(Color.Hex(color), Color.Hex(gcolor), intensity) { }

        public HemisphereLight(SerializationInfo info, StreamingContext context) : base(info, context) { }
               
    }
}
