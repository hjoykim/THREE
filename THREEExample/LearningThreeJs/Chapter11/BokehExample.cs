using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter11
{
    [Example("05-Bokeh", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class BokehExample : EffectComposerTemplate
    {
        MeshStandardMaterial sphereMaterial;
        MeshStandardMaterial boxMaterial1;
        MeshStandardMaterial boxMaterial2;
        Mesh sphere1;
        public BokehExample() :base()
        {

        }
        public override void InitLighting()
        {
            base.InitLighting();
            DemoUtils.InitDefaultLighting(scene);
        }
        public override void Init()
        {
            base.Init();
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene, true);
            groundPlane.Position.Y = -8;

            List<string> urls = new List<string>
            {
                "../../../../assets/textures/cubemap/flowers/right.png",
                "../../../../assets/textures/cubemap/flowers/left.png",
                "../../../../assets/textures/cubemap/flowers/top.png",
                "../../../../assets/textures/cubemap/flowers/bottom.png",
                "../../../../assets/textures/cubemap/flowers/front.png",
                "../../../../assets/textures/cubemap/flowers/back.png"
            };
            sphereMaterial = new MeshStandardMaterial
            {
                EnvMap = CubeTextureLoader.Load(urls),
                NormalMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_NORM.jpg"),
                AoMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_NORM.jpg"),
                Color = Color.Hex(0xffffff),
                Metalness = 1.0f,
                Roughness = 0.3f
            };

            var sphere = new SphereBufferGeometry(5, 50, 50);
            sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere", sphereMaterial);

            boxMaterial1 = new MeshStandardMaterial { Color = Color.Hex(0x0066ff) };
            var m1 = new BoxBufferGeometry(10, 10, 10);
            var m1m = AddGeometryWithMaterial(scene, m1, "m1", boxMaterial1);
            m1m.Position.Z = -40;
            m1m.Position.X = -35;
            m1m.Rotation.Y = 1;

            var m2 = new BoxBufferGeometry(10, 10, 10);
            boxMaterial2 = new MeshStandardMaterial { Color = Color.Hex(0xff6600) };
            var m2m = AddGeometryWithMaterial(scene, m2, "m", boxMaterial2);
            m2m.Position.Z = -40;
            m2m.Position.X = 35;
            m2m.Rotation.Y = -1;

            var totalWidth = 220;
            var nBoxes = 10;
            for (var i = 0; i < nBoxes; i++)
            {
                var box = new BoxBufferGeometry(10, 10, 10);
                var mat = new MeshStandardMaterial { Color = Color.Hex(0x66ff00) };
                var mesh = new Mesh(box, mat);
                mesh.Position.Z = -120;
                mesh.Position.X = -(totalWidth / 2) + (totalWidth / nBoxes) * i;
                mesh.Rotation.Y = i;
                scene.Add(mesh);
            }

            var parameter = new Hashtable
             {
                 {"focus",10 },
                 {"aspect",camera.Aspect },
                 {"aperture", 0.0002f },
                 {"maxblur", 0.1f }
              };

            renderPass = new RenderPass(scene, camera);
            bokehPass = new BokehPass(scene, camera, parameter);
            bokehPass.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(bokehPass);

            AddGuiControlsAction = () =>
            {
                AddShaderControls("bokeh", bokehPass);
            };
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls.Update();
            composer.Render();
            sphere1.Rotation.Y -= 0.01f;

        }
       
        float focus = 10.0f;
        float aperture = 0.0002f;
        float maxblur = 0.1f;
        public void AddShaderControls(string rootName, BokehPass pass)
        {
            AddBasicMaterialSettings(sphereMaterial, "sphere" + "-THREE.Material");
            AddSpecificMaterialSettings(sphereMaterial, "sphere" + "-Material");

            AddBasicMaterialSettings(boxMaterial1, "m1" + "-THREE.Material");
            AddSpecificMaterialSettings(boxMaterial1, "m1" + "-Material");

            AddBasicMaterialSettings(boxMaterial2, "m2" + "-THREE.Material");
            AddSpecificMaterialSettings(boxMaterial2, "m2" + "-Material");

            if (ImGui.TreeNode(rootName))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);

                if(ImGui.SliderFloat("focus",ref focus,1.0f,200.0f))
                {
                    (pass.uniforms["focus"] as GLUniform)["value"] = focus;
                }
                if (ImGui.SliderFloat("aperture", ref aperture, 0.0000f, 0.0005f))
                {
                    (pass.uniforms["aperture"] as GLUniform)["value"] = aperture;
                }
                if (ImGui.SliderFloat("maxblur", ref maxblur, 0.1f, 1.0f))
                {
                    (pass.uniforms["maxblur"] as GLUniform)["value"] = maxblur;
                }
                ImGui.TreePop();
            }
        }

    }
}
