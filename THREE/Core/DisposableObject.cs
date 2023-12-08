using System;

namespace THREE
{
    public abstract class DisposableObject
    {
        public event EventHandler<EventArgs> Disposed;
        
        public DisposableObject()
        {

        }
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        public bool IsDisposed()
        {
            return this.disposed;
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
                this.RaiseDisposed();
                this.disposed = true;
            }
            finally
            {

            }
            this.disposed = true;
        }
    }
}
