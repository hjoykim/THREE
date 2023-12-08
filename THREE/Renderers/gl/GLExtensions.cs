using System.Collections.Generic;
using OpenTK.Graphics.ES30;
namespace THREE
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
