namespace THREE
{
    class InstancedInterleavedBuffer<T> : InterleavedBuffer<T>
    {
        public int MeshPerAttribute
        {
            get
            {
                return (int)this["meshPerAttribute"];
            }
            set
            {
                this["meshPerAttribute"] = value;
            }
        }

        public InstancedInterleavedBuffer()
        {
            this.Add("meshPerAttribute", 1);
        }
        public InstancedInterleavedBuffer(T[] array, int stride, int? meshPerAttribute = null) : this()
        {
            this.Array = array;
            this.Stride = stride;
            this.UpdateRange = new UpdateRange { Offset = 0, Count = -1 };
            this.MeshPerAttribute = meshPerAttribute!=null ? (int)meshPerAttribute : 1;
        }
    }
}
