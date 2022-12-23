using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace THREE
{
    public class GLUniforms : StructuredUniform 
    {
        public int Program = 0;

        public GLUniforms() : base()
        {
            UniformKind = "GLUniforms";
        }
        public GLUniforms(string id) : base(id)
        {
            UniformKind = "GLUniforms";
        }

        public static GLUniforms Copy(GLUniforms source)
        {
            var target = new GLUniforms();

            target.Program = source.Program;

            foreach (DictionaryEntry entry in source)
            {
                target.Add(entry.Key, (entry.Value as GLUniform).Copy(entry.Value as GLUniform));
            }

            return target;
        }

        public GLUniforms(int program)
        {
            this.Program = program;

            int n = 0;

            GL.GetProgram(this.Program, GetProgramParameterName.ActiveUniforms,out n);

            for (int i = 0; i < n; i++)
            {
                int size;
                ActiveUniformType info;

                string name = GL.GetActiveUniform(program,i,out size, out info);               
                var addr = GL.GetUniformLocation(program, name);

                //Debug.WriteLine("Uniform name {0}, position {1}", name, addr);
                
                ParseUniform(name, info, size,addr, this);
                //Debug.WriteLine("uniformName:" + name);
            }
               
        }

        private void AddUniform(GLUniforms container,GLUniform uniformObject)
        {
            container.Seq.Add(uniformObject);
            container.Add(uniformObject.Id, uniformObject);
        }

        public void ParseUniform(string name,ActiveUniformType uniformType, int size,int addr, GLUniforms container)
        {
            
            var path = name;
            var pathLength = name.Length;
            //var RePathPart = /([\w\d_]+)(\])?(\[|\.)?/g;
            var RePathPart = @"([\w\d_]+)(\])?(\[|\.)?";

            MatchCollection mc = Regex.Matches(path.Trim(), RePathPart, RegexOptions.None);

            foreach (Match m in mc)
            {
                GroupCollection groups = m.Groups;
                string id = groups[1].Value;
                int idInt = -1;
                bool idIsIndex = groups[2].Value.Equals("]");
                string subscript = groups[3].Value;
                
                if (idIsIndex) idInt = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(id);
                //g.Value,g.Value.Length+m.Index);
                if (string.IsNullOrEmpty(subscript) || subscript.Equals("[") && groups[1].Value.Length + m.Index == (pathLength-3))
                {
                    if(string.IsNullOrEmpty(subscript))
                        AddUniform(container,new SingleUniform(id, uniformType,addr));
                    else
                        AddUniform(container,new PureArrayUniform(id,uniformType,size,addr));

                    break;
                }
                else
                {
                    var map = container;
                    GLUniforms next = null;
                    if (!map.ContainsKey(id))
                    {
                        next = new GLUniforms(id);
                        AddUniform(container, next);
                    }
                    else
                    {
                        next = (GLUniforms)map[id];
                    }

                    container = next;

                }
            }
        }

        // When resizeing, it's always need to apply camera ProjectionMatrix
        public void SetProjectionMatrix(Matrix4 projMatrix)
        {
            if (this.ContainsKey("projectionMatrix"))
            {
                SingleUniform u = (SingleUniform)this["projectionMatrix"];
                GL.UniformMatrix4(u.Addr, 1, false, projMatrix.Elements);
            }
        }

        public void SetValue(string name, object value, GLTextures textures=null)
        {
            if (this.ContainsKey(name))
            {
                GLUniform u = (GLUniform)this[name];
                if (u is SingleUniform)
                {
                    (u as SingleUniform).SetValue(value, textures);
                }
                else if(u is PureArrayUniform)
                {
                    (u as PureArrayUniform).SetValue(value, textures);

                }
                else
                {
                    (u as StructuredUniform).SetValue(value, textures);
                }
            }
        }

        public void SetOptional(Hashtable objects,string name)
        {
            if (objects.ContainsKey(name))
            {
                object value = objects[name];

                this.SetValue(name, value, null);
            }
        }

        public static void Upload(List<GLUniform> Seq,Hashtable values,GLTextures textures)
        {
            for (int i = 0, n = Seq.Count; i != n; ++i)
            {
                var u = Seq[i];
                object v = (values[u.Id] as GLUniform)["value"];
                if (v == null) continue;
                var property = v.GetType().GetProperty("NeedsUpdate");
                //Debug.WriteLine("{0}:{1}",u.Id,v.ToString());
                //if (u.Id.Equals("flipEnvMap") || u.Id.Equals("maxMipLevel"))
                //{

                //    Debug.WriteLine("ambientLightColor {0}", v);
                //}
                //if (property != null && (bool)property.GetValue(v, null) != false)
                //{
                if (u.UniformKind.Equals("SingleUniform"))
                {
                
                    (u as SingleUniform).SetValue(v, textures);

                    ErrorCode error = GL.GetError();

                    if (error == ErrorCode.InvalidOperation)
                    {
                        Debug.WriteLine(error.ToString());
                    }
                }
                else if (u.UniformKind.Equals("PureArrayUniform"))
                {
                    (u as PureArrayUniform).SetValue(v, textures);
                    ErrorCode error = GL.GetError();
                    if (error == ErrorCode.InvalidOperation)
                    {
                        Debug.WriteLine(error.ToString());
                    }
                }
                else
                {

                    (u as StructuredUniform).SetValue(v, textures);
                    ErrorCode error = GL.GetError();
                    if (error == ErrorCode.InvalidOperation)
                    {
                        Debug.WriteLine(error.ToString());
                    }
                }
                //}


            }
        }

        public static List<GLUniform> SeqWithValue(List<GLUniform> seq,GLUniforms values)
        {
            List<GLUniform> r = new List<GLUniform>();

            for (int i = 0, n = seq.Count; i != n; ++i)
            {
                var u = seq[i];
                if (values.ContainsKey(u.Id))
                    r.Add(u);
            }
            return r;
        }        
    }
}
