using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace THREE
{
    public abstract class ControlsContainer : DisposableObject, IControlsContainer
    {
        public Rectangle ClientRectangle => GetClientRectangle();

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseWheel;
        public event EventHandler<ResizeEventArgs> SizeChanged;
        public event EventHandler<KeyboardKeyEventArgs> KeyDown;
        public event EventHandler<KeyboardKeyEventArgs> KeyUp;

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
        public virtual void OnKeyDown(Keys key, int scanCode,KeyModifiers modifer)
        {
            KeyDown?.Invoke(this,new KeyboardKeyEventArgs(key, scanCode, modifer, false));
        }
        public virtual void OnKeyUp(Keys key,int scanCode,KeyModifiers modifer)
        {
            KeyDown?.Invoke(this, new KeyboardKeyEventArgs(key, scanCode, modifer, false));
        }

    }
}
