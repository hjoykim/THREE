
using Silk.NET.GLFW;
using Silk.NET.Input;
using MouseButton = Silk.NET.Input.MouseButton;
using Key = Silk.NET.Input.Key;

namespace THREE.Silk
{
    [Serializable]
    public abstract class ControlsContainer : DisposableObject, IControlsContainer
    {
        public Rectangle ClientRectangle => GetClientRectangle();

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseWheel;
        public event EventHandler<EventArgs> MouseEnter;
        public event EventHandler<EventArgs> MouseLeave;
        public event EventHandler<ResizeEventArgs> SizeChanged;
        public event EventHandler<KeyboardKeyEventArgs> KeyDown;
        public event EventHandler<KeyboardKeyEventArgs> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        abstract public Rectangle GetClientRectangle();
        public virtual void OnResize(ResizeEventArgs args)
        {
            SizeChanged?.Invoke(this, args);
        }
        public virtual void OnMouseUp(MouseButton button, int x,int y)
        {
            MouseUp?.Invoke(this, new MouseEventArgs(button,x,y,0));
        }
        public virtual void OnMouseWheel(int x,int y, int delta)
        {
            MouseWheel?.Invoke(this, new MouseEventArgs(MouseButton.Middle,x,y,delta));
        }
        public virtual void OnMouseMove(MouseButton button, int x,int y)
        {
            MouseMove?.Invoke(this, new MouseEventArgs(button,x,y,0));
        }
        public virtual void OnMouseDown(MouseButton button, int x,int y)
        {
            MouseDown?.Invoke(this, new MouseEventArgs(button, x,y,0));
        }
        public virtual void OnKeyDown(Key key, int scanCode,KeyModifiers modifer)
        {
            KeyDown?.Invoke(this,new KeyboardKeyEventArgs(key, scanCode, modifer, false));
        }
        public virtual void OnKeyUp(Key key,int scanCode,KeyModifiers modifer)
        {
            KeyDown?.Invoke(this, new KeyboardKeyEventArgs(key, scanCode, modifer, false));
        }
        public virtual void OnKeyPress(string ch)
        {
            KeyPress?.Invoke(this, new KeyPressEventArgs(ch));
        }
        public virtual void OnMouseEnter()
        {
            MouseEnter?.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseLeave()
        {
            MouseLeave?.Invoke(this, EventArgs.Empty);
        }

    }
}
