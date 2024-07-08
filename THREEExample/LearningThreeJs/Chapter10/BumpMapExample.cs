using OpenTK;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Chapter10;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using ImGuiNET;


namespace THREEExample.Learning.Chapter09
{
    [Example("08-bump-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class BumpMapExample : TemplateExample
    {
        MeshStandardMaterial cubeMaterialWithBumpMap;
        public BumpMapExample() : base()
        {

        }
        public override void SetGeometryWithTexture()
        {
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;

            scene.Add(new AmbientLight(new THREE.Color(0x444444)));

            var cube = new BoxBufferGeometry(16, 16, 16);
            var cubeMaterial = new MeshStandardMaterial();
            cubeMaterial.Map = TextureLoader.Load("../../../../assets/textures/stone/stone.jpg");
            cubeMaterial.Metalness = 0.2f;
            cubeMaterial.Roughness = 0.07f;

            cubeMaterialWithBumpMap = (MeshStandardMaterial)cubeMaterial.Clone();
            cubeMaterialWithBumpMap.BumpMap = TextureLoader.Load("../../../../assets/textures/stone/stone-bump.jpg");

            var cube1 = AddGeometryWithMaterial(scene, cube, "cube-1", cubeMaterial);
            cube1.Position.X = -17;
            cube1.Rotation.Y = 1.0f / 3 * (float)System.Math.PI;

            var cube2 = AddGeometryWithMaterial(scene, cube, "cube-2", cubeMaterialWithBumpMap);
            cube2.Position.X = 17;
            cube2.Rotation.Y = -1.0f / 3 * (float)System.Math.PI;
        }
        public override void Init()
        {
            base.Init();

            AddGuiControlsAction = () =>
            {
                foreach (var item in materialsLib)
                {

                    AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                    AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
                }
                ImGui.SliderFloat("bumpScale", ref cubeMaterialWithBumpMap.BumpScale, -1.0f, 1.0f);
            };
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;
            controls.Update();
            this.renderer.Render(scene, camera);                     
        }       
    }
}
