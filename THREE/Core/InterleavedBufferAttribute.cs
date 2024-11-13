
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
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
                return this.Data.Array;
            }

        }

        public new int count
        {
            get
            {
                return Data.count;
            }
        }

        public InterleavedBufferAttribute() : base()
        {
            this.Add("offset", 0);
            this.Add("data", null);
        }
        public InterleavedBufferAttribute(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public InterleavedBufferAttribute(InterleavedBuffer<T> interleavedBuffer, int itemSize, int offset, bool normalized=false) : this()
        {
            this.Data = interleavedBuffer;
            this.ItemSize = itemSize;
            this.Offset = offset;
            this.Normalized = normalized == true;
        }

        public new InterleavedBufferAttribute<T> SetY(int index, T y)
        {
            this.Data.Array[index * this.Data.Stride + this.Offset + 1] = y;
            return this;
        }
        public new InterleavedBufferAttribute<T> SetZ(int index, T z)
        {
            this.Data.Array[index * this.Data.Stride + this.Offset + 2] = z;

            return this;

        }
        public new InterleavedBufferAttribute<T> SetW(int index, T w)
        {

            this.Data.Array[index * this.Data.Stride + this.Offset + 3] = w;

            return this;

        }

        public new T GetX(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset];

        }

        public new T GetY(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset + 1];

        }

        public new T GetZ(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset + 2];

        }

        public new T GetW(int index)
        {

            return this.Data.Array[index * this.Data.Stride + this.Offset + 3];

        }

        public new InterleavedBufferAttribute<T> SetXY(int index, T x, T y)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index + 0] = x;
            this.Data.Array[index + 1] = y;

            return this;

        }
        public new InterleavedBufferAttribute<T> SetXYZ(int index, T x, T y, T z)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index + 0] = x;
            this.Data.Array[index + 1] = y;
            this.Data.Array[index + 2] = z;

            return this;

        }

        public new InterleavedBufferAttribute<T> SetXYZW(int index, T x, T y, T z, T w)
        {
            index = index * this.Data.Stride + this.Offset;
            this.Data.Array[index + 0] = x;
            this.Data.Array[index + 1] = y;
            this.Data.Array[index + 2] = z;
            this.Data.Array[index + 3] = w;

            return this;

        }
        public InterleavedBufferAttribute<T> ApplyMatrix4(Matrix4 m)
        {
            Vector3 _vector = new Vector3();
            for (int i = 0, l = this.Data.count; i < l; i++)
            {

                _vector.X = (float)(object)this.GetX(i);
                _vector.Y = (float)(object)this.GetY(i);
                _vector.Z = (float)(object)this.GetZ(i);

                _vector.ApplyMatrix4(m);

                this.SetXYZ(i, (T)(object)_vector.X, (T)(object)_vector.Y, (T)(object)_vector.Z);

            }

            return this;

        }
    }
}
