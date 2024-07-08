using OpenTK.Graphics.ES30;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace THREE
{
    [Serializable]
    public class GLProperties
    {
        private Dictionary<object, Hashtable> Properties = new Dictionary<object, Hashtable>();

        public GLProperties()
        {
        }

        public Hashtable Get(object obj)
        {
            Hashtable map = new Hashtable(); ;
            if (!Properties.ContainsKey(obj))
            //if (!Properties.TryGetValue(obj, out map))
            {                
                Properties.Add(obj, map);
            }
            else
            {
                map = Properties[obj];
            }
            return map;
        }

        public void Remove(object obj)
        {
            Properties.Remove(obj);
        }

        public void Update(object obj, object key, object value)
        {
            Hashtable map;
            if (!Properties.TryGetValue(obj, out map))
            {
                map[key] = value;
            }
        }

        public void Dispose()
        {
            foreach (var entry in Properties)
            {
                if(entry.Key is Texture)
                {
                    Hashtable hashtable = (Hashtable)entry.Value;
                    foreach (DictionaryEntry entry1 in hashtable)
                    {
                        if(entry1.Key.Equals("glFramebuffer") || entry1.Key.Equals("glDepthbuffer"))
                        {
                            object value = hashtable[entry1.Key];
                            if (value is int)
                            {
                                GL.DeleteFramebuffer((int)value);
                            }
                            if(value is int[])
                            {
                                int[] buffers = (int[])value;
                                GL.DeleteFramebuffers(buffers.Length,buffers);
                            }
                        }
                        if(entry1.Key.Equals("glTexture"))
                        {
                            GL.DeleteTexture((int)hashtable[entry1.Key]);
                        }

                    }
                }
            }
            Properties = new Dictionary<object, Hashtable>();
        }
    }
}
