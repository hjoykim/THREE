using THREE;

namespace THREE
{
    [Serializable]
    public class GLObject
    {
        public long Id { get; set; }

        public BaseGeometry buffer {get;set; }

        public Object3D object3D { get; set; }

        public Material material { get; set; }

        public float z { get; set; }

        public bool render { get; set; }
    }
}
