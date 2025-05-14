using Silk.NET.OpenGLES;
using System.Collections;
using System.Text.RegularExpressions;

namespace THREE
{
    [Serializable]
    public static class UniformsExtensions
    {

        public static GLUniforms Merge(this GLUniforms source,GLUniforms target)
        {
            foreach(var entry in target)
            {
                if(source.ContainsKey(entry.Key))
                {
                    //Trace.TraceWarning("key is already exist {0}. this key's value update to newer", entry.Key);
                    source[entry.Key] = entry.Value;
                }
                else
                {
                    source.Add(entry.Key, entry.Value);
                }
            }

            return source;
        }

        public static void LoadProgram(this GLUniforms source,GL gl,uint program)
        {
            source.Program = (int)program;

            int n = 0;

            gl.GetProgram((uint)source.Program, GLEnum.ActiveUniforms, out n);

            for (uint i = 0; i < n; i++)
            {
                int size;
                UniformType info;

                string name = gl.GetActiveUniform((uint)program, i, out size, out info);
                var addr = gl.GetUniformLocation((uint)program, name);
                ParseUniform(gl,name, info, size, addr, source);
            }
        }
        private static void ParseUniform(GL gl,string name, UniformType uniformType, int size, int addr, GLUniforms container)
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

                if (idIsIndex) idInt = string.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(id);
                //g.Value,g.Value.Length+m.Index);
                if (string.IsNullOrEmpty(subscript) || subscript.Equals("[") && groups[1].Value.Length + m.Index == (pathLength - 3))
                {
                    if (string.IsNullOrEmpty(subscript))
                        container.AddUniform(new SingleUniform(gl,id, uniformType, addr));
                    else
                        container.AddUniform(new PureArrayUniform(gl,id, uniformType, size, addr));

                    break;
                }
                else
                {
                    var map = container;
                    GLUniforms next = null;
                    if (!map.ContainsKey(id))
                    {
                        next = new GLUniforms(id);
                        container.AddUniform(next);
                    }
                    else
                    {
                        next = (GLUniforms)map[id];
                    }

                    container = next;

                }
            }
        }
        private static void AddUniform(this GLUniforms container,GLUniform uniformObject)
        {
            container.Seq.Add(uniformObject);
            container.Add(uniformObject.Id, uniformObject);
        }
        public static void SetProjectionMatrix(this GLUniforms source,GL gl, Matrix4 projMatrix)
        {
            if (source.ContainsKey("projectionMatrix"))
            {
                SingleUniform u = (SingleUniform)source["projectionMatrix"];
                gl.UniformMatrix4(u.Addr, (uint)1, false, projMatrix.Elements);
            }
        }

        public static void SetValue(this GLUniforms source,string name, object value, GLTextures textures = null)
        {
            if (source.ContainsKey(name))
            {
                GLUniform u = (GLUniform)source[name];
                if (u is SingleUniform)
                {
                    (u as SingleUniform).SetValue(value, textures);
                }
                else if (u is PureArrayUniform)
                {
                    (u as PureArrayUniform).SetValue(value, textures);

                }
                else
                {
                    (u as StructuredUniform).SetValue(value, textures);
                }
            }
        }

        public static void SetOptional(this GLUniforms source,Hashtable objects, string name)
        {
            if (objects.ContainsKey(name))
            {
                object value = objects[name];

                source.SetValue(name, value, null);
            }
        }

    }
}
