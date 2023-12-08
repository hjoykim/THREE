using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;


namespace THREE
{
    public class BufferType
    {
        public int buffer;

        public int Type;

        public int BytesPerElement;

        public int Version;

    }
    public class GLAttributes : Dictionary<object, object>
    {
        // buffers = this

        public GLAttributes()
        {
        }
        
        public BufferType CreateBuffer<T>(BufferAttribute<T> attribute, BufferTarget bufferType)
        {
            var array = attribute.Array;
            BufferUsageHint usage = attribute.Usage;

            int buffer;

            int type = (int)VertexAttribPointerType.Float;

            int bytePerElement = 4;

            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(bufferType, buffer);
           
            
            if (attribute.Type==typeof(float))
            {
                GL.BufferData(bufferType,(array.Length*sizeof(float)),array as float[],usage);               
                type = (int)VertexAttribPointerType.Float;
                bytePerElement = sizeof(float);
            }
            else if (attribute.Type == typeof(int))
            {
                GL.BufferData(bufferType, (array.Length * sizeof(int)), array as int[], usage);
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(int);
            }
            else if (attribute.Type==typeof(uint))
            {
                GL.BufferData(bufferType, (array.Length * sizeof(uint)), array as uint[], usage);
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(uint);
            }
            else if (attribute.Type == typeof(byte))
            {
                GL.BufferData(bufferType, (array.Length * sizeof(byte)), array as byte[], usage);
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(byte);
            }
            else
            {
                GL.BufferData(bufferType, (array.Length * 2), array as short[], usage);
                type = (int)VertexAttribPointerType.UnsignedShort;
                bytePerElement = sizeof(short);
            }
 

            return new BufferType { buffer = buffer, Type = type, BytesPerElement = bytePerElement, Version = attribute.Version };
        }

        public void UpdateBuffer<T>(int buffer, BufferAttribute<T> attribute, BufferTarget bufferType)
        {
            var array = attribute.Array;
            var updateRange = attribute.UpdateRange;

            GL.BindBuffer(bufferType, buffer);

            if (updateRange.Count == -1)
            {
                if (null != array as float[])
                {
                    GL.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(float), array as float[]);
                }
                else if (null != array as ushort[])
                {
                    GL.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(ushort), array as ushort[]);
                }
                else if (null != array as uint[])
                {
                    GL.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(uint), array as uint[]);
                }
                else if (null != array as byte[])
                {
                    GL.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(byte), array as byte[]);
                }
            }
            else
            {
                int length = updateRange.Offset + updateRange.Count;
                int startIndex = updateRange.Offset;

                T[] subarray = new T[length];

                Array.Copy(array, startIndex, subarray, 0, length);

                if (null != array as float[])
                {
                    GL.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(float)), length * sizeof(float), subarray as float[]);
                }
                else if (null != array as ushort[])
                {
                    GL.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(ushort)), length * sizeof(float), subarray as float[]);
                }
                else if (null != array as uint[])
                {
                    GL.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(uint)), length * sizeof(uint), subarray as uint[]);
                }
                else if (null != array as byte[])
                {
                    GL.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(byte)), length * sizeof(byte), subarray as byte[]);
                }

                attribute.UpdateRange.Count = -1;

            }
        }
                
        public BufferType Get<T>(object attribute)
        {
            //if(!this.ContainsKey(attribute))
            //{
            //    this.Add(attribute,new BufferType());
            //}
            if (attribute is InterleavedBufferAttribute<T>) attribute = (attribute as InterleavedBufferAttribute<T>).Data;


            return this.ContainsKey(attribute) ? (BufferType )this[attribute] : null;
        }

        //public void Remove(string attribute)
        //{
        //    this.Remove(attribute);
        //}
        public void UpdateBufferAttribute(GLBufferAttribute attribute,BufferTarget bufferType)
        {
            BufferType cached = this[attribute] as BufferType;
            if (cached != null || cached.Version < attribute.Version) {               
                this.Add(attribute, new BufferType { buffer = attribute.Buffer, Type = attribute.Type, BytesPerElement = attribute.ElementSize, Version = attribute.Version });
            }
        }
        public void Update<T>(BufferAttribute<T> attribute, BufferTarget bufferType)
        {
            if (attribute is InterleavedBufferAttribute<T>)       
                attribute = (attribute as InterleavedBufferAttribute<T>).Data;
            

            BufferType data = this.Get<T>(attribute);

            //if (!this.ContainsKey(attribute))
            //{
            //    this.Add(attribute,CreateBuffer(attribute,bufferType));
            //}
            if (data == null)
            {
                this.Add(attribute, CreateBuffer(attribute, bufferType));
            }
            else if(data.Version < attribute.Version)
            {
                UpdateBuffer<T>(data.buffer, attribute, bufferType);
                //BufferType data = (BufferType)this[attribute];
                //if (data.Version < attribute.Version)
                //{
                //    UpdateBuffer<T>(data.buffer, attribute, bufferType);
                //    data.Version = attribute.Version;
                //    this[attribute] = data;
                //}

            }
        }
    }
}
