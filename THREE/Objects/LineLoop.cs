
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class LineLoop : Line
    {

        public LineLoop(Geometry geometry, Material material) : base(geometry, material)
        {
        }
        public LineLoop(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
