using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Core
{
    public abstract class BasicObject : Hashtable,IDisposable
    {
        public event EventHandler<EventArgs> Disposed;
        public BasicObject()
        {

        }
        ~BasicObject()
        {
            this.Dispose(false);
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
