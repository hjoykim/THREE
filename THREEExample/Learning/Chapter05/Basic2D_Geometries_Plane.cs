using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Core;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter05
{
    [Example("01.Basic-2D-Geometries-Plane", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic2D_Geometries_Plane : Example
    {
        public Mesh appliedMesh;

        public Scene scene;

        public Camera camera;

        public TrackballControls controls;

        public float step = 0.0f;

        public ImGuiManager imGuiManager;

        public Mesh groundPlane;
        int wireframeLinejoinIndex = 0;
        int wireframeLinecapIndex = 0;
        int appliedMaterialIndex = 0;
        public Basic2D_Geometries_Plane() : base()
        {
            camera = new THREE.Cameras.PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
        }
        public virtual void InitLighting()
        {
            DemoUtils.InitDefaultLighting(this.scene);
        }
         float width = 20;
         float height = 20;
         float widthSegment = 4;
         float heightSegment = 4;

        public Material appliedNormalMaterial=null;
        public Material appliedStandardMaterial=null;

        public virtual BufferGeometry BuildGeometry()
        {
            return new PlaneBufferGeometry(width, height, widthSegment, heightSegment);
        }
        public virtual void BuildMesh()
        {
            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);
            groundPlane.Position.Y = -10;

            InitLighting();

            appliedMesh = DemoUtils.AppliyMeshNormalMaterial(BuildGeometry(),ref appliedNormalMaterial);

            appliedMesh.CastShadow = true;

            scene.Add(appliedMesh);

        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public virtual void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(-30, 40, 30);
            camera.LookAt(scene.Position);
        }
        public virtual void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
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

        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            InitCamera();

            InitCameraController();

            BuildMesh();

            imGuiManager = new ImGuiManager(this.glControl);

        }

        public override void Render()
        {
            controls.Update();
            renderer.Render(scene, camera);

            appliedMesh.Rotation.Y = step += 0.001f;
            appliedMesh.Rotation.X = step;
            appliedMesh.Rotation.Z = step;

            ShowControls();
        }

        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);

            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }

        public virtual void RebuildGeometry()
        {
            scene.Remove(appliedMesh);
            appliedMesh.Geometry = BuildGeometry();
            scene.Add(appliedMesh);
        }
        public virtual void Redraw()
        {
            scene.Remove(appliedMesh);
            if (appliedMaterialIndex == 0)
                appliedMesh = DemoUtils.AppliyMeshNormalMaterial(appliedMesh.Geometry, ref appliedNormalMaterial);
            else
                appliedMesh = DemoUtils.AppliyMeshStandardMaterial(appliedMesh.Geometry, ref appliedStandardMaterial);

            scene.Add(appliedMesh);
        }
        public virtual void AddCastShadow()
        {
            ImGui.Checkbox("castShadow", ref appliedMesh.CastShadow);
        }
        public virtual void AddGroundPlaneVisible()
        {
            ImGui.Checkbox("groundPlaneVisible", ref groundPlane.Material.Visible);
        }
        public virtual bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("width", ref width, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("height", ref height, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("widthSegment", ref widthSegment, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderFloat("heightSegment", ref heightSegment, 0, 10)) rebuildGeometry = true;

            return rebuildGeometry;
        }
        public virtual void AddGeometrySettings()
        {
            bool rebuildGeometry = AddGeometryParameter();

            if(ImGui.Combo("appliedMaterial",ref appliedMaterialIndex,"meshNormal\0meshStandrad\0"))
            {
                Redraw(); // material changed
            }
            if(rebuildGeometry)
            {
                // parameter changed
                RebuildGeometry();
               
            }
            AddCastShadow();
            AddGroundPlaneVisible();
           
        }
        public virtual void ShowControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            AddGeometrySettings();
            AddBasicMaterialSettings(appliedMesh.Material, "THREE.Material");
            if (appliedMesh.Material is MeshNormalMaterial)
                AddNormaterialSettings(appliedMesh.Material);
            else 
                AddStandardMaterialSettings(appliedMesh.Material);
            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        public virtual void AddBasicMaterialSettings(Material material, string name)
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
        public virtual void AddColorPicker(Material material)
        {
            System.Numerics.Vector3 color = new System.Numerics.Vector3(material.Color.Value.R, material.Color.Value.G, material.Color.Value.B);
            if (ImGui.ColorPicker3("color", ref color))
            {
                material.Color = new Color(color.X, color.Y, color.Z);
            }
        }
        public virtual void AddEmissivePicker(Material material)
        {
            System.Numerics.Vector3 emissive = new System.Numerics.Vector3(material.Emissive.Value.R, material.Emissive.Value.G, material.Emissive.Value.B);
            if (ImGui.ColorPicker3("emissive", ref emissive))
            {
                material.Emissive = new Color(emissive.X, emissive.Y, emissive.Z);
            }
        }
        public virtual void AddSpecularPicker(Material material)
        {

        }
        public virtual void AddShiness(Material material)
        {

        }
        public virtual void AddRoughness(Material material)
        {
            ImGui.SliderFloat("roughness", ref material.Roughness, 0, 1);
        }
        public virtual void AddMetalness(Material material)
        {
            ImGui.SliderFloat("metalness", ref material.Metalness, 0, 1);
        }
        public virtual void AddWireframeProperty(Material material)
        {
            ImGui.Checkbox("wireframe", ref material.Wireframe);
            //ImGui.SliderFloat("wireframeLineWidth", ref material.WireframeLineWidth, 0, 20);
        }
        public virtual void AddWireframeLineProperty(Material material)
        {
            //if (ImGui.Combo("wireframeLinejoin", ref wireframeLinejoinIndex, "round\0bevel\0miter\0"))
            //{
            //    if (wireframeLinejoinIndex == 0) material.WireframeLineJoin = "round";
            //    else if (wireframeLinejoinIndex == 1) material.WireframeLineJoin = "bevel";
            //    else material.WireframeLineJoin = "miter";
            //}
            //if (ImGui.Combo("wireframeLinecap", ref wireframeLinecapIndex, "butt\0round\0square\0"))
            //{
            //    if (wireframeLinecapIndex == 0) material.WireframeLineCap = "round";
            //    else if (wireframeLinecapIndex == 1) material.WireframeLineCap = "bevel";
            //    else material.WireframeLineCap = "miter";
            //}
        }
        public virtual void AddStandardMaterialSettings(Material material)
        {
            Color? materialColor = material.Color;
            Color? emissiveColor = material.Emissive;

            if (ImGui.TreeNode("THREE.MeshStandardMaterial"))
            {
                if (materialColor != null)
                    AddColorPicker(material);
                if (emissiveColor != null)
                    AddEmissivePicker(material);
                AddSpecularPicker(material);
                AddShiness(material);
                AddMetalness(material);
                AddRoughness(material);
                AddWireframeProperty(material);
                ImGui.TreePop();
            }            
        }
        public virtual void AddNormaterialSettings(Material material)
        {
            if (ImGui.TreeNode("THREE.MeshNormalMaterial"))
            {
                AddWireframeProperty(material);
                //AddWireframeLineProperty(material);
                ImGui.TreePop();
            }
        }
    }
}
