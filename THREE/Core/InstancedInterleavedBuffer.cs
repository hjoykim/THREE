using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class InstancedInterleavedBuffer<T> : InterleavedBuffer<T>
    {
        public int MeshPerAttribute { get; set; } = 1;
       

        public InstancedInterleavedBuffer()
        {
        }
       
        public InstancedInterleavedBuffer(T[] array, int stride, int? meshPerAttribute = null) : this()
        {
            this.Array = array;
            this.Stride = stride;
            this.UpdateRange = new UpdateRange { Offset = 0, Count = -1 };
            this.MeshPerAttribute = meshPerAttribute != null ? (int)meshPerAttribute : 1;
        }
    }
}
