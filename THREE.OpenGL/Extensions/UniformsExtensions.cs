using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using THREE.Renderers.Shaders;

namespace THREE.OpenGL.Extensions
{
    [Serializable]
    public static class UniformsExtensions
    {
        public static GLUniform ToGLUniform(this Uniform uniform)
        {
            var glUniform = new GLUniform();
            foreach (var entry in uniform)
            {
                glUniform.Add(entry.Key, entry.Value);
            }

            return glUniform;
        }
        
        public static GLUniforms ToGLUniforms(this Uniforms uniforms)
        {
            var glUniforms = new GLUniforms();
            foreach (var entry in uniforms)
            {
                glUniforms.Add(entry.Key, (entry.Value as Uniform).ToGLUniform());
            }

            return glUniforms;
        }


    }
}
