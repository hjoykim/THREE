using FastDeepCloner;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;


namespace THREE
{
    public class BufferGeometryUtils
    {

        public static BufferGeometry ToIndexedBufferGeometry(BufferGeometry src, bool? fullIndex = null, int? precision = null)
        {
            precision = precision != null ? precision : 6;
            float prec = (float)Math.Pow(10, precision.Value);
            float precHalf = (float)Math.Pow(10, Math.Floor((double)(precision.Value / 2)));
            var geometry = new BufferGeometry();
            var Array = (src.Attributes["position"] as BufferAttribute<float>).Array;
            var list = new List<float>();
            Dictionary<string, int> vertices = new Dictionary<string, int>();

            string Floor(float[] array, int offset)
            {
                return ((int)Math.Floor(array[offset] * prec)).ToString();
            }


            string HasAttribute(float[] array, int offset)
            {
                return Floor(array, offset) + '_' + Floor(array, offset + 1) + '_' + Floor(array, offset + 2);
            }

            int Store(int index, int n)
            {
                string id = "";
                int itemSize = 3;
                int offset = itemSize * index * 3 + n * itemSize;
                id += HasAttribute(Array, offset) + "_";
                if (!vertices.ContainsKey(id))
                {
                    vertices[id] = list.Count();
                    list.Add(index * 3 + n);
                }
                return vertices[id];
            }
            int StoreFast(float x, float y, float z, float v)
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
                if (fullIndex)
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

            geometry = IndexBufferGeometry(src, fullIndex == null ? true : fullIndex.Value);
            return geometry;
        }

 
        public static BufferGeometry MergeVertices(BufferGeometry geometry, double tolerance = 1e-4)
        {
            tolerance = Math.Max(tolerance,double.Epsilon);
            Dictionary<string,int> hashToIndex = new Dictionary<string,int>();

            var indices = geometry.GetIndex();
            var positions = geometry.Attributes["position"] as BufferAttribute<float>;
            var vertexCount = indices!=null ? indices.count : positions.count;

            // next value for triangle indices
            var nextIndex = 0;

            // attributes and new attribute arrays
            var attributeNames = geometry.Attributes.Keys.ToArray();
            var tmpAttributes = new Dictionary<object,object>();
            var tmpMorphAttributes = new Dictionary<object,object>();
            var newIndices = new List<int>();

            // Initialize the arrays, allocating space conservatively. Extra
            // space will be trimmed in the last step.
            for (var i = 0;i< attributeNames.Length; i++)
            {

                var name = attributeNames[i];
                var attr = geometry.Attributes[name] as IBufferAttribute;
                if(attr is BufferAttribute<float>)
                {
                    tmpAttributes[name] = new BufferAttribute<float>(new float[attr.ItemSize * attr.count],attr.ItemSize,attr.Normalized);
                }
                else if (attr is BufferAttribute<int>)
                {
                    tmpAttributes[name] = new BufferAttribute<int>(new int[attr.ItemSize * attr.count], attr.ItemSize, attr.Normalized);
                }
                else 
                {
                    tmpAttributes[name] = new BufferAttribute<byte>(new byte[attr.ItemSize * attr.count], attr.ItemSize, attr.Normalized);
                }


                var morphAttributes = geometry.MorphAttributes[name];
                if (morphAttributes!=null)
                {
                    
                    foreach (var morphAttr in morphAttributes as List<IBufferAttribute>)
                    {
                        if (!tmpMorphAttributes.ContainsKey(name)) tmpMorphAttributes[name] = new List<IBufferAttribute>();
                        if (morphAttr.Type == typeof(float))
                        {
                            
                            var array = new float[morphAttr.count * morphAttr.ItemSize];
                            (tmpMorphAttributes[name] as List<IBufferAttribute>).Add(new BufferAttribute<float>(array, morphAttr.ItemSize, morphAttr.Normalized));
                        }
                        else if (morphAttr.Type == typeof(int))
                        {
                            var array = new int[morphAttr.count * morphAttr.ItemSize];
                            (tmpMorphAttributes[name] as List<IBufferAttribute>).Add(new BufferAttribute<int>(array, morphAttr.ItemSize, morphAttr.Normalized));
                        }
                        else 
                        {
                            var array = new byte[morphAttr.count * morphAttr.ItemSize];
                            (tmpMorphAttributes[name] as List<IBufferAttribute>).Add(new BufferAttribute<byte>(array, morphAttr.ItemSize, morphAttr.Normalized));
                        }
                    }
                   

                }
            }

            // convert the error tolerance to an amount of decimal places to truncate to
            var halfTolerance = tolerance * 0.5;
            var exponent = Math.Log10(1 / tolerance);
            var hashMultiplier = (float)Math.Pow(10, exponent);
            var hashAdditive = halfTolerance * hashMultiplier;
            for (var i = 0; i < vertexCount; i++)
            {

                var index = indices != null ? indices.GetX(i) : i;

                // Generate a hash for the vertex attributes at the current index 'i'
                var hash = "";
                for (var j = 0; j < attributeNames.Length; j++)
                {

                    var name = attributeNames[j];
                    var attribute = geometry.Attributes[name] as IBufferAttribute;
                    var itemSize = attribute.ItemSize;

                    for (var k = 0; k < itemSize; k++)
                    {
                        hash += $"{Math.Truncate((float)attribute.Getter(k, index)*hashMultiplier+hashAdditive)},";
                        // double tilde truncates the decimal value
                        //hash += `${ ~~(attribute[getters[k]](index) * hashMultiplier + hashAdditive) },`;
                    }

                }

                // Add another reference to the vertex if it's already
                // used by another index
                if (hashToIndex.ContainsKey(hash))
                {
                    newIndices.Add(hashToIndex[hash]);
                }
                else
                {
                    // copy data to the new index in the temporary attributes
                    for (var j = 0; j < attributeNames.Length; j++)
                    {
                        var name = attributeNames[j];
                        var attribute = geometry.Attributes[name] as IBufferAttribute;
                        var morphAttributes = geometry.MorphAttributes[name] as List<IBufferAttribute>;
                        var itemSize = attribute.ItemSize;
                        var newArray = tmpAttributes[name] as IBufferAttribute;
                        var newMorphArrays = tmpMorphAttributes.ContainsKey(name)?tmpMorphAttributes[name] : null;

                        for (var k = 0; k < itemSize; k++)
                        {
                            newArray.Setter(k, nextIndex, attribute.Getter(k, index));

                            if (morphAttributes != null && newMorphArrays != null)
                            {

                                for (var m = 0; m < (morphAttributes as List<IBufferAttribute>).Count; m++)
                                {
                                    (newMorphArrays as List<IBufferAttribute>)[m].Setter(k, nextIndex, morphAttributes[m].Getter(k, index));
                                }
                            }
                        }
                    }

                    hashToIndex[hash] = nextIndex;
                    newIndices.Add(nextIndex);
                    nextIndex++;
                }
            }

            // generate result BufferGeometry
            var result = geometry.Clone();
            foreach(var name in geometry.Attributes.Keys ) 
            {
                var tmpAttribute = tmpAttributes[name] as IBufferAttribute;
                result.SetAttribute((string)name, tmpAttribute);
                
                if (!tmpMorphAttributes.ContainsKey(name)) continue;
                var list = (tmpMorphAttributes[name] as List<IBufferAttribute>);
                for (var j = 0; j < (tmpMorphAttributes[name] as List<IBufferAttribute>).Count; j++)
                {
                    var tmpMorphAttribute = (tmpMorphAttributes[name] as List<IBufferAttribute>)[j];
                    (result.MorphAttributes[name] as List<IBufferAttribute>)[j] = tmpMorphAttribute;       

                }
            }

            // indices

            result.SetIndex(newIndices);

            return result;
        }
    }
}
