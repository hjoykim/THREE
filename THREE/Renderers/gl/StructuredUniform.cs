using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace THREE
{
    public class StructuredUniform : GLUniform
    {
        public List<GLUniform> Seq = new List<GLUniform>();


        public StructuredUniform()
        {
            UniformKind = "StructuredUniform";
        }

        public StructuredUniform(string id) :this()
        {
            this.Id = id;
        }

        public void SetValue(StructuredUniform uniform,object value,GLTextures textures)
        {
            for (int j = 0; j < uniform.Seq.Count; j++)
            {
                GLUniform u = uniform.Seq[j];

                object v = (value as Hashtable)[u.Id];

                if (u is SingleUniform)
                    (u as SingleUniform).SetValue(v,textures);                  

                else if (u is PureArrayUniform)
                    (u as PureArrayUniform).SetValue(v,textures);                   

                else if (u is StructuredUniform)
                    (u as StructuredUniform).SetValue(v, textures);
                else
                {
                    Trace.TraceWarning("StructuredUniform.SetValue : Unknown uniformtype");
                }
            }

        }

        public void SetValue(object value, GLTextures textures)
        {
            for (int i = 0, n = Seq.Count; i != n; ++i)
            {
                GLUniform u = Seq[i];
                object v = null;
               
                if (value is Hashtable)
                {
                    v = (value as Hashtable)[u.Id];
                }
                else if (value is Hashtable[])
                {
                    //var id = Convert.ToInt32(u.Id);
                    //value = (value as Hashtable[])[id];
                    v = (value as Hashtable[])[i];
                    if(v==null)
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
                    (u as SingleUniform).SetValue(v,textures);
                }
                else if(u.UniformKind.Equals("PureArrayUniform"))
                {
                    (u as PureArrayUniform).SetValue(v,textures);
                }
                else if(u is StructuredUniform)
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
                    (u as StructuredUniform).SetValue(v, textures);
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
