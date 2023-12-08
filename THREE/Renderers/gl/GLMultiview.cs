using OpenTK.Graphics.ES30;
using System.Collections;
using System.Collections.Generic;


namespace THREE
{
    public class GLMultiview
    {
        public int DEFAULT_NUMVIEWS = 2;

        private GLExtensions extensions;

        private GLProperties properties;

        private GLRenderTarget renderTarget;

        private GLRenderTarget currentRenderTarget;

        private GLRenderer renderer;

        private bool available;

        private int maxNumViews = 0;

        private List<Matrix4> mat4 = new List<Matrix4>();

        private List<Matrix3> mat3 = new List<Matrix3>();

        private List<Camera> CameraArray = new List<Camera>();

        private Vector2 renderSize;

        public GLMultiview(GLRenderer renderer)
        {
            this.extensions = renderer.extensions;
            this.properties = renderer.properties;
            this.renderer = renderer;
           
        }

        public bool IsAvailable()
        {
            available = false;
            var extension = extensions.Get("GL_OVR_multiview2");

            if (extension != -1)
            {
                available = true;
                maxNumViews = GL.GetInteger(GetPName.MaxViewportDims);

                renderTarget = new GLMultiviewRenderTarget(0, 0, DEFAULT_NUMVIEWS);

                renderSize = Vector2.Zero();

                for (int i = 0; i < maxNumViews; i++)
                {
                    mat4.Add(new Matrix4());
                    mat3.Add(new Matrix3());
                }
            }

            return available;
        }

        public List<Camera> GetCameraArray(Camera camera)
        {
            if (camera is ArrayCamera)
            {
                return (camera as ArrayCamera).Cameras;
            }

            this.CameraArray.Add(camera);

            return this.CameraArray;
        }

        public void UpdateCameraProjectionMatricesUniform(Camera camera, Hashtable uniforms)
        {
            var cameras = this.GetCameraArray(camera);

            for (var i = 0; i < cameras.Count; i++)
            {
                mat4[i] = cameras[i].ProjectionMatrix;
            }

            if (uniforms.ContainsKey("projectionMatrices"))
                uniforms["projectionMatrices"] = mat4;
            else
                uniforms.Add("projectionMatrices", mat4);
            
        }

        public void UpdateCameraViewMatricesUniform(Camera camera, Hashtable uniforms)
        {
            var cameras = this.GetCameraArray(camera);

            for (var i = 0; i < cameras.Count; i++)
            {
                mat4[i] = cameras[i].MatrixWorldInverse;
            }

            if (uniforms.ContainsKey("viewMatrices"))
                uniforms["viewMatrices"] = mat4;
            else
                uniforms.Add("viewMatrices", mat4);

        }

        public void UpdateObjectMatricesUniforms(Object3D object3D, Camera camera, Hashtable uniforms)
        {
        }

        public bool IsMultiviewCompatible(Camera camera)
        {
            return true;
        }

        public void ResizeRenderTarget(Camera camera)
        {
        }

        public void AttachCamera(Camera camera)
        {
        }

        public void DetachCamera(Camera camera)
        {
        }

        public void Flush(Camera camera)
        {
        }
    }
}
