using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace THREE
{
    public class GLInfo
    {
        public struct Memory
        {
            public int Geometries;

            public int Textures;
        }

        public struct Render
        {
            public int Frame;

            public int Calls;

            public int Triangles;

            public int Points;

            public int Lines;
        }
        public Memory memory = new Memory { Geometries = 0, Textures = 0 };

        public Render render = new Render { Frame = 0, Calls = 0, Triangles = 0, Points = 0, Lines = 0 };

        public List<GLProgram> programs;

        public bool AutoReset = true;

        public GLInfo()
        {
        }

        public void Update(int count, int mode, int? instanceCount=null)
        {
            if (instanceCount == null) instanceCount = 1;

            this.render.Calls++;

            PrimitiveType type = (PrimitiveType)Enum.ToObject(typeof(PrimitiveType),mode);

            switch(type)
            {
                case PrimitiveType.Triangles:
                    render.Triangles += (int)instanceCount * (count / 3);
                    break;
                case PrimitiveType.Lines :
                    render.Lines += (int)instanceCount * (count / 2);
                    break;
                case PrimitiveType.LineStrip :
                    render.Lines += (int)instanceCount * (count - 1);
                    break;
                case PrimitiveType.LineLoop:
                    render.Lines += (int)instanceCount * count;
                    break;
                case PrimitiveType.Points :
                    render.Points += (int)instanceCount * count;
                    break;
                default :
                    Trace.TraceError("THREE.gl.GLInfo:Unknown draw mode:", mode);
                    break;
            }
        }
        public void Reset()
        {
            render.Frame++;
            render.Calls = 0;
            render.Triangles = 0;
            render.Points = 0;
            render.Lines = 0;
        }
    }
}
