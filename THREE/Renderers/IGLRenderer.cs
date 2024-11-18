using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    public interface IGLRenderer
    {
        bool IsGL2 { get; set; }
        bool AutoClear { get; set; }
        bool AutoClearColor { get; set; }
        bool AutoClearDepth { get; set; }
        bool AutoClearStencil { get; set; }
        //IGLState state_todo { get; set; }

        void Clear(bool? color = null, bool? depth = null, bool? stencil = null);
        Color GetClearColor();
        void SetClearColor(Color color, float depth = 1.0f);
        float GetClearAlpha();
        void SetClearAlpha(float alpha);
        void ClearDepth();
        Vector2 GetSize(Vector2 target = null);
        float GetPixelRatio();       
        Vector4 GetCurrentViewport(Vector4 target);
        GLRenderTarget GetRenderTarget();
        void SetRenderTarget(GLRenderTarget target,int? activeCubeFace=null, int? activeMipmapLevel=null);
        void Render(Object3D scene, Camera camera);
        void RenderBufferDirect(Camera camera, Object3D scene, Geometry geometry, Material material, Object3D object3D, DrawRange? group);
        void ReadRenderTargetPixels(GLRenderTarget renderTarget, float x, float y, int width, int height, byte[] buffer, int? activeCubeFaceIndex);
        void CopyFramebufferToTexture(Vector2 position, Texture texture, int? level = null);
    }
}
