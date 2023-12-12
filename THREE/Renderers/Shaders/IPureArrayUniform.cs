using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace THREE
{
    public interface IPureArrayUniform
    {
        int Size { get; set; }
        void UpdateCache(object[] data);
        void UpdateCache(List<object> data);
        void SetValue(float[] v);
        void SetValue(Vector2 v);
        void SetValue(Vector3 v);
        void SetValue(Vector2[] v); // setValueV2fArray
        void SetValue(Vector3[] v);// setValueV3fArray
        void SetValue(Color[] v);
        void SetValue(Vector4[] v);
        // setValueM3Array
        void SetValue(Matrix3[] v);
        // setValueM4Array
        void SetValue(Matrix4[] v);

        // Array of textures(2D/Cube)
        //setValueT1Array
        void SetValue(Texture[] v, IGLTextures textures);
        void SetValue(object v, IGLTextures textures = null);
    }
}
