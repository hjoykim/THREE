using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Renderers
{
    public class GLMultiviewRenderTarget : GLRenderTarget
    {
        public int numViews;

        public GLMultiviewRenderTarget(int width,int height,int numViews,Hashtable options=null) : base(width,height,options)
        {
            this.IsGLMultiviewRenderTarget = true;
            this.numViews = numViews;
        }
    }
}
