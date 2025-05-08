using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace THREE
{
    public interface IStructuredUniform
    {
        public List<GLUniform> Seq { get; set; }
        void SetValue(IStructuredUniform uniform, object value, IGLTextures textures = null);
        void SetValue(object value, IGLTextures textures);
       
        
    }
}
