using OpenTK.Graphics.ES30;
using System;
using System.Diagnostics;

namespace THREE
{
    public class SingleUniform : GLUniform
    {
        public SingleUniform()
        {
            UniformKind = "SingleUniform";
        }
        public SingleUniform(string id, ActiveUniformType type, int addr):this()
        {
            this.Id = id;

            this.Addr = addr;

            this.UniformType = type;
        }
        public void SetValue(int v)
        {
            GL.Uniform1(this.Addr, v);
        }
        public void SetValue(float v)
        {
            //if (Cache.Count>0 && v == (float)Cache[0]) return;

            GL.Uniform1(this.Addr, v);

            //this.Cache.Add(v);
        }

        public void SetValue(Vector2 v)
        {
            //if (Cache.Count > 0 && v.Equals((Vector2)Cache[0])) return;

            GL.Uniform2(this.Addr, v.X,v.Y);

            //this.Cache.Add(v);
        }

        public void SetValue(Vector3 v)
        {
            //if (Cache.Count > 0 && v.Equals((Vector3)Cache[0])) return;

            GL.Uniform3(this.Addr, v.X,v.Y,v.Z);

            //this.Cache.Add(v);
        }

        public void SetValue(Vector4 v)
        {
            //if (Cache.Count > 0 && v.Equals((Vector4)Cache[0])) return;

            GL.Uniform4(this.Addr, v.X, v.Y, v.Z, v.W);

            //this.Cache.Add(v);
        }

        //public void SetValue(Matrix2 matrix)
        //{
        //    if (Cache.Count > 0 && matrix.Equals((Matrix2)Cache[0])) return;

        //    GL.UniformMatrix2(this.Addr, false, ref matrix);

        //    this.Cache.Add(matrix);

        //}

        public void SetValue(Matrix3 matrix)
        {
            //if (Cache.Count > 0 && matrix.Equals((Matrix3)Cache[0])) return;

            GL.UniformMatrix3(this.Addr, 1,false, matrix.Elements);

            //this.Cache.Add(matrix);

        }

        public void SetValue(Matrix4 matrix)
        {
            //if (Cache.Count > 0 && matrix.Equals((Matrix4)Cache[0])) return;

            GL.UniformMatrix4(this.Addr, 1,false, matrix.Elements);

            //this.Cache.Add(matrix);

        }

        public void SetValue(Color color)
        {
            //if (Cache.Count > 0 && color.Equals((Color)Cache[0])) return;


            GL.Uniform3(this.Addr, color.R,color.G,color.B);

            //if (Cache.Count == 0) Cache.Add(color);
            //else Cache[0] = color;
            //this.Cache.Add(color);
        }

        public void SetValue(float[] color)
        {
            //float[] cachedColor = null;
            //if(Cache.Count>0)  cachedColor = (float[])Cache[0];
            //if (cachedColor!=null && cachedColor[0] == color[0] && cachedColor[1] == color[1] && cachedColor[2] == color[2]) return;

            GL.Uniform3(this.Addr, color[0],color[1],color[2]);

            //if (Cache.Count == 0) Cache.Add(color);
            //else Cache[0] = color;
            
        }
        public void SetValue(Texture v,GLTextures textures)
        {
            //var cache = this.Cache;
            var unit = textures.AllocateTextureUnit();

            //if (cache.Count == 0)
            //{
            //    GL.Uniform1(this.Addr,unit);
            //    this.Cache.Add(unit);
            //}
            //else{

            //    if((int)cache[0]!=unit)
            //    {
            //        GL.Uniform1(this.Addr, unit);
            //        this.Cache[0] = unit;
            //    }
            //}

            GL.Uniform1(this.Addr, unit);

            switch (this.UniformType)
            {
                case ActiveUniformType.Sampler2D:
                    textures.SafeSetTexture2D(v, unit);
                    break;
                case ActiveUniformType.Sampler3D :
                    textures.SetTexture3D(v, unit);
                    break;
                case ActiveUniformType.SamplerCube :
                    textures.SafeSetTextureCube(v,unit);
                    break;
                case ActiveUniformType.Sampler2DArray :
                    textures.SetTexture2DArray(v, unit);
                    break;

            }
           
        }
        public void SetValue(object v, GLTextures textures=null)
        {
            //Debug.WriteLine("SingleUniform, Id={0},Value={1}", this.Id, v);
            if (v is Texture && textures != null)
            {
                SetValue((Texture)v, textures);
            }
            else if(this.UniformType==ActiveUniformType.Int)
            {
                if (v is bool)
                {
                    bool value = Convert.ToBoolean(v);
                    SetValue(value == true ? 1 : 0);
                    this.UniformType = ActiveUniformType.Int;
                }
                else {
                    SetValue((int)v);
                }
            }
            else if (this.UniformType == ActiveUniformType.Bool)
            {
                bool value = Convert.ToBoolean(v);
                SetValue(value == true ? 1 : 0);
                this.UniformType = ActiveUniformType.Int;
            }
            else if (this.UniformType == ActiveUniformType.Float)
            {
                SetValue(Convert.ToSingle(v));
            }
            //else if (v is int)
            //{
            //    SetValue((int)v);
            //}
            //else if (v is float)
            //{
            //    SetValue((float)v);
            //}
            else if (v is Vector2)
            {
                SetValue((Vector2)v);
            }
            else if (v is Vector3)
            {
                SetValue((Vector3)v);
            }
            else if (v is Vector4)
            {
                SetValue((Vector4)v);
            }
            //else if (v is Matrix2)
            //{
            //    SetValue((Matrix2)v);
            //}
            else if (v is Matrix3)
            {
                SetValue((Matrix3)v);
            }
            else if (v is Matrix4)
            {
                SetValue((Matrix4)v);
            }
            else if (v is Color)
            {
                SetValue((Color)v);
            }
            else if (v is float[])
            {
                SetValue((float[])v);
            }
            else if (v is bool)
            {
                SetValue((bool)v == true ? 1 : 0);
            }
            else
            {
                Trace.TraceWarning("SingleUniform not defined value was access " + v.ToString());
            }

        }
    }

}
