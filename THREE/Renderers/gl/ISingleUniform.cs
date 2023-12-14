using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace THREE
{
    public interface ISingleUniform
    {
        public void SetValue(int v);
        public void SetValue(float v);
        public void SetValue(Vector2 v);
        public void SetValue(Vector3 v);
        public void SetValue(Vector4 v);
        public void SetValue(Matrix3 matrix);
        public void SetValue(Matrix4 matrix);
        public void SetValue(Color color);
        public void SetValue(float[] color);

        public void SetValue(Texture v, IGLTextures textures);

        public void SetValue(object v, IGLTextures textures = null);

    }
}
