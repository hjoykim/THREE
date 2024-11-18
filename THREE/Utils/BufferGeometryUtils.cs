using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    public class BufferGeometryUtils
    {

       
       
        public static BufferGeometry ToIndexedBufferGeometry(BufferGeometry src,bool? fullIndex = null , int? precision=null)
        {
            precision = precision!=null ? precision : 6;
            float prec  = (float)Math.Pow(10, precision.Value);
            float precHalf = (float)Math.Pow(10, Math.Floor((double)(precision.Value / 2)));
            var geometry = new BufferGeometry();
            var Array = (src.Attributes["position"] as BufferAttribute<float>).Array;
            var list = new List<float>();
            Dictionary<string,int> vertices = new Dictionary<string,int>();

            string Floor(float[] array,int offset)
            {
                return ((int)Math.Floor(array[offset] * prec)).ToString();
            }

            
            string HasAttribute(float[] array,int offset)
            {
                return Floor(array, offset) + '_' + Floor(array, offset + 1) + '_' + Floor(array, offset + 2);
            }

            int Store(int index,int n)
            {
                string id = "";
                int itemSize = 3;
                int offset = itemSize * index * 3 + n *itemSize;
                id += HasAttribute(Array, offset) + "_";
                if (!vertices.ContainsKey(id))
                {
                    vertices[id] = list.Count();
                    list.Add(index * 3 + n);
                }
                return vertices[id];
            }
            int StoreFast(float x,float y,float z,float v)
            {
                string id = ((int)Math.Floor(x * prec)).ToString() + '_' + ((int)Math.Floor(y * prec)).ToString() + '_' + ((int)Math.Floor(z * prec)).ToString();

                if (!vertices.ContainsKey(id))
                {

                    vertices[id] = list.Count;
                    list.Add(v);
                }

                return vertices[id];
            }
            BufferGeometry IndexBufferGeometry(BufferGeometry src, bool fullIndex)
            {
                BufferGeometry dst;
                var position = (src.Attributes["position"] as BufferAttribute<float>).Array;
                var faceCount = position.Length / 3 / 3;
                int[] indexArray = new int[faceCount * 3];
                if(fullIndex)
                {
                    for (int i = 0, l = faceCount; i < l; i++)
                    {

                        indexArray[i * 3] = Store(i, 0);
                        indexArray[i * 3 + 1] = Store(i, 1);
                        indexArray[i * 3 + 2] = Store(i, 2);

                    }
                }
                else
                {
                    for (int i = 0, l = faceCount; i < l; i++)
                    {

                        int offset = i * 9;

                        indexArray[i * 3] = StoreFast(position[offset], position[offset + 1], position[offset + 2], i * 3);
                        indexArray[i * 3 + 1] = StoreFast(position[offset + 3], position[offset + 4], position[offset + 5], i * 3 + 1);
                        indexArray[i * 3 + 2] = StoreFast(position[offset + 6], position[offset + 7], position[offset + 8], i * 3 + 2);

                    }
                }

                dst = src.Clone();
                dst.SetIndex(new BufferAttribute<int>(indexArray, 1));

                // Groups

                var groups = src.Groups;

                for (int i = 0, l = groups.Count; i < l; i++)
                {

                    var group = groups[i];

                    dst.AddGroup((int)group.Start, (int)group.Count, group.MaterialIndex);

                }
                return dst;
            }
            
            geometry = IndexBufferGeometry(src, fullIndex==null?true:fullIndex.Value);
            return geometry;
        }
    }
}
