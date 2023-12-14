using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.OpenGL.Extensions
{
    public class GLUniformsLoader
    {
        public static void Upload(List<GLUniform> Seq, GLUniforms values, GLTextures textures)
        {
            for (int i = 0, n = Seq.Count; i != n; ++i)
            {
                var u = Seq[i];
                object v = (values[u.Id] as GLUniform)["value"];
                if (v == null) continue;

                var property = v.GetType().GetProperty("NeedsUpdate");

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
            }
        }
        public static List<GLUniform> SeqWithValue(List<GLUniform> seq, GLUniforms values)
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
