using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace THREE
{
    public interface IThreeWindow
    {
        unsafe Window* windowPtr { get; set; }
        Box2i Bounds { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float AspectRatio{get;}

        void MakeCurrent();
        void SwapBuffers();
        void PollEvents();
        IGraphicsContext Context {get;set;}
        event Action<MouseButtonEventArgs> MouseDown;
        event Action<MouseButtonEventArgs> MouseUp;
        event Action<MouseMoveEventArgs> MouseMove;
        event Action<MouseWheelEventArgs> MouseWheel;
        event Action<ResizeEventArgs> SizeChanged;
    }
}
