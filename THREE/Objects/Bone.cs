
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class Bone : Object3D
    {
        public Bone() : base()
        {
            this.type = "Bone";
        }
        public Bone(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
