using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace THREE
{
    [Serializable]
    public class UniformsUtils
    {
        public static GLUniforms CloneUniforms(GLUniforms src)
        {
            return src.Copy(src);
        }

        public static GLUniforms Merge(List<GLUniforms> uniforms)
        {
            var merged = new GLUniforms();

            foreach (var uniform in uniforms)
            {
                foreach (var entry in uniform)
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
