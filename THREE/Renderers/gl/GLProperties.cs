using System.Runtime.CompilerServices;
using System.Collections;

namespace THREE
{
    public class GLProperties
    {
        private ConditionalWeakTable<object,Hashtable> Properties = new ConditionalWeakTable<object,Hashtable>();
        
        public GLProperties()
        {
        }

        public Hashtable Get(object obj)
        {
            Hashtable map;
            if (!Properties.TryGetValue(obj, out map))
            {
                map = new Hashtable();
                Properties.Add(obj, map);
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
            Properties = new ConditionalWeakTable<object, Hashtable>();
        }
    }
}
