using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Silk
{
    [Serializable]
    public readonly struct ResizeEventArgs
    {
        public Vector2D<int> Size { get; }
        public int Width =>Size.X;
        public int Height =>Size.Y;
        public ResizeEventArgs(Vector2D<int> size)
        {
            Size = size;
        }
        public ResizeEventArgs(int width,int height) : this(new Vector2D<int>(width,height))
        {

        }
    }
}
