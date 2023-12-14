using OpenTK.Graphics.ES30;
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
        public static void SetShaderType(this GLShader shader, ShaderType type, string code)
        {
            shader.Type = (int)type;

            shader.Code = code;

            shader.ShaderId = GL.CreateShader(type);

            GL.ShaderSource(shader.ShaderId, code);

            GL.CompileShader(shader.ShaderId);
        }
    }
}
