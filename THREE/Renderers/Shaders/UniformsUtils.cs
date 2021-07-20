using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Renderers.gl;

namespace THREE.Renderers.Shaders
{
    public class UniformsUtils
    {
        public static GLUniforms CloneUniforms(GLUniforms src)
        {
            return GLUniforms.Copy(src);
        }

        public static GLUniforms Merge(List<GLUniforms> uniforms)
        {
            var merged = new GLUniforms();

            foreach (var uniform in uniforms)
            {
                foreach (DictionaryEntry entry in uniform)
                {
                    if (merged.ContainsKey(entry.Key))
                    {
                        Trace.TraceWarning("key is already exist. this key skipped");
                        merged[entry.Key] = entry.Value;
                    }
                    else
                    {
                        merged.Add(entry.Key, entry.Value);
                    }
                }
            }
            return merged;
        }
    }
}
