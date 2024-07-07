using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using System.Collections.Generic;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using Vector3 = THREE.Vector3;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter10
{
    [Example("00-Template", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class TemplateExample : Example
    {

        public Mesh polyhedronMesh;

        public Mesh sphereMesh;

        public Mesh cubeMesh;


        public Dictionary<string, Material> materialsLib = new Dictionary<string, Material>();
        public TemplateExample() : base()
        {

        }

        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(0, 20, 40);
            camera.LookAt(Vector3.Zero());
        }
        public override void InitLighting()
        {
            base.InitLighting();
            DemoUtils.InitDefaultDirectionalLighting(scene);
        }
        public override void InitCameraController()
        {
            base.InitCameraController();
            controls.StaticMoving = false;
            controls.RotateSpeed = 3.0f;
            controls.ZoomSpeed = 2;
            controls.PanSpeed = 2;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.2f;
        }
        public override void Init()
        {
            base.Init();

            SetGeometryWithTexture();

            AddGuiControlsAction = ControlSetting;
        }
        public virtual void SetGeometryWithTexture()
        {
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;


            scene.Add(new AmbientLight(new Color(0x444444)));

            var polyhedron = new IcosahedronBufferGeometry(8, 0);
            polyhedronMesh = AddGeometry(scene, polyhedron, "polyhedron", TextureLoader.Load("../../../../assets/textures/general/metal-rust.jpg"));
            polyhedronMesh.Position.X = 20;

            var sphere = new SphereBufferGeometry(5, 20, 20);
            sphereMesh = AddGeometry(scene, sphere, "sphere", TextureLoader.Load("../../../../assets/textures/general/floor-wood.jpg"));

            var cube = new BoxBufferGeometry(10, 10, 10);
            cubeMesh = AddGeometry(scene, cube, "cube", TextureLoader.Load("../../../../assets/textures/general/brick-wall.jpg"));
            cubeMesh.Position.X = -20;
        }
        public override void Render()
        {
            polyhedronMesh.Rotation.X += 0.01f;
            sphereMesh.Rotation.Y += 0.01f;
            cubeMesh.Rotation.Z += 0.01f;
            base.Render();
        }
        


        public virtual Mesh AddGeometry(Scene scene,Geometry geometry,string name,Texture texture)
        {
            var mat = new MeshStandardMaterial()
            {
                Map = texture,
                Metalness = 0.2f,
                Roughness=0.07f
            };

            var mesh = new Mesh(geometry, mat);

            mesh.CastShadow = true;

            scene.Add(mesh);

            materialsLib.Add(name, mat);

            return mesh;
        }
        public virtual Mesh AddGeometryWithMaterial(Scene scene, BufferGeometry geometry, string name, Material material)
        {
            var mesh = new Mesh(geometry, material);
            mesh.CastShadow = true;

            scene.Add(mesh);

            materialsLib.Add(name, material);
            return mesh;
        }
     

        public void ControlSetting()
        {
            foreach (var item in materialsLib)
            {

                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }
        }
        public virtual void AddBasicMaterialSettings(Material material,string name)
        {
            int currentSide = material.Side;
            int shadowSide = material.ShadowSide == null ? 0 : material.ShadowSide.Value;
            if (ImGui.TreeNode(name))
            {
                ImGui.Text($"id={material.Id}");
                ImGui.Text($"uuid={material.Uuid}");
                ImGui.Text($"name={material.Name}");
                ImGui.SliderFloat("opacity", ref material.Opacity, 0.0f, 1.0f);
                ImGui.Checkbox("transparent", ref material.Transparent);
                ImGui.Checkbox("visible", ref material.Visible);
                if (ImGui.Combo("side", ref currentSide, "FrontSide\0BackSide\0BothSide\0"))
                {
                    material.Side = currentSide;
                }
                ImGui.Checkbox("colorWrite", ref material.ColorWrite);
                if (ImGui.Checkbox("flatShading", ref material.FlatShading))
                {
                    material.NeedsUpdate = true;
                }
                ImGui.Checkbox("premultipliedAlpha", ref material.PremultipliedAlpha);
                ImGui.Checkbox("dithering", ref material.Dithering);
                if (ImGui.Combo("shadowSide", ref shadowSide, "FrontSide\0BackSide\0BothSide\0"))
                {
                    material.ShadowSide = shadowSide;
                }
                ImGui.Checkbox("fog", ref material.Fog);               
                ImGui.TreePop();
            }
        }
        public virtual void AddSpecificMaterialSettings(Material material,string name)
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
                        if (ImGui.TreeNode("Material Colors"))
                        {
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
                            ImGui.TreePop();
                        }
                        ImGui.SliderFloat("metalness", ref material.Metalness, 0, 1);
                        ImGui.SliderFloat("roughness", ref material.Roughness, 0, 1);
                        ImGui.Checkbox("wireframe", ref material.Wireframe);
                        break;
                }
                ImGui.TreePop();
            }
        }

    }
}
