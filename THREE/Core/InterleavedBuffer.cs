using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Linq;

namespace THREE
{
    public class InterleavedBuffer<T> : BufferAttribute<T>
    {
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


        public new int count
        {
            get
            {
                return this.Array != null ? this.Array.Length / Stride : 0;
            }
        }
           
        public InterleavedBuffer() : base()
        {

            this.Usage = (BufferUsageHint)Enum.ToObject(typeof(BufferUsageHint),Constants.StaticDrawUsage);
            this.Add("stride", 0);
        }


        public InterleavedBuffer(T[] array,int stride) : this()
        {
            this.Array = array;
            this.Stride = stride;
            this.UpdateRange = new UpdateRange { Offset = 0, Count = -1 };
            this.Type = typeof(T);
        }



        public InterleavedBuffer<T> CopyAt(int index1,InterleavedBuffer<T> attribute,int index2)
        {
            index1 *= this.Stride;
            index2 *= attribute.Stride;

            for(int i = 0, l = this.Stride; i < l; i++)
            {
                this.Array[index1 + i] = attribute.Array[index2 + i];
            }

            return this;
        }

        public InterleavedBuffer<T> Set(List<T> value,int offset)
        {
            List<T> list = new List<T>();
            list = this.Array.ToList();
            for(int i = offset; i < value.Count; i++)
            {
                list.Insert(i, value[i - offset]);
            }
            this.Array = list.ToArray();
            return this;
        }

    }
}
