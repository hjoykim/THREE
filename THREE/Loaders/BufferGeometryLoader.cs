using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Linq;

namespace THREE
{
    public class BufferGeometryLoader
    {
        Hashtable interleavedBufferMap = new Hashtable();
        Hashtable arrayBufferMap = new Hashtable();

        public BufferGeometryLoader()
        {

        }

        public BufferGeometry Load(string fileName)
        {
            string text = File.ReadAllText(fileName);
            JObject jobject = JObject.Parse(text);
            return Parse(jobject);
        }

        public BufferGeometry Parse(JObject json)
        {
            BufferGeometry geometry = null;

            if (json.ContainsKey("isInstancedBufferGeometry"))
            {
                geometry = (bool)json["isInstancedBufferGeometry"] ? new InstancedBufferGeometry() : new BufferGeometry();
            }
            else
            {
                geometry = new BufferGeometry();
            }

            object indexObj = json["data"]["index"];

            if(indexObj!=null)
            {
                int[] index = indexObj as int[];
                geometry.SetIndex(index.ToList<int>(), 1);
            }

            JObject data = (JObject)json["data"];

            if (data != null)
            {
                JObject attributes = (JObject)data["attributes"];
                if(attributes!=null)
                {
                    foreach(var o in attributes)
                    {
                        JObject attribute = (JObject)attributes[o.Key];
                        if(attribute!=null)
                        {
                            int itemSize = (int)attribute["itemSize"];
                            JToken arrayToken = attribute["array"];
                            float[] floatArray = arrayToken.ToObject<float[]>();
                            geometry.SetAttribute(o.Key, new BufferAttribute<float>(floatArray, itemSize));
                        }
                    }
                }
            }
            return geometry;
        }
    }
}
