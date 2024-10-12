using System.Collections.Generic;

namespace THREE
{
    [Serializable]
    public class GLAttribute : Dictionary<string, object>,IGLAttribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public int ItemSize { get; set; }
        //public VertexAttribPointerType type;

        //public int bytesPerElement;
    }
}
