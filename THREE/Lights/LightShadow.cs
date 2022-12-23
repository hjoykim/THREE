using System;
using System.Collections.Generic;

namespace THREE
{
    public class LightShadow : ICloneable
    {
        public Camera Camera;

        public float Bias;

        public float NormalBias;

        public float Radius;

        public Vector2 MapSize;       

        public GLRenderTarget Map;

        public GLRenderTarget MapPass;

        public Matrix4 Matrix = Matrix4.Identity();

        public Frustum _frustum = new Frustum();

        public Vector2 _frameExtents = new Vector2(1, 1);

        public int _viewportCount = 1;

        public List<Vector4> _viewports = new List<Vector4>();

        public Matrix4 _projScreenMatrix = Matrix4.Identity();

        public Vector3 _lightPositionWorld = Vector3.Zero();

        public Vector3 _lookTarget = Vector3.Zero();

        public bool AutoUpdate = true;

        public bool NeedsUpdate = false;
        public LightShadow(Camera camera)
        {
            this.Camera = camera;

            this.Bias = 0;

            this.NormalBias = 0;

            this.Radius = 1;

            this.MapSize = new Vector2(512, 512);

            this.Map = null;

            this.MapPass = null;

            _viewports.Add(new Vector4(0, 0, 1, 1));
        }

        protected LightShadow(LightShadow other)
        {
            this.Camera = (Camera)other.Camera.Clone();

            this.Bias = other.Bias;

            this.Radius = other.Radius;

            this.MapSize = other.MapSize;

        }

        public object Clone()
        {
            return new LightShadow(this);
        }
        public int GetViewportCount()
        {
            return this._viewportCount;
        }

        public Frustum GetFrustum()
        {
            return this._frustum;
        }

        public virtual void UpdateMatrices(Light light)
        {
            var shadowCamera = this.Camera;
            var shadowMatrix = this.Matrix;
            var projScreenMatrix = this._projScreenMatrix;
            var lookTarget = this._lookTarget;
            var lightPositionWorld = this._lightPositionWorld;

            lightPositionWorld.SetFromMatrixPosition(light.MatrixWorld);
            shadowCamera.Position.Copy(lightPositionWorld);

            lookTarget.SetFromMatrixPosition(light.Target.MatrixWorld);
            shadowCamera.LookAt(lookTarget);
            shadowCamera.UpdateMatrixWorld();

            projScreenMatrix.MultiplyMatrices(shadowCamera.ProjectionMatrix, shadowCamera.MatrixWorldInverse);
            this._frustum.SetFromProjectionMatrix(projScreenMatrix);

            shadowMatrix.Set(
                0.5f, 0.0f, 0.0f, 0.5f,
                0.0f, 0.5f, 0.0f, 0.5f,
                0.0f, 0.0f, 0.5f, 0.5f,
                0.0f, 0.0f, 0.0f, 1.0f
            );

            shadowMatrix.Multiply(shadowCamera.ProjectionMatrix);
            shadowMatrix.Multiply(shadowCamera.MatrixWorldInverse);
        }

        public Vector4 GetViewport(int viewportIndex)
        {
            return this._viewports[viewportIndex];
        }

        public Vector2 GetFrameExtents()
        {
            return this._frameExtents;
        }
    }
}
