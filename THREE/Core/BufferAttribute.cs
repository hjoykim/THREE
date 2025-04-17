using System;
using System.Reflection;
using System.Runtime.Serialization;
using THREE;

namespace THREE
{
    [Serializable]
    public struct UpdateRange
    {
        public int Offset;

        public int Count;
    }

    [Serializable]
    public class BufferAttribute<T> : IBufferAttribute
    {
        public string Name { get; set; }
        public int Usage { get; set; }// BufferUsageHint Usage;

        public UpdateRange UpdateRange;

        public int Version = 0;

        public T[] Array { get; set; }
       
        
        public Type Type { get; set; }

        public int ItemSize { get; set; } = -1;


        public int Buffer { get; set; } = -1;

        private bool needsUpdate = false;
        public bool NeedsUpdate
        {
            get
            {
                return needsUpdate;
            }
            set
            {
                this.Version++;
                needsUpdate = value;
            }
        }

        public int count
        {
            get
            {
                return Array.Length / ItemSize;
            }
        }



        public bool Normalized { get; set; } = false;
        
        public BufferAttribute()
        {          
            this.Usage = Constants.StaticDrawUsage;;

            this.UpdateRange = new UpdateRange { Offset = 0, Count = -1 };
        }       

        public BufferAttribute(T[] array, int itemSize, bool? normalized = null)
            : this()
        {
            this.Name = "";
            this.Array = array;
            this.ItemSize = itemSize;
            this.Type = typeof(T);

            this.Normalized = normalized != null && normalized.Value == true ? true : false;
        }
        protected BufferAttribute(BufferAttribute<T> source)
        {
            Copy(source);
        }

        public int Length
        {
            get
            {
                return this.Array.Length;
            }
        }

        public void SetUsage(int hint)
        {
            this.Usage = hint;
        }

        public BufferAttribute<T> Clone()
        {
            return new BufferAttribute<T>(this);
        }
        public BufferAttribute<T> Copy(BufferAttribute<T> source)
        {
            this.Name = source.Name;
            if (source.Array != null)
            {
                this.Array = new T[source.Array.Length];
                source.Array.CopyTo(this.Array, 0);
            }
            this.ItemSize = source.ItemSize;
            this.Normalized = source.Normalized;
            this.Usage = source.Usage;
            this.Type = typeof(T);
            return this;
        }

        public BufferAttribute<T> CopyAt(int index1, BufferAttribute<T> attribute, int index2)
        {
            index1 *= this.ItemSize;
            index2 *= attribute.ItemSize;

            for (int i = 0; i < this.ItemSize; i++)
            {
                this.Array[index1 + i] = attribute.Array[index2 + i];
            }

            return this;
        }

        public BufferAttribute<T> CopyArray(T[] array)
        {
            this.Array = array;

            return this;
        }

        public BufferAttribute<T> CopyVector2sArray(Vector2[] vectors)
        {
            var array = this.Array as float[];
            if (array is null)
            {
                array = new float[vectors.Length * 2];

               Array = array as T[];
            }
            int offset = 0;

            for (int i = 0; i < vectors.Length; i++)
            {
                var vector = vectors[i];

                array[offset++] = vector.X;
                array[offset++] = vector.Y;
            }

            return this;
        }
        public BufferAttribute<T> CopyColorsArray(Color[] colors)
        {
            var array = this.Array as float[];
            //color.R / 255.0f, color.G / 255.0f, color.B / 255.0f
            if (array is null)
            {
                array = new float[colors.Length * 3];

                Array = array as T[];
            }
            int offset = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                var color = colors[i];

                array[offset++] = color.R;
                array[offset++] = color.G;
                array[offset++] = color.B;
            }

            return this;

        }

        public BufferAttribute<T> CopyVector3sArray(Vector3[] vectors)
        {
            var array = this.Array as float[];

            if (array is null)
            {
                array = new float[vectors.Length * 3];

                Array = array as T[];
            }

            int offset = 0;

            for (int i = 0; i < vectors.Length; i++)
            {
                var vector = vectors[i];

                array[offset++] = vector.X;
                array[offset++] = vector.Y;
                array[offset++] = vector.Z;
            }

            return this;
        }

        public BufferAttribute<T> CopyVector4sArray(Vector4[] vectors)
        {
            var array = this.Array as float[];
            if (array is null)
            {
                array = new float[vectors.Length * 4];

                Array = array as T[];
            }
            int offset = 0;

            for (int i = 0; i < vectors.Length; i++)
            {
                var vector = vectors[i];

                array[offset++] = vector.X;
                array[offset++] = vector.Y;
                array[offset++] = vector.Z;
                array[offset++] = vector.W;
            }

            return this;
        }

        public BufferAttribute<T> Set(float[] array, int offset = 0)
        {
            array.CopyTo(this.Array, offset);

            return this;
        }

        public T GetX(int index)
        {
            return this.Array[index * this.ItemSize];
        }

        public BufferAttribute<T> SetX(int index, T x)
        {
            this.Array[index * this.ItemSize] = x;
            return this;
        }

        public T GetY(int index)
        {
            return this.Array[index * this.ItemSize + 1];
        }

        public BufferAttribute<T> SetY(int index, T y)
        {
            this.Array[index * this.ItemSize + 1] = y;
            return this;
        }

        public T GetZ(int index)
        {
            return this.Array[index * this.ItemSize + 2];
        }

        public BufferAttribute<T> SetZ(int index, T z)
        {
            this.Array[index * this.ItemSize + 2] = z;
            return this;
        }

        public T GetW(int index)
        {
            return this.Array[index * this.ItemSize + 3];
        }

        public BufferAttribute<T> SetW(int index, T w)
        {
            this.Array[index * this.ItemSize + 3] = w;
            return this;
        }

        public BufferAttribute<T> SetXY(int index, T x, T y)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
            return this;
        }

        public BufferAttribute<T> SetXYZ(int index, T x, T y, T z)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
            return this;
        }

        public BufferAttribute<T> SetXYZW(int index, T x, T y, T z, T w)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
            this.Array[index + 3] = w;
            return this;
        }       

        public object Getter(int k, int index)
        {
            switch(k)
            {
                case 0: return GetX(index);
                case 1: return GetY(index);
                case 2: return GetZ(index);
                case 3: return GetW(index);
                default: return 0;
            }
        }

        public void Setter(int k, int index, object value)
        {
            switch (k)
            {
                case 0: SetX(index,(T)value);break;
                case 1: SetY(index, (T)value); break;
                case 2: SetZ(index, (T)value); break;
                case 3: SetW(index, (T)value); break;
                default: return ;
            }
        }
    }
}
