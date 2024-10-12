using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    [Serializable]
    public static class ShaderExtension
    {
        public static void SetShaderType(this GLShader shader, GL gl,ShaderType type, string code)
        {
            shader.Type = (int)type;

            shader.Code = code;

            uint shaderId =gl.CreateShader(type);

            gl.ShaderSource(shaderId, code);

            gl.CompileShader(shaderId);

            shader.ShaderId = (int)shaderId;
        }
    }
}
