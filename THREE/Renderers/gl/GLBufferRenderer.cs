using OpenTK.Graphics.ES30;


namespace THREE
{
    public class GLBufferRenderer
    {
        private GLRenderer renderer;
        
        private GLExtensions extensions;

        public GLInfo info;

        private GLCapabilities capabilities;

        public PrimitiveType mode;

        public bool IsGL2;

        public GLBufferRenderer(GLRenderer renderer, GLExtensions extensions, GLInfo info, GLCapabilities capabilities)
        {
            this.renderer = renderer;

            this.extensions = extensions;

            this.info = info;

            this.capabilities = capabilities;
        }

        public void SetMode(PrimitiveType value)
        {
            this.mode = value;
        }

        public virtual void Render(int start,int count)
        {
            GL.DrawArrays(this.mode, start, count);

            this.info.Update(count, (int)mode);
        }

        public virtual void RenderInstances(Geometry geometry,int start,int count,int primcount)
        {
            if (primcount == 0) return;

            GL.DrawArraysInstanced(this.mode, start, count, primcount);

            this.info.Update(count, (int)mode, primcount);
        }
    }
}
