using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Renderers
{
    public class GLMultisampleRenderTarget : GLRenderTarget
    {
        public int Samples = 4;
        public GLMultisampleRenderTarget(int width, int height, Hashtable option = null)
            : base(width, height, option)
        {

        }
    }
}
