﻿using System.Collections;


namespace THREE
{
    [Serializable]
    public class GLMultisampleRenderTarget : GLRenderTarget
    {
        public int Samples = 4;
        public GLMultisampleRenderTarget(int width, int height, Hashtable option = null)
            : base(width, height, option)
        {

        }
    }
}
