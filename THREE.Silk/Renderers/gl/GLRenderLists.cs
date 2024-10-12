using System.Collections;

namespace THREE
{
    [Serializable]
    public class GLRenderLists : Hashtable
    {
        GLProperties properties;
        public GLRenderLists(GLProperties properties)
        {
            this.properties = properties;
        }

        public GLRenderList Get(Scene scene, int renderCallDepth)
        {
            GLRenderList list = null;

            if (!this.ContainsKey(scene))
            {
                list = new GLRenderList(properties);
                List<GLRenderList> lists = new List<GLRenderList>() { list };

                this.Add(scene, lists);
            }
            else
            {

                if (renderCallDepth >= (this[scene] as List<GLRenderList>).Count)
                {
                    list = new GLRenderList(properties);
                    (this[scene] as List<GLRenderList>).Add(list);
                }
                else
                {
                    list = (this[scene] as List<GLRenderList>)[renderCallDepth];
                }
            }

            return list;
        }
    }
}
