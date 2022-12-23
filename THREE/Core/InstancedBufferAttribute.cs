namespace THREE
{
    public class InstancedBufferAttribute<T> : BufferAttribute<T>
    {
        public int MeshPerAttribute = 1;
  

        public InstancedBufferAttribute()
        {
            
        }

        public InstancedBufferAttribute(T[] array, int itemSize, bool? normalized=null, int? meshPerAttribute=null)
            :base(array,itemSize,normalized)
        {
            
        }
        protected InstancedBufferAttribute(InstancedBufferAttribute<T> source) : this(source.Array,source.ItemSize,source.Normalized,source.MeshPerAttribute)
        {

        }
        public InstancedBufferAttribute<T> Clone()
        {
            return new InstancedBufferAttribute<T>(this);
        }
        public InstancedBufferAttribute<T> Copy(InstancedBufferAttribute<T> source)
        {
            return new InstancedBufferAttribute<T>(source);
        }

    }
}
