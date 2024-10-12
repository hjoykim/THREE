using System;
using System.Collections;

namespace THREE
{
    [Serializable]
    public class GLRenderStates
    {
        public Hashtable renderStates = new Hashtable();
        private GLExtensions extensions;
        private GLCapabilities capabilities;
        public GLRenderStates(GLExtensions extensions, GLCapabilities capabilities)
        {
            this.extensions = extensions;
            this.capabilities = capabilities;
        }

        public void OnSceneDispose(object sender, EventArgs e)
        {
            var scene = sender as Scene;

            scene.Disposed -= OnSceneDispose;
            renderStates.Remove(scene);
        }

        public GLRenderState Get(Scene scene, int renderCallDepth=0)
        {
            GLRenderState renderState;

            if (!renderStates.Contains(scene))
            {
                renderState = new GLRenderState(extensions,capabilities);
                List<GLRenderState> list = new List<GLRenderState>() { renderState };
                renderStates.Add(scene, list);

                scene.Disposed += OnSceneDispose;
            }
            else
            {
                if (renderCallDepth >= (renderStates[scene] as List<GLRenderState>).Count)
                {
                    renderState = new GLRenderState(extensions,capabilities);
                    (renderStates[scene] as List<GLRenderState>).Add(renderState);
                }
                else
                {
                    renderState = (renderStates[scene] as List<GLRenderState>)[renderCallDepth];
                }
            }

            return renderState;
        }
    }
}
