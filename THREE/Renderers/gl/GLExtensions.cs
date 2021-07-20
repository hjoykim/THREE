using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.ES30;
namespace THREE.Renderers.gl
{
    public class GLExtensions
    {
        public List<string> ExtensionsName = new List<string>();
        public Dictionary<string, int> Extensions = new Dictionary<string, int>();
        public GLExtensions()
        {
            ExtensionsName = new List<string>((GL.GetString(StringName.Extensions)).Split(' '));
        }

        public int Get(string name)
        {
            int index = -1;

            int value;

            if (Extensions.TryGetValue(name,out value))
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
