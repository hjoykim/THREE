using Silk.NET.OpenGLES;
using System.Collections.Generic;

namespace THREE
{
    [Serializable]
    public class GLExtensions
    {
        public List<string> ExtensionsName = new List<string>();
        public Dictionary<string, int> Extensions = new Dictionary<string, int>();
        public GL gl;
        public GLExtensions(GL gl)
        {
            this.gl = gl;
            unsafe
            {
                var v = gl.GetString(GLEnum.Extensions);
                string extensions = new string((char*)v);
                ExtensionsName = new List<string>(extensions.Split(' '));
            }
        }

        public int Get(string name)
        {
            int index = -1;

            int value;

            if (Extensions.TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                index = ExtensionsName.IndexOf(name);
                if (index >= 0)
                {
                    Extensions.Add(name, index);
                }
                return index;
            }
        }
    }
}
