using OpenTK.Graphics.ES30;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace THREE
{
    public class PureArrayUniform : GLUniform
    {
        public int Size = 0;

        private Hashtable arrayCacheF32 = new Hashtable();

        private Hashtable arrayCacheI32 = new Hashtable();

        public PureArrayUniform()  
        {
            UniformKind = "PureArrayUniform";
        }

        public PureArrayUniform(string id, ActiveUniformType type, int size, int addr) :this()
        {
            this.Id = id;

            this.Addr = addr;

            this.Size = size;

            this.UniformType = type;        
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
            if (this.UniformType == ActiveUniformType.FloatVec4)
            {
                GL.Uniform4(this.Addr, this.Size, v);
            }
            else
            {
                GL.Uniform1(this.Addr, v.Length, v);
            }
        }

        public void SetValue(Vector2 v)
        {
            GL.Uniform2(this.Addr, v.X,v.Y);
           
        }

        public void SetValue(Vector3 v)
        {
            GL.Uniform3(this.Addr, v.X, v.Y, v.Z);
        }

        public void SetValue(Vector2[] v) // setValueV2fArray
        {
            int size = v.Length*2;

            float[] r = new float[size];

            for(int i=0,j=0;i<size;i+=2,j++)
            {
                r[i] = v[j].X;
                r[i+1] = v[j].Y;
            }
            GL.Uniform2(this.Addr, size/2, r);
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

            GL.Uniform3(this.Addr, v.Length, r);
        }
        
        public void SetValue(Color[] v)
        {
            int size = v.Length * 3;

            float[] r = new float[size];

            for (int i = 0,j=0; i < size; i += 3,j++)
            {
                Color cv = v[j];
                r[i] = cv.R;
                r[i + 1] = cv.G;
                r[i + 2] = cv.B;
            }

            GL.Uniform3(this.Addr, v.Length, r);
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

            GL.Uniform4(this.Addr, v.Length, r);
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

        //    GL.UniformMatrix2(this.Addr, v.Length * 4,false, data.ToArray());
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

            GL.UniformMatrix3(this.Addr, v.Length, false,data.ToArray());
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

            GL.UniformMatrix4(this.Addr, v.Length, false, data.ToArray());
        }

        private int[] AllocTextUnits(GLTextures textures, int n)
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
        public void SetValue(Texture[] v, GLTextures textures)
        {
            int n = v.Length;

            var units = AllocTextUnits(textures, n);

            GL.Uniform1(this.Addr, n, units);

           
            for (int i = 0; i != n; ++i)
            {
                if(this.UniformType==ActiveUniformType.Sampler2D) 
                    //setValueT1Array
                    textures.SafeSetTexture2D(v[i], units[i]);
                else if(this.UniformType==ActiveUniformType.SamplerCube)
                    //setValueT6Array
                    textures.SafeSetTextureCube(v[i], units[i]);
            }
        }

        public void SetValue(object v, GLTextures textures=null)
        {
            //Debug.WriteLine("PureArrayUniform, Id={0},Value={1}", this.Id, v);
            if (v is Texture[] && textures!=null)
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
            else if(v is List<float>)
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
