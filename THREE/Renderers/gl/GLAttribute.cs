using OpenTK.Graphics.ES30;
using System.Collections.Generic;

namespace THREE
{
    public class GLAttribute : Dictionary<object,object>
    {
        public string Name;

        public VertexAttribPointerType type;

        public int bytesPerElement;
    }
}
