
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class SpriteMaterial : Material
    {

        //public bool SizeAttenuation = true;

        public SpriteMaterial()
        {
            this.type = "SpriteMaterial";

            this.Color = new Color().SetHex(0x000000);

            this.Transparent = true;

            this.Map = null;

            this.AlphaMap = null;

            this.Rotation = 0;

            this.SizeAttenuation = true;

            this.Transparent = true;

        }
        public SpriteMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
