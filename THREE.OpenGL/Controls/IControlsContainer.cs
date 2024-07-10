using OpenTK.Windowing.Common;
namespace THREE
{
    public interface IControlsContainer
    {
        Rectangle ClientSize { get; }
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<MouseEventArgs> MouseWheel;
        event EventHandler<ResizeEventArgs> SizeChanged;
        event EventHandler<KeyboardKeyEventArgs> KeyDown;
        event EventHandler<KeyboardKeyEventArgs> KeyUp;
    }
}
