using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    public interface IGLTextures
    {
        int AllocateTextureUnit();
        void SafeSetTexture2D(Texture texture, int slot);
        void SetTexture3D(Texture texture, int slot);
        void SafeSetTextureCube(Texture texture, int slot);
        void SetTexture2DArray(Texture texture, int slot);
    }
}
