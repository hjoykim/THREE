using FastDeepCloner;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public abstract class BasicObject : EventDispatcher, IDisposable, ICloneable
    {
        public event EventHandler<EventArgs> Disposed;
        public BasicObject() {}

        public BasicObject(SerializationInfo info, StreamingContext context) { }

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

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
