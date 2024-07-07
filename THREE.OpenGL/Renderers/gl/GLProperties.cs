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
            Debug.WriteLine("Properties Disposing");
            foreach (var entry in Properties)
            {
                Debug.WriteLine("###"+entry.Key.GetType().Name);
                if(entry.Key is Texture)
                {
                    Hashtable hashtable = (Hashtable)entry.Value;
                    foreach (DictionaryEntry entry1 in hashtable)
                    {
                        Debug.WriteLine(entry1.Key);
                        if(entry1.Key.Equals("glFramebuffer") || entry1.Key.Equals("glDepthbuffer"))
                        {
                            Debug.WriteLine("###" + entry1.Key.GetType().Name);
                            object value = hashtable[entry1.Key];
                            if (value is int)
                            {
                                GL.DeleteFramebuffer((int)value);
                                Debug.WriteLine("   ---" + entry1.Key + " is deleted");
                            }
                            if(value is int[])
                            {
                                int[] buffers = (int[])value;
                                GL.DeleteFramebuffers(buffers.Length,buffers);
                                Debug.WriteLine("   ---" + entry1.Key + " is deleted");
                            }
                        }
                        if(entry1.Key.Equals("glTexture"))
                        {
                            GL.DeleteTexture((int)hashtable[entry1.Key]);
                            Debug.WriteLine("   ---" + entry1.Key + " is deleted");
                        }

                    }
                }
            }
            Properties = new Dictionary<object, Hashtable>();
        }
    }
}
