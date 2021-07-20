using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Renderers.Shaders;

namespace THREE.Renderers.gl
{
    public class GLShader
    {
        public ShaderType Type;

        public string Code;

        public int Shader { get; set; }

        public string Name;

        public string VertexShader;
        
        public GLUniforms Uniforms;
        
        public string FragmentShader;

        public GLShader()
        {
        }
        public GLShader(ShaderType type, string code)
        {
            this.Type = type;

            this.Code = code;

            Shader = GL.CreateShader(type);

            GL.ShaderSource(Shader, Code);

            GL.CompileShader(Shader);
        }
    }
}
