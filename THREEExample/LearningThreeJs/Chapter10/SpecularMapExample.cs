using System;
using THREE;

namespace THREEExample.Learning.Chapter10
{
    [Example("16-specular-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class SpecularMapExample : TemplateExample
    {
        Mesh sphere1;
        public SpecularMapExample() : base()
        {

        }
        public override void InitLighting()
        {
            base.InitLighting();
            (scene.GetObjectByName("ambientLight") as AmbientLight).Color.SetHex(0x050505);
        }
        public override void SetGeometryWithTexture()
        {           

            var earthMaterial = new MeshPhongMaterial
            {
                Map = TextureLoader.Load("../../../../assets/textures/earth/Earth.png"),
                NormalMap = TextureLoader.Load("../../../../assets/textures/earth/EarthNormal.png"),
                SpecularMap = TextureLoader.Load("../../../../assets/textures/earth/EarthSpec.png"),
                NormalScale = new THREE.Vector2(6, 6)
            };

            var sphere = new SphereBufferGeometry(9, 50, 50);
            sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere", earthMaterial);
            sphere1.Rotation.Y = 1 / 6 * (float)Math.PI;
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;
            controls.Update();

            this.renderer.Render(scene, camera);

            sphere1.Rotation.Y -= 0.01f;

        }       
    }
}
