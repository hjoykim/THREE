using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Utils
{
    public class GeometryWithMaterialTemplate : MaterialExampleTemplate
    {
        public Mesh appliedMesh;
        public float step = 0.0f;
        public Mesh groundPlane;
        int wireframeLinejoinIndex = 0;
        int wireframeLinecapIndex = 0;
        int appliedMaterialIndex = 0;

        float width = 20;
        float height = 20;
        float widthSegment = 4;
        float heightSegment = 4;
        public Material appliedNormalMaterial = null;
        public Material appliedStandardMaterial = null;

        public GeometryWithMaterialTemplate() : base() { }
 
        public override void InitLighting()
        {
            DemoUtils.InitDefaultLighting(this.scene);
        }



        public virtual BufferGeometry BuildGeometry()
        {
            return new PlaneBufferGeometry(width, height, widthSegment, heightSegment);
        }
        public virtual void BuildMesh()
        {
            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);
            groundPlane.Position.Y = -10;

            appliedMesh = DemoUtils.AppliyMeshNormalMaterial(BuildGeometry(), ref appliedNormalMaterial);

            appliedMesh.CastShadow = true;

            scene.Add(appliedMesh);

        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void Init()
        {
            base.Init();

            BuildMesh();

            AddGuiControlsAction = ShowControls;

        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            renderer.Render(scene, camera);

            appliedMesh.Rotation.Y = step += 0.001f;
            appliedMesh.Rotation.X = step;
            appliedMesh.Rotation.Z = step;

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

            if (ImGui.Combo("appliedMaterial", ref appliedMaterialIndex, "meshNormal\0meshStandrad\0"))
            {
                Redraw(); // material changed
            }
            if (rebuildGeometry)
            {
                // parameter changed
                RebuildGeometry();

            }
            AddCastShadow();
            AddGroundPlaneVisible();

        }
        public virtual void ShowControls()
        {

            AddGeometrySettings();
            AddBasicMaterialSettings(appliedMesh.Material, "THREE.Material");
            if (appliedMesh.Material is MeshNormalMaterial)
                AddNormaterialSettings(appliedMesh.Material);
            else
                AddSpecificMaterialSettings(appliedMesh.Material,"THREE.StandardMaterial");
 
        }


        public override void AddEmissivePicker(Material material)
        {
            System.Numerics.Vector3 emissive = new System.Numerics.Vector3(material.Emissive.Value.R, material.Emissive.Value.G, material.Emissive.Value.B);
            if (ImGui.ColorPicker3("emissive", ref emissive))
            {
                material.Emissive = new Color(emissive.X, emissive.Y, emissive.Z);
            }
        }

        public override void AddRoughness(Material material)
        {
            ImGui.SliderFloat("roughness", ref material.Roughness, 0, 1);
        }
        public override void AddMetalness(Material material)
        {
            ImGui.SliderFloat("metalness", ref material.Metalness, 0, 1);
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
