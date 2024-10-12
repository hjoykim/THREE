using ImGuiNET;
using System.Collections.Generic;
using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example.Learning.Utils
{
    public class MaterialExampleTemplate : Example
    {

        public Dictionary<string, Material> materialsLib = new Dictionary<string, Material>();

        int wireframeLinejoinIndex = 0;
        int wireframeLinecapIndex = 0;

        public MaterialExampleTemplate() : base()
        {
        }



        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 20, 40);
            camera.LookAt(new THREE.Vector3(10,0,0));
        }

        public virtual void MaterialsGUIControls()
        {
            foreach (var item in materialsLib)
            {
                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }
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
    }
}
