using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Geometries;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREE.Helpers
{
    public class PointLightHelper : Mesh
    {
        public Light Light;

        public Color? Color;

        public PointLightHelper(Light light,float? sphereSize=null,Color? color=null) : base() 
        {
            this.Light = light;
            this.Light.UpdateMatrixWorld();


            this.Color = color;

            
            var geometry = new SphereBufferGeometry(sphereSize!=null? sphereSize.Value:1, 4, 2);
            var material = new MeshBasicMaterial() { Wireframe = true, Fog = false };
           

            this.InitGeometry(geometry, material);


            this.Matrix = this.Light.MatrixWorld;
            this.MatrixAutoUpdate = false;

            this.Update();
        }

        public void Update()
        {
            if (this.Color != null)
                this.Material.Color = this.Color;
            else
                this.Material.Color = this.Light.Color;
        }
    }
}
