
namespace THREE
{
    public class InterleavedBufferAttribute<T> : BufferAttribute<T>
    {
        public int Offset
        {
            get
            {
                return (int)this["offset"];
            }
            set
            {
                this["offset"] = value;
            }
        }

        public int Stride
        {
            get
            {
                return (int)this["stride"];
            }
            set
            {
                this["stride"] = value;
            }
        }
        public InterleavedBuffer<T> Data
        {
            get
            {
                return (InterleavedBuffer<T>)this["data"];
            }
            set
            {
                this["data"] = value;
            }
        }

        public new T[] Array
        {
            get
            {
                return (this.Data as BufferAttribute<T>).Array;
            }
            
        }

        public new int count
        {
            get
            {
                return (Data as BufferAttribute<T>).count;
            }
        }
       
        public InterleavedBufferAttribute() :base()
        {
            this.Add("offset", 0);
            this.Add("data", null);
        }

        public InterleavedBufferAttribute(InterleavedBuffer<T> interleavedBuffer, int itemSize, int offset, bool normalized) : this()
        {
            this.Data = interleavedBuffer;
            this.ItemSize = itemSize;
            this.Offset = offset;
            this.Normalized = normalized==true;
        }
        public InterleavedBufferAttribute<T> SetX(int index, T x )
        {

            this.Data.Array[index * this.Data.Stride + this.Offset] = x;

            return this;

        }
        public InterleavedBufferAttribute<T> SetY(int index, T y)
        {

            this.Data.Array[index * this.Data.Stride + this.Offset+1] = y;

            return this;

        }
        public InterleavedBufferAttribute<T> SetZ(int index, T z)
        {

            this.Data.Array[index * this.Data.Stride + this.Offset+2] = z;

            return this;

        }
        public InterleavedBufferAttribute<T> SetW(int index, T w)
        {

            this.Data.Array[index * this.Data.Stride + this.Offset + 3] = w;

            return this;

        }

        public T GetX(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset];

        }

        public T GetY(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset+1];

        }

        public T GetZ(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset + 2];

        }

        public T GetW(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset + 3];

        }

        public InterleavedBufferAttribute<T> SetXY(int index, T x,T y)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index+0] = x;
            this.Data.Array[index + 1] = y;

            return this;

        }
        public InterleavedBufferAttribute<T> SetXYZ(int index, T x, T y,T z)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index + 0] = x;
            this.Data.Array[index + 1] = y;
            this.Data.Array[index + 2] = z;

            return this;

        }

        public InterleavedBufferAttribute<T> SetXYZW(int index, T x, T y, T z,T w)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index + 0] = x;
            this.Data.Array[index + 1] = y;
            this.Data.Array[index + 2] = z;
            this.Data.Array[index + 3] = w;

            return this;

        }
    }
}
