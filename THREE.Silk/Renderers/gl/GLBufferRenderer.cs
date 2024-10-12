

using Silk.NET.OpenGLES;

namespace THREE
{
    [Serializable]
    public class GLBufferRenderer
    {
        private GLRenderer renderer;

        private GLExtensions extensions;

        public GLInfo info;

        private GLCapabilities capabilities;

        public PrimitiveType mode;

        public bool IsGL2;
        protected GL gl;
        public GLBufferRenderer(GLRenderer renderer, GLExtensions extensions, GLInfo info, GLCapabilities capabilities)
        {
            this.renderer = renderer;

            this.extensions = extensions;

            this.info = info;

            this.capabilities = capabilities;

            gl = renderer.gl;
        }

        public void SetMode(PrimitiveType value)
        {
            this.mode = value;
        }

        public virtual void Render(int start, int count)
        {
            gl.DrawArrays(this.mode, start, (uint)count);

            this.info.Update(count, (int)mode);
        }

        public virtual void RenderInstances(Geometry geometry, int start, int count, int primcount)
        {
            if (primcount == 0) return;

            gl.DrawArraysInstanced(this.mode, start, (uint)count, (uint)primcount);

            this.info.Update(count, (int)mode, primcount);
        }
    }
}
