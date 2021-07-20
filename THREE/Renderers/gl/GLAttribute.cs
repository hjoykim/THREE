using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Renderers.gl
{
    public class GLAttribute : Dictionary<object,object>
    {
        public string Name;

        public VertexAttribPointerType type;

        public int bytesPerElement;
    }
}
