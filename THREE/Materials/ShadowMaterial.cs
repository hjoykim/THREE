using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class ShadowMaterial : Material
    {
        public ShadowMaterial()
        {
            this.type = "ShadowMaterial";

            this.Transparent = true;

            this.Color = new Color().SetHex(0x000000);
        }
        public ShadowMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
