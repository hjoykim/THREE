using System.Collections;
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
            Properties = new Dictionary<object, Hashtable>();
        }
    }
}
