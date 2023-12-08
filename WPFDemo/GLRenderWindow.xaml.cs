using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using THREEExample;
using WPFDemo.Controls;

namespace WPFDemo
{
    /// <summary>
    /// UserExample.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GLRenderWindow : UserControl, IDisposable
    {
        public static bool IsInDesignerMode
        {
            get
            {
                return ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
            }
        }

        public Example example;


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
            var renderControl = new GLRenderControl();
            this.GLHost.Child = renderControl;
            this.example = example;
            //this.example.parentWindow = parentWindow;
            this.example.glControl = renderControl;
            //this.example.Parent = this;
            //renderer.scene = this.example;
#if NET6_0_OR_GREATER
            renderControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
#endif
        }
        public void Render()
        {
            if (!IsInDesignerMode)
            {
                if (this.example == null) return;
                this.example.Resize(example.glControl.ClientSize);
                this.example.Render();
                this.example.glControl.SwapBuffers();
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                if (example != null)
                    example.renderer.Dispose();
            }
            this.disposed = true;
        }
    }
}



