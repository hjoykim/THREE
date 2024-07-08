using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter10
{
    [Example("17-env-map-static", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class EnvMapStaticExample : TemplateExample
    {
        Mesh cube1, sphere1;
        bool refraction = false;
        public EnvMapStaticExample() : base()
        {

        }

        public override void SetGeometryWithTexture()
        {
            List<string> urls = new List<string>{
                "../../../../assets/textures/cubemap/flowers/right.png",
                "../../../../assets/textures/cubemap/flowers/left.png",
                "../../../../assets/textures/cubemap/flowers/top.png",
                "../../../../assets/textures/cubemap/flowers/bottom.png",
                "../../../../assets/textures/cubemap/flowers/front.png",
                "../../../../assets/textures/cubemap/flowers/back.png"
            };

            var cubeTexture = CubeTextureLoader.Load(urls);
            scene.Background = cubeTexture;

            var cubeMaterial = new MeshStandardMaterial
            {
                EnvMap = scene.Background as CubeTexture,
                Color = new THREE.Color(0xffffff),
                Metalness = 1.0f,
                Roughness = 0.0f,
            };
            var sphereMaterial = cubeMaterial.Clone() as MeshStandardMaterial;
            sphereMaterial.NormalMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_NORM.jpg");
            sphereMaterial.AoMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_OCC.jpg");

            var cube = new BoxBufferGeometry(16, 12, 12);
            cube1 = AddGeometryWithMaterial(scene, cube, "cube", cubeMaterial);
            cube1.Position.X = -15;
            cube1.Rotation.Y = -1 / 3 * (float)Math.PI;

            var sphere = new SphereBufferGeometry(10, 50, 50);
            sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere", sphereMaterial);
            sphere1.Position.X = 15;
            (scene.Background as CubeTexture).Mapping = Constants.CubeReflectionMapping;
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
                if (ImGui.Checkbox("refraction", ref refraction))
                {
                    if (refraction == true)
                    {
                        (scene.Background as CubeTexture).Mapping = Constants.CubeRefractionMapping;
                    }
                    else
                    {
                        (scene.Background as CubeTexture).Mapping = Constants.CubeReflectionMapping;
                    }
                    cube1.Material.NeedsUpdate = true;
                    sphere1.Material.NeedsUpdate = true;
                }
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
            cube1.Rotation.Y += 0.01f;
            sphere1.Rotation.Y -= 0.01f;
        }
    }
}
