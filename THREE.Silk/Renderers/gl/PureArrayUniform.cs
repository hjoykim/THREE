using Silk.NET.OpenGLES;
using System.Collections;
using System.Collections.Generic;
using Silk.NET.OpenGLES;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class PureArrayUniform : GLUniform,IPureArrayUniform
    {
        private GL gl;
        public int Size { get; set; } = 0;

        private Hashtable arrayCacheF32 = new Hashtable();

        private Hashtable arrayCacheI32 = new Hashtable();

        public PureArrayUniform()
        {
            UniformKind = "PureArrayUniform";
        }
        public PureArrayUniform(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public PureArrayUniform(GL gl,string id, UniformType type, int size, int addr) : this()
        {
            this.gl = gl;

            this.Id = id;

            this.Addr = addr;

            this.Size = size;

            this.UniformType = (int)type;
        }

        public void UpdateCache(object[] data)
        {
            this.Cache = data.ToList();
        }

        public void UpdateCache(List<object> data)
        {
            this.Cache = data;
        }

        public void SetValue(float[] v)
        {
            if (this.UniformType == (int)GLEnum.FloatVec4)
            {
                gl.Uniform4(this.Addr,(uint)this.Size, v);
            }
            else
            {
                gl.Uniform1(this.Addr, (uint)v.Length, v);
            }
        }

        public void SetValue(Vector2 v)
        {
            gl.Uniform2(this.Addr, v.X, v.Y);

        }

        public void SetValue(Vector3 v)
        {
            gl.Uniform3(this.Addr, v.X, v.Y, v.Z);
        }

        public void SetValue(Vector2[] v) // setValueV2fArray
        {
            int size = v.Length * 2;

            float[] r = new float[size];

            for (int i = 0, j = 0; i < size; i += 2, j++)
            {
                r[i] = v[j].X;
                r[i + 1] = v[j].Y;
            }
            gl.Uniform2(this.Addr, (uint)(size / 2), r);
        }

        public void SetValue(Vector3[] v)// setValueV3fArray
        {
            int size = v.Length * 3;

            float[] r = new float[size];

            for (int i = 0, j = 0; i < size; i += 3, j++)
            {
                r[i] = v[j].X;
                r[i + 1] = v[j].Y;
                r[i + 2] = v[j].Z;
            }

            gl.Uniform3(this.Addr,(uint) v.Length, r);
        }

        public void SetValue(Color[] v)
        {
            int size = v.Length * 3;

            float[] r = new float[size];

            for (int i = 0, j = 0; i < size; i += 3, j++)
            {
                Color cv = v[j];
                r[i] = cv.R;
                r[i + 1] = cv.G;
                r[i + 2] = cv.B;
            }

            gl.Uniform3(this.Addr, (uint)v.Length, r);
        }


        //setValueV4fArray
        public void SetValue(Vector4[] v)
        {
            int size = v.Length * 4;

            float[] r = new float[size];

            for (int i = 0, j = 0; i < size; i += 4, j++)
            {
                r[i] = v[j].X;
                r[i + 1] = v[j].Y;
                r[i + 2] = v[j].Z;
                r[i + 3] = v[j].W;
            }

            gl.Uniform4(this.Addr, (uint)v.Length, r);
        }

        // setValueM2Array
        //public void SetValue(Matrix2[] v) 
        //{
        //    List<float> data = new List<float>();

        //    for (int i = 0; i < v.Length; i++)
        //    {
        //        var r = v[i].Array().ToList();
        //        data.AddRange(r);
        //    }

        //    gl.UniformMatrix2(this.Addr, v.Length * 4,false, data.ToArray());
        //}

        // setValueM3Array
        public void SetValue(Matrix3[] v)
        {
            List<float> data = new List<float>();

            for (int i = 0; i < v.Length; i++)
            {
                var r = v[i].ToArray().ToList();
                data.AddRange(r);
            }

            gl.UniformMatrix3(this.Addr, (uint)v.Length, false, data.ToArray());
        }

        // setValueM4Array
        public void SetValue(Matrix4[] v)
        {
            List<float> data = new List<float>();

            for (int i = 0; i < v.Length; i++)
            {
                var r = v[i].ToArray().ToList();
                data.AddRange(r);
            }

            gl.UniformMatrix4(this.Addr, (uint)v.Length, false, data.ToArray());
        }

        private int[] AllocTextUnits(IGLTextures textures, int n)
        {
            int[] r = null;
            if (!arrayCacheI32.ContainsKey(n))
            {
                r = new int[n];
                arrayCacheI32.Add(n, r);
            }
            else
            {
                r = (int[])arrayCacheI32[n];
            }

            for (int i = 0; i != n; ++i)
            {
                r[i] = textures.AllocateTextureUnit();
            }

            return r;
        }

        // Array of textures(2D/Cube)
        //setValueT1Array
        public void SetValue(Texture[] v, IGLTextures textures)
        {
            int n = v.Length;

            var units = AllocTextUnits(textures, n);

            gl.Uniform1(this.Addr, (uint)n, units);


            for (int i = 0; i != n; ++i)
            {
                if (this.UniformType == (int)GLEnum.Sampler2D)
                    //setValueT1Array
                    textures.SafeSetTexture2D(v[i], units[i]);
                else if (this.UniformType == (int)GLEnum.SamplerCube)
                    //setValueT6Array
                    textures.SafeSetTextureCube(v[i], units[i]);
            }
        }

        public void SetValue(object v, IGLTextures textures = null)
        {
            if (v is Texture[] && textures != null)
            {
                SetValue((Texture[])v, textures);
            }
            else if (v is float[])
            {
                SetValue((float[])v);
            }
            else if (v is Vector2[])
            {
                SetValue((Vector2[])v);
            }
            else if (v is Vector3[])
            {
                SetValue((Vector3[])v);
            }
            else if (v is Vector4[])
            {
                SetValue((Vector4[])v);
            }
            //else if (v is Matrix2[])
            //{
            //    SetValue((Matrix2[])v);
            //}
            else if (v is Matrix3[])
            {
                SetValue((Matrix3[])v);
            }
            else if (v is Matrix4[])
            {
                SetValue((Matrix4[])v);
            }
            else if (v is Color[])
            {
                SetValue((Color[])v);
            }
            else if (v is List<Vector3>)
            {
                SetValue((v as List<Vector3>).ToArray());
            }
            else if (v is List<float>)
            {
                SetValue((v as List<float>).ToArray());
            }
            else
            {
                Trace.TraceWarning("PureArrayform.SetValue : Unknown uniformtype");
            }

        }
    }
}
