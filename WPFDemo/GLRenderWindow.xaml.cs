using ImGuiNET;
using OpenTK.Windowing.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using THREEExample;
using WPFDemo.Controls;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Input;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
namespace WPFDemo
{
    /// <summary>
    /// UserExample.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GLRenderWindow : System.Windows.Controls.UserControl, IDisposable
    {
        public static bool IsInDesignerMode
        {
            get
            {
                return ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
            }
        }

        public Example currentExample;


        public GLRenderWindow()
        {
            InitializeComponent();
            //if (!IsInDesignerMode)
            //{
            //    this.renderer = new GLRenderControl();
            //    this.GLHost.Child = renderer;
            //    //this.example = new GLRenderer();
            //    //this.example.parentWindow = parentWindow;
            //    this.example.glControl = renderer;
            //    //this.example.renderer.ParentWindow = this as Control;

            //}

           
        }


        public GLRenderWindow(Example example)
        {
            InitializeComponent();

            this.GLHost.Child = example.glControl;
            this.currentExample = example;

            this.currentExample.glControl.MouseMove += glControl_MouseMove;
            this.currentExample.glControl.MouseDown += glControl_MouseDown;
            this.currentExample.glControl.MouseUp += glControl_MouseUp;
            this.currentExample.glControl.Resize += glControl_Resize;
            this.currentExample.glControl.MouseWheel += glControl_MouseWheel;
            this.currentExample.glControl.SizeChanged += glControl_SizeChanged;
            this.currentExample.glControl.KeyDown += glControl_KeyDown;
            this.currentExample.glControl.KeyUp += glControl_KeyUp;

        }
        private void glControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (currentExample != null)
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
                currentExample.OnKeyDown(key, e.KeyValue, (KeyModifiers)e.Modifiers);
            }
        }
        private void glControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (currentExample != null)
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
                currentExample.OnKeyUp(key, e.KeyValue, (KeyModifiers)e.Modifiers);
            }
        }


        private void glControl_SizeChanged(object sender, EventArgs e)
        {
            var control = sender as GLControl;
            this.currentExample?.OnResize(new ResizeEventArgs(control.Width, control.Height));
        }

 

        private void glControl_Resize(object sender, EventArgs e)
        {

            var control = sender as GLControl;

            if (control.ClientSize.Height == 0)
                control.ClientSize = new System.Drawing.Size(control.ClientSize.Width, 1);

            GL.Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);

            if (currentExample != null)
                currentExample.OnResize(new ResizeEventArgs(control.ClientSize.Width, control.ClientSize.Height));

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
            if (currentExample == null) return;
            MouseButton button = MouseButton.Left;
            currentExample.OnMouseDown(GetMouseButton(e), e.X, e.Y);
        }
        private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (currentExample == null) return;
            currentExample.OnMouseMove(GetMouseButton(e),e.X, e.Y);
        }

        private void glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (currentExample == null) return;
            currentExample.OnMouseUp(GetMouseButton(e),e.X,e.Y);
        }
        private void glControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (currentExample == null) return;
            currentExample.OnMouseWheel(e.X,e.Y,e.Delta);
        }
        public void Render()
        {
            if (!IsInDesignerMode)
            {
                if (this.currentExample == null) return;
                this.currentExample.OnResize(new ResizeEventArgs(currentExample.glControl.Width,currentExample.glControl.Height));
                this.currentExample.Render();
                if (this.currentExample.AddGuiControlsAction != null)
                {
                    ImGui.NewFrame();
                    this.currentExample.ShowGUIControls();
                    ImGui.Render();
                    currentExample.imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
                }
                this.currentExample.glControl.SwapBuffers();
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.Render();
        }


        private bool disposed;
        public virtual void Dispose()
        {
            this.currentExample.glControl.Dispose();
            this.currentExample.imGuiManager?.Dispose();
            this.currentExample?.Dispose();

            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                if (currentExample != null)
                {
                    currentExample.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}



