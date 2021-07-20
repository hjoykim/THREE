using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace THREE.Extension
{
    public class ConcurrentList<T> : IList<T>,IDisposable
    {
        #region Fields
        private readonly List<T> _list;
        private readonly ReaderWriterLockSlim _lock;
        #endregion

        #region Constructors
        public ConcurrentList()
        {
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this._list = new List<T>();
        }

        public ConcurrentList(int capacity)
        {
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this._list = new List<T>(capacity);
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this._list = new List<T>(items);
        }
        #endregion

        #region Methods
        public void Add(T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Add(item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public void Insert(int index, T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Insert(index, item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            this._lock.EnterWriteLock();
            try
            {
                return this._list.Remove(item);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.RemoveAt(index);
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public int IndexOf(T item)
        {
            this._lock.EnterReadLock();
            try
            {
                return this._list.IndexOf(item);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }

        public void Clear()
        {
            this._lock.EnterWriteLock();
            try
            {
                this._list.Clear();
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            this._lock.EnterReadLock();
            try
            {
                return this._list.Contains(item);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._lock.EnterReadLock();
            try
            {
                this._list.CopyTo(array, arrayIndex);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(this._list, this._lock);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(this._list, this._lock);
        }

        ~ConcurrentList()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            this._lock.Dispose();
        }
        #endregion

        #region Properties
        public T this[int index]
        {
            get
            {
                this._lock.EnterReadLock();
                try
                {
                    return this._list[index];
                }
                finally
                {
                    this._lock.ExitReadLock();
                }
            }
            set
            {
                this._lock.EnterWriteLock();
                try
                {
                    this._list[index] = value;
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                this._lock.EnterReadLock();
                try
                {
                    return this._list.Count;
                }
                finally
                {
                    this._lock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion
    }

    public class ConcurrentEnumerator<T> : IEnumerator<T>
    {
        #region Fields
        private readonly IEnumerator<T> _inner;
        private readonly ReaderWriterLockSlim _lock;
        #endregion

        #region Constructor
        public ConcurrentEnumerator(IEnumerable<T> inner, ReaderWriterLockSlim @lock)
        {
            this._lock = @lock;
            this._lock.EnterReadLock();
            this._inner = inner.GetEnumerator();
        }
        #endregion

        #region Methods
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        public void Reset()
        {
            _inner.Reset();
        }

        public void Dispose()
        {
            this._lock.ExitReadLock();
        }
        #endregion

        #region Properties
        public T Current
        {
            get { return _inner.Current; }
        }

        object IEnumerator.Current
        {
            get { return _inner.Current; }
        }
        #endregion

    }
}
