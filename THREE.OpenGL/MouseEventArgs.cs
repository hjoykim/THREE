
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace THREE
{
    public struct MouseEventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MouseButton Button { get; }
        public int Delta { get; }
        public MouseEventArgs(MouseButton button, int x,int y, int delta)
        {
            Button = button;           
            X = x;
            Y = y;
            Delta = delta;
        }
        
    }
}
