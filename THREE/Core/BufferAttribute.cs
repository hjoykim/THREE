using OpenTK.Graphics.ES30;
using System;
using THREE;

namespace THREE
{
    public struct UpdateRange
    {
        public int Offset;

        public int Count;
    }

    public class BufferAttribute<T> : GLAttribute,IBufferAttribute    {

        public BufferUsageHint Usage;

        public UpdateRange UpdateRange;

        public int Version = 0;

        public T[] Array
        {
            get
            {
                return (T[])this["array"];
            }
            set
            {
                this["array"] = value;
            }
        }

        public Type Type
        {
            get 
            { 
                return (Type)this["type"]; 
            }
            set
            {
                this["type"] = value;
            }
        }
        public int ItemSize
        {
            get
            {
                return (int)this["itemSize"];
            }
            set
            {
                this["itemSize"] = value;
            }
        }

        public int Buffer
        {
            get
            {
                return (int)this["buffer"];
            }
            set
            {
                this["buffer"] = value;
            }
        }

        public bool NeedsUpdate
        {
            get
            {
                return (bool)this["needsUpdate"];
            }
            set
            {
                this.Version++;
                this["needsUpdate"] = value;
            }
        }

        public int count
        {
            get
            {
                return Array.Length / ItemSize;
            }
        }

        public bool Normalized
        {
            get
            {
                return (bool)this["normalized"];
            }
            set
            {
                this["normalized"] = value;
            }
        }
        public BufferAttribute()
        {
            this.Add("array", null);
            this.Add("itemSize", -1);
            this.Add("buffer", -1);
            this.Add("needsUpdate", false);
            this.Add("type", null);
            this.Add("normalized", false);

            this.Usage = BufferUsageHint.StaticDraw;

            this.UpdateRange = new UpdateRange { Offset = 0, Count = -1 };
        }

        public BufferAttribute(T[] array, int itemSize,bool? normalized=null) 
            : this()
        {
            this.Name = "";
            this.Array = array;
            this.ItemSize = itemSize;
            this.Type = typeof(T);

            this.Normalized = normalized != null && normalized.Value==true ? true : false;
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

        public void SetUsage(BufferUsageHint hint)
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

            return this;
        }

        public BufferAttribute<T> CopyAt(int index1,BufferAttribute<T> attribute,int index2)
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

                this["array"] = array;
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

                this["array"] = array;
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

            if(array is null)
            {
                array = new float[vectors.Length * 3];

                this["array"] = array;                
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

                this["array"] = array;
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

        public BufferAttribute<T> Set(float[] array, int offset=0)
        {
            array.CopyTo(this.Array, offset);

            return this;
        }

        public T getX(int index)
        {
            return this.Array[index * this.ItemSize];
        }

        public void setX(int index, T x)
        {
            this.Array[index * this.ItemSize] = x;
        }

        public T getY(int index)
        {
            return this.Array[index * this.ItemSize+1];
        }

        public void setY(int index, T y)
        {
            this.Array[index * this.ItemSize+1] = y;
        }

        public T getZ(int index)
        {
            return this.Array[index * this.ItemSize+2];
        }

        public void setZ(int index, T z)
        {
            this.Array[index * this.ItemSize+2] = z;
        }

        public T getW(int index)
        {
            return this.Array[index * this.ItemSize+3];
        }

        public void setW(int index, T w)
        {
            this.Array[index * this.ItemSize+3] = w;
        }

        public void setXY(int index, T x, T y)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
        }

        public void setXYZ(int index, T x, T y,T z)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
        }

        public void setXYZW(int index, T x, T y, T z,T w)
        {
            index *= this.ItemSize;
            this.Array[index + 0] = x;
            this.Array[index + 1] = y;
            this.Array[index + 2] = z;
            this.Array[index + 3] = w;
        }
    }
}
