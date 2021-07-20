using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;
using THREE.Textures;

namespace THREE.Scenes
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
            this.Fog = null;
            this.OverrideMaterial = null;

            this.AutoUpdate = true;
        }
        public virtual void Resize(float width, float height)
        {

        }        
    }
}
