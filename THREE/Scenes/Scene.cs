
namespace THREE
{
    public class Scene : Object3D
    {
        public Fog Fog;

        public Material OverrideMaterial;

        public bool AutoUpdate;

        public object Background = null;

        public bool ClearBeforeRender = true;

        public Texture Environment;

        public Scene()
        {
            this.type = "Scene";
            this.Background = null;
            this.Environment = null;
            this.Fog = null;
            this.OverrideMaterial = null;

            this.AutoUpdate = true;
        }

        public virtual void Resize(float width, float height)
        {

        }        
    }
}
