using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Materials;
using THREE.Math;
using THREE.Scenes;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter04
{
    public class MaterialExampleTemplate : Example
    {
        public Scene scene;

        public Camera camera;

        public TrackballControls controls;

        public ImGuiManager imGuiManager;


        public Dictionary<string, Material> materialsLib = new Dictionary<string, Material>();

        int wireframeLinejoinIndex = 0;
        int wireframeLinecapIndex = 0;

        public MaterialExampleTemplate() : base()
        {
            camera = new THREE.Cameras.PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            renderer.Render(scene, camera);
            ShowGUIControls();
        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(new Color().SetHex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public virtual void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(0, 20, 40);
            camera.LookAt(new THREE.Math.Vector3(10,0,0));
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

            imGuiManager = new ImGuiManager(this.glControl);           
        }
        public virtual void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");

            foreach (var item in materialsLib)
            {
                AddBasicMaterialSettings(item.Value,item.Key+"-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }

            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
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
            if(ImGui.ColorPicker3("color",ref color))
            {
                material.Color = new Color(color.X, color.Y, color.Z);
            }
        }
        public virtual void AddEmissivePicker(Material material)
        {

        }
        public virtual void AddSpecularPicker(Material material)
        {

        }
        public virtual void AddShiness(Material material)
        {

        }
        public virtual void AddRoughness(Material material)
        {

        }
        public virtual void AddMetalness(Material material)
        {

        }
        public virtual void AddWireframeProperty(Material material)
        {
            ImGui.Checkbox("wireframe", ref material.Wireframe);
            ImGui.SliderFloat("wireframeLineWidth", ref material.WireframeLineWidth, 0, 20);
        }
        public virtual void AddWireframeLineProperty(Material material)
        {
            if(ImGui.Combo("wireframeLinejoin",ref wireframeLinejoinIndex, "round\0bevel\0miter\0"))
            {
                if (wireframeLinejoinIndex == 0) material.WireframeLineJoin = "round";
                else if (wireframeLinejoinIndex == 1) material.WireframeLineJoin = "bevel";
                else  material.WireframeLineJoin = "miter";
            }
            if (ImGui.Combo("wireframeLinecap", ref wireframeLinecapIndex, "butt\0round\0square\0"))
            {
                if (wireframeLinecapIndex == 0) material.WireframeLineCap = "round";
                else if (wireframeLinecapIndex == 1) material.WireframeLineCap = "bevel";
                else material.WireframeLineCap = "miter";
            }
        }
        public virtual void AddSpecificMaterialSettings(Material material, string name)
        {
            Color? materialColor = material.Color;
            Color? emissiveColor = material.Emissive;
            
            if (ImGui.TreeNode(name))
            {
                if(materialColor!=null)
                    AddColorPicker(material);
                if(emissiveColor!=null)
                    AddEmissivePicker(material);
                AddSpecularPicker(material);
                ImGui.TreePop();
            }
            AddShiness(material);
            AddMetalness(material);
            AddRoughness(material);
            AddWireframeProperty(material);
            AddWireframeLineProperty(material);
        }
    }
}
