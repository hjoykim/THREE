using OpenTK.Windowing.Common;
namespace THREE
{
    public interface IControlsContainer
    {
        Rectangle ClientRectangle { get; }
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<MouseEventArgs> MouseWheel;
        event EventHandler<EventArgs> MouseEnter;
        event EventHandler<EventArgs> MouseLeave;
        event EventHandler<ResizeEventArgs> SizeChanged;
        event EventHandler<KeyboardKeyEventArgs> KeyDown;
        event EventHandler<KeyboardKeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
    }
}
