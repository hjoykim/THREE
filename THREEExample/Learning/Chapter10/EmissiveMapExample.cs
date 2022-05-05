using ImGuiNET;
using OpenTK;
using System;
using System.Diagnostics;
using THREE.Core;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter10
{
    [Example("15-emissive-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class EmissiveMapExample : TemplateExample
    {
        SpotLight spotLight;

        public EmissiveMapExample() : base()
        {

        }
        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;

            this.renderer = new THREE.Renderers.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -8;

            DemoUtils.InitDefaultLighting(scene);

            spotLight = (SpotLight)scene.GetObjectByName("spotLight");
            spotLight.Intensity = 0.1f;
            scene.Remove(scene.GetObjectByName("ambientLight"));

            var cubeMaterial = new MeshStandardMaterial
            {
                Emissive = new THREE.Math.Color(0xffffff),
                EmissiveMap = TextureLoader.Load("../../../assets/textures/emissive/lava.png"),
                NormalMap = TextureLoader.Load("../../../assets/textures/emissive/lava-normals.png"),
                MetalnessMap = TextureLoader.Load("../../../assets/textures/emissive/lava-smoothness.png"),
                Metalness = 1,
                Roughness = 0.4f,
                NormalScale = new THREE.Math.Vector2(4, 4)
            };

            var cube = new BoxBufferGeometry(16, 16, 16);
            var cube1 = AddGeometryWithMaterial(scene, cube, "cube", cubeMaterial);
            cube1.Rotation.Y = 1 / 3 * (float)System.Math.PI;
            cube1.Position.X = -12;

            var sphere = new SphereBufferGeometry(9, 50, 50);
            var sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere", (MeshStandardMaterial)cubeMaterial.Clone());
            sphere1.Rotation.Y = 1 / 6 * (float)System.Math.PI;
            sphere1.Position.X = 15;
        }
        public override void Render()
        {
            controls.Update();

            this.renderer.Render(scene, camera);

            ShowGUIControls();
        }
        
        public override void AddSpecificMaterialSettings(Material material, string name)
        {
            Color materialColor = material.Color.Value;
            Color emissiveColor = material.Emissive.Value;
            System.Numerics.Vector3 color = new System.Numerics.Vector3(materialColor.R, materialColor.G, materialColor.B);
            System.Numerics.Vector3 emissive = new System.Numerics.Vector3(emissiveColor.R, emissiveColor.G, emissiveColor.B);
            if (ImGui.TreeNode(name))
            {
                switch (material.type)
                {
                    case "MeshNormalMaterial":
                        ImGui.Checkbox("wireframe", ref material.Wireframe);
                        break;
                    case "MeshPhongMaterial":
                        ImGui.SliderFloat("shininess", ref material.Shininess, 0, 100);
                        break;
                    case "MeshStandardMaterial":
                        if (ImGui.ColorPicker3("color", ref color))
                        {
                            Color mColor = new Color(color.X, color.Y, color.Z);
                            material.Color = mColor;
                        }
                        if (ImGui.ColorPicker3("emissive", ref emissive))
                        {
                            Color eColor = new Color(emissive.X, emissive.Y, emissive.Z);
                            material.Emissive = eColor;
                        }
                        ImGui.SliderFloat("metalness", ref material.Metalness, 0, 1);
                        ImGui.SliderFloat("roughness", ref material.Roughness, 0, 1);
                        ImGui.Checkbox("wireframe", ref material.Wireframe);
                        break;
                }
                ImGui.SliderFloat("lightIntessity", ref spotLight.Intensity, 0.0f, 1.0f);
                ImGui.TreePop();
            }
        }
    }
}
