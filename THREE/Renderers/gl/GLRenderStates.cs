using System;
using System.Collections;

namespace THREE
{
    public class GLRenderStates
    {
        public Hashtable renderStates = new Hashtable();

        public GLRenderStates()
        {
        }

        public void OnSceneDispose(object sender,EventArgs e)
        {
            var scene = sender as Scene;

            scene.Disposed -= OnSceneDispose;
            renderStates.Remove(scene);
        }

        public GLRenderState Get(Scene scene, Camera camera)
        {
            GLRenderState renderState;

            if (!renderStates.Contains(scene))
            {
                renderState = new GLRenderState();
                renderStates.Add(scene, new Hashtable());
                (renderStates[scene] as Hashtable).Add(camera, renderState);

                scene.Disposed += OnSceneDispose;
            }
            else
            {
                if (!(renderStates[scene] as Hashtable).Contains(camera))
                {
                    renderState = new GLRenderState();
                    (renderStates[scene] as Hashtable).Add(camera, renderState);
                }
                else
                {
                    renderState = (renderStates[scene] as Hashtable)[camera] as GLRenderState;
                }
            }

            return renderState;
        }
    }
}
