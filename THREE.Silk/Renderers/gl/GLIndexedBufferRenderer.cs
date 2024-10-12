using Silk.NET.OpenGLES;
using System;

namespace THREE
{
    [Serializable]
    public class GLIndexedBufferRenderer : GLBufferRenderer
    {
        private DrawElementsType type;

        private int bytesPerElement;



        public GLIndexedBufferRenderer(GLRenderer renderer, GLExtensions extensions, GLInfo info, GLCapabilities capabilities) : base(renderer, extensions, info, capabilities)
        {
        }

        //public void SetMode(PrimitiveType value)
        //{
        //    this.mode = value;
        //}

        public void SetIndex(BufferType value)
        {
            var pointerType = (DrawElementsType)Enum.ToObject(typeof(DrawElementsType), value.Type);
            this.type = pointerType;

            this.bytesPerElement = value.BytesPerElement;
        }

        public unsafe override void Render(int start, int count)
        {
            int indices = start * this.bytesPerElement;           

            gl.DrawElements(mode, (uint)count, type, (void*)indices);

            info.Update(count, (int)mode);
        }

        public unsafe override void RenderInstances(Geometry geometry, int start, int count, int primcount)
        {
            if (primcount == 0) return;

            int indices = start * this.bytesPerElement;
           

            gl.DrawElementsInstanced(mode, (uint)count, type, (void*) indices, (uint)primcount);

            info.Update(count, (int)mode, primcount);

        }
    }
}
