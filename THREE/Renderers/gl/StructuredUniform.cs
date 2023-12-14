using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class StructuredUniform : GLUniform,IStructuredUniform
    {
        public List<GLUniform> Seq { get; set; } = new List<GLUniform>();


        public StructuredUniform()
        {
            UniformKind = "StructuredUniform";
        }

        public StructuredUniform(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public StructuredUniform(string id) : this()
        {
            this.Id = id;
        }

        public void SetValue(IStructuredUniform uniform, object value, IGLTextures textures=null)
        {
            for (int j = 0; j < uniform.Seq.Count; j++)
            {
                GLUniform u = uniform.Seq[j];

                object v = (value as Hashtable)[u.Id];

                if (u is ISingleUniform)
                    (u as ISingleUniform).SetValue(v, textures);

                else if (u is IPureArrayUniform)
                    (u as IPureArrayUniform).SetValue(v, textures);

                else if (u is IStructuredUniform)
                    (u as IStructuredUniform).SetValue(v, textures);
                else
                {
                    Trace.TraceWarning("StructuredUniform.SetValue : Unknown uniformtype");
                }
            }

        }

        public void SetValue(object value, IGLTextures textures=null)
        {
            for (int i = 0, n = Seq.Count; i != n; ++i)
            {
                GLUniform u = Seq[i];
                object v = null;

                if (value is GLUniform)
                {
                    v = (value as GLUniform)[u.Id];
                }
                else if (value is GLUniform[])
                {
                    //var id = Convert.ToInt32(u.Id);
                    //value = (value as GLUniform[])[id];
                    v = (value as GLUniform[])[i];
                    if (v == null)
                    {
                        Debug.WriteLine("Value is null");
                    }
                }
                else
                {
                    v = value;
                }
                if (u.UniformKind.Equals("SingleUniform"))
                {
                    (u as ISingleUniform).SetValue(v, textures);
                }
                else if (u.UniformKind.Equals("PureArrayUniform"))
                {
                    (u as IPureArrayUniform).SetValue(v, textures);
                }
                else if (u is IStructuredUniform)
                {

                    //if (value is Hashtable[])
                    //{
                    //    //var id = Convert.ToInt32(u.Id);
                    //    //value = (value as Hashtable[])[id];
                    //    v = (value as Hashtable[])[i];
                    //}
                    //else
                    //{
                    //    v = value;
                    //}
                    (u as IStructuredUniform).SetValue(v, textures);
                    //SetValue( u as StructuredUniform,v, textures);
                }

                else
                {
                    Trace.TraceWarning("StructuredUniform.SetValue : Unknown uniformtype");
                }
            }
        }
    }
}
