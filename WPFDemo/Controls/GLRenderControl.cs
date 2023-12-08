using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using System;

namespace WPFDemo.Controls
{
    public class GLRenderControl : OpenTK.GLControl
    {
        public EventHandler<EventArgs> OnInitGL;



        public GLRenderControl() : base()
        {
            InitGLContext();
        }

        public GLRenderControl(GraphicsMode mode) : base(mode)
        {
            InitGLContext();
        }

        private void InitGLContext()
        {
            this.MakeCurrent();
            this.VSync = true;
            if (OnInitGL != null)
            {
                OnInitGL(this, new EventArgs());
            }


        }
        public GLRenderControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags) : base(mode, major, minor, flags)
        {
            InitGLContext();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.IsHandleCreated)
            {
                SetViewport(0, 0, this.Size.Width, this.Size.Height);
                //if(scene!=null)
                //    scene.Resize(this.Size.Width,this.Size.Height);
            }
        }
        public void SetViewport(int x, int y, int width, int height)
        {

            GL.Viewport(x, y, width, height);
        }


        public void Clean()
        {
            //GL.ClearColor(System.Drawing.Color.FromArgb(this.ClearAlpha, this.ClearColor));
            GL.ClearBuffer(ClearBuffer.Color, 0, new float[4] { 0, 0, 0, 1 });
        }

       
    }
}
