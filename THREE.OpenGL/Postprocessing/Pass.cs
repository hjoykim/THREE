using System;

namespace THREE
{
    [Serializable]
    // Helper for passes that need to fill the viewport with a single quad.
    public class FullScreenQuad : IDisposable
    {
        private OrthographicCamera camera = new OrthographicCamera(-1, 1, 1, -1, 0, 1);
        private BufferGeometry geometry = new BufferGeometry();

        private Mesh _mesh = null;
        public event EventHandler<EventArgs> Disposed;
      
        public Material material
        {
            get
            {
                return _mesh.Material;
            }
            set
            {
                _mesh.Material = value;
            }
        }

        ~FullScreenQuad()
        {
            Dispose(false);
        }
        public FullScreenQuad(Material material = null)  
        {
            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { -1, 3, 0, -1, -1, 0, 3, -1, 0 }, 3));
            geometry.SetAttribute("uv", new BufferAttribute<float>(new float[] { 0, 2, 0, 0, 2, 0 }, 2));
            _mesh = new Mesh(geometry, material);

        }

        public void Render(GLRenderer renderer)
        {
            renderer.Render(_mesh, camera);
        }
        public virtual void Dispose()
        {
            Dispose(disposed);
        }
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
                handler(this, new EventArgs());
        }
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            try
            {
                if (_mesh != null)
                    _mesh.Geometry.Dispose();
                this.RaiseDisposed();
                this.disposed = true;
            }
            finally
            {

            }
            this.disposed = true;
        }

    }
    [Serializable]
    public abstract class Pass
    {
        public bool Enabled = true;
        public bool NeedsSwap = true;
        public bool Clear = false;
        public bool RenderToScreen = false;
        public FullScreenQuad fullScreenQuad;
        public Pass() { }

        public abstract void SetSize(float width, float height);



        public abstract void Render(GLRenderer renderer, GLRenderTarget writeBuffer , GLRenderTarget readBuffer=null, float? deltaTime = null, bool? maskActive = null);

    }
}
