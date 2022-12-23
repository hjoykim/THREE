using System;
using System.Collections.Generic;
using THREE;
namespace THREE
{
    public abstract class BaseGeometry : IDisposable
    {
        public Guid Uuid = Guid.NewGuid();

        private bool _disposed = false;

        public int Id;

        public string Name;

        public string type;

        public Box3 BoundingBox = null;

        public Sphere BoundingSphere = null;

        public abstract void ComputeBoundingSphere();

        public abstract void ComputeBoundingBox();

        public abstract void ComputeVertexNormals(bool areaWeighted = false);
        
        #region dynamic attribute
        public bool glInit = false;

        public int glLineDistanceBuffer = 0;

        public int glVertexBuffer = 0;

        public int glNormalBuffer = 0;

        public int glTangentBuffer = 0;

        public int glColorBuffer = 0;

        public int glUVBuffer = 0;

        public int glUV2Buffer = 0;

        public int glSkinIndicesBuffer = 0;

        public int glSkinWeightsBuffer = 0;

        public int glFaceBuffer = 0;

        public int glLineBuffer = 0;

        public List<int> glMorphTargetsBuffers;

        public List<int> glMorphNormalsBuffers;

        public object sortArray;

        public float[] vertexArray;

        public float[] normalArray;

        public float[] tangentArray;

        public float[] colorArray;

        public float[] uvArray;

        public float[] uv2Array;

        public float[] skinIndexArray;

        public float[] skinWeightArray;

        public Type typeArray;

        public ushort[] faceArray;

        public ushort[] lineArray;

        public List<float[]> morphTargetsArrays;

        public List<float[]> morphNormalsArrays;

        public int glFaceCount = -1;

        public int glLineCount = -1;

        public int glParticleCount = -1;

        public List<GLAttribute> glCustomAttributesList;

        public bool initttedArrays;

        public float[] lineDistanceArray;



        #endregion

        public event EventHandler<EventArgs> Disposed;
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    this.RaiseDisposed();
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion
    }
}
