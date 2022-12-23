using System;

namespace THREE
{
    
    public abstract class Pass
    {
        public bool Enabled = true;
        public bool NeedsSwap = true;
        public bool Clear = false;
        public bool RenderToScreen = false;

        // Helper for passes that need to fill the viewport with a single quad.
        public class FullScreenQuad : IDisposable
        {
            private OrthographicCamera camera = new OrthographicCamera(-1, 1, 1, -1, 0, 1);
            private PlaneBufferGeometry geometry = new PlaneBufferGeometry(2, 2);

            private Mesh _mesh = null;
            private Scene scene = new Scene();
            public event EventHandler<EventArgs> Disposed;

            public Material material
            {
                get
                {
                    if (_mesh == null) return null;
                    else return _mesh.Material;
                }
                set
                {
                    if(_mesh==null)
                    {
                        _mesh = new Mesh(geometry, value);
                        scene.Add(_mesh);
                    }
                    else
                    {
                        _mesh.Material = value;
                    }
                }
            }
            public FullScreenQuad() 
            {
                
            }

            ~FullScreenQuad()
            {
                Dispose(false);
            }
            public FullScreenQuad(Material material)
            {
                _mesh = new Mesh(geometry, material);
               
                scene.Add(_mesh);
            }

            public void Render(GLRenderer renderer)
            {
                renderer.Render(scene, camera);
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
                    if(_mesh!=null)
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
        public FullScreenQuad fullScreenQuad = null;

        public Pass() { }

        public abstract void SetSize(float width, float height);
        
        

        public abstract void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime=null, bool? maskActive=null);

    }
}
