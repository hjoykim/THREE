using System.Windows.Forms;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.WinForms;
using THREE;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
namespace SingleFormsDemo
{
    public partial class Form1 : Form
    {
        public THREEAppInstance threeInstance = null;

        private System.Windows.Forms.Timer _timer;
        private int timeInterval = 10;

        public Form1()
        {
            InitializeComponent();

            glControl.MouseWheel += glControl_MouseWheel;
        }
        private void Run()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = timeInterval;
            _timer.Tick += (sender, e) =>
            {
                Render();
            };
            _timer.Start();
        }

        private void Render()
        {
            this.glControl.MakeCurrent();
            threeInstance.Render();
            this.glControl.SwapBuffers();
        }

        private void glControl_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            (threeInstance.glControl as GLControl).Focus();
            threeInstance.OnMouseWheel(e.X, e.Y, e.Delta);
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            this.glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            threeInstance = new THREEAppInstance();
            threeInstance.Load(glControl);
            threeInstance.OnResize(new ResizeEventArgs(glControl.ClientSize.Width, glControl.ClientSize.Height));

            Run();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            var control = sender as GLControl;

            if (control.ClientSize.Height == 0)
                control.ClientSize = new Size(control.ClientSize.Width, 1);

            GL.Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);
            threeInstance?.OnResize(new ResizeEventArgs(control.ClientSize.Width, control.ClientSize.Height));
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
        private MouseButton GetMouseButton(System.Windows.Forms.MouseEventArgs e)
        {
            MouseButton button = MouseButton.Left;
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    button = MouseButton.Middle;
                    break;
                case MouseButtons.Right:
                    button = MouseButton.Right;
                    break;
                case MouseButtons.Left:
                case MouseButtons.None:
                default:
                    break;
            }
            return button;
        }

        private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            threeInstance?.OnMouseDown(GetMouseButton(e), e.X, e.Y);
        }

        private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            threeInstance?.OnMouseMove(GetMouseButton(e), e.X, e.Y);
        }

        private void glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            threeInstance?.OnMouseUp(GetMouseButton(e), e.X, e.Y);
        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyCode;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Right:
                    key = Keys.Right;
                    break;
                case System.Windows.Forms.Keys.Left:
                    key = Keys.Left;
                    break;
                case System.Windows.Forms.Keys.Down:
                    key = Keys.Down;
                    break;
                case System.Windows.Forms.Keys.Up:
                    key = Keys.Up;
                    break;

            }
            threeInstance?.OnKeyUp(key, e.KeyValue, (KeyModifiers)e.Modifiers);
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyCode;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Right:
                    key = Keys.Right;
                    break;
                case System.Windows.Forms.Keys.Left:
                    key = Keys.Left;
                    break;
                case System.Windows.Forms.Keys.Down:
                    key = Keys.Down;
                    break;
                case System.Windows.Forms.Keys.Up:
                    key = Keys.Up;
                    break;

            }
            threeInstance?.OnKeyDown(key, e.KeyValue, (KeyModifiers)e.Modifiers);
        }

        private void glControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            threeInstance?.OnKeyPress(e.KeyChar.ToString());
        }
    }
}
