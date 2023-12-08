
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class LineDashedMaterial : LineBasicMaterial
    {
        public float Scale = 1;

        public float DashSize = 3;

        public float GapSize = 1;

        public LineDashedMaterial() : base()
        {
            this.type = "LineDashedMaterial";
        }
        public LineDashedMaterial(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
