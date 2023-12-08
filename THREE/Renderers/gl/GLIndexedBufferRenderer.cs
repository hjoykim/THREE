using OpenTK.Graphics.ES30;
using System;

namespace THREE
{
    public class GLIndexedBufferRenderer : GLBufferRenderer
    {
        private VertexAttribPointerType type;

        private int bytesPerElement;



        public GLIndexedBufferRenderer(GLRenderer renderer, GLExtensions extensions, GLInfo info, GLCapabilities capabilities) : base(renderer,extensions,info,capabilities)
        {
        }

        //public void SetMode(PrimitiveType value)
        //{
        //    this.mode = value;
        //}

        public void SetIndex(BufferType value)
        {
            var pointerType = (VertexAttribPointerType)Enum.ToObject(typeof(VertexAttribPointerType), value.Type);
            this.type = pointerType;

            this.bytesPerElement = value.BytesPerElement;    
        }

        public override void Render(int start, int count)
        {
            int indices = start*this.bytesPerElement;
            IntPtr ptr = IntPtr.Add(IntPtr.Zero,indices);
            

            All mode1 = (All)Enum.ToObject(typeof(All), (int)mode);
            All type1 = (All)Enum.ToObject(typeof(All), (int)type);
            //PrimitiveType mode1 = (PrimitiveType)Enum.ToObject(typeof(PrimitiveType), (int)mode);
            //DrawElementsType type1 = (DrawElementsType)Enum.ToObject(typeof(DrawElementsType), (int)type);

            GL.DrawElements(mode1, count, type1, ptr);

            info.Update(count, (int)mode);
        }

        public override void RenderInstances(Geometry geometry, int start, int count, int primcount)
        {
            if (primcount == 0) return;

            int indices = start*this.bytesPerElement;

            All mode1 = (All)Enum.ToObject(typeof(All), (int)mode);
            All type1 = (All)Enum.ToObject(typeof(All), (int)type);

            //PrimitiveType mode1 = (PrimitiveType)Enum.ToObject(typeof(PrimitiveType), (int)mode);
            //DrawElementsType type1 = (DrawElementsType)Enum.ToObject(typeof(DrawElementsType), (int)type);

            GL.DrawElementsInstanced(mode1, count, type1, ref indices, primcount);

            info.Update(count, (int)mode, primcount);

        }
    }
}
