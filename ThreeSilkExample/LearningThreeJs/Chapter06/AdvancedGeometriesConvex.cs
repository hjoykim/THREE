using ImGuiNET;
using System;
using System.Collections.Generic;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;
using Vector3 = THREE.Vector3;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("01.Advanced-3D-Geometries-Convex", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class AdvancedGeometriesConvex : GeometryWithMaterialTemplate
    {
        public Object3D spGroup=null;



        int materialIndex = 0;


        public bool redrawButtonEnabled = true;
        public AdvancedGeometriesConvex() : base() { }

        public override void Init()
        {
            base.Init();
            groundPlane.Position.Y = -30;
            //NewGeometry();
            materialsLib.Add(appliedNormalMaterial.type, appliedNormalMaterial);
        }
        public override void Render()
        {
            appliedMesh.Rotation.Y = step += 0.005f;
            appliedMesh.Rotation.X = step;
            appliedMesh.Rotation.Z = step;

            if (spGroup != null)
            {
                spGroup.Rotation.Y = step;
                spGroup.Rotation.X = step;
                spGroup.Rotation.Z = step;
            }

            base.Render();

        }


        public override BufferGeometry BuildGeometry()
        {
            if (spGroup != null) scene.Remove(spGroup);
            spGroup = new Object3D();
            var points = new List<Vector3>();
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                var randomX = -15 + MathUtils.NextFloat(0, 30);
                var randomY = -15 + MathUtils.NextFloat(0, 30);
                var randomZ = -15 + MathUtils.NextFloat(0, 30);

                points.Add(new Vector3(randomX, randomY, randomZ));
            }


            var material = new MeshBasicMaterial()
            {
                Color = Color.Hex(0xff0000),
                Transparent = false
            };

            points.ForEach(delegate (Vector3 point) {

                var spGeom = new SphereBufferGeometry(0.2f);
                var spMesh = new Mesh(spGeom, material);
                spMesh.Position.Copy(point);
                spGroup.Add(spMesh);
            });
            scene.Add(spGroup);
            // use the same points to create a convexgeometry
            var convexGeometry = new ConvexBufferGeometry(points.ToArray());

            convexGeometry.ComputeVertexNormals();

            convexGeometry.ComputeFaceNormals();

            convexGeometry.NormalsNeedUpdate = true;

            return convexGeometry;
        }
        public virtual void AddSettings()
        {
            if (ImGui.Combo("appliedMaterial", ref materialIndex, "meshNormal\0meshStandard\0"))
            {
                scene.Remove(appliedMesh);
                materialsLib.Remove(appliedNormalMaterial.type);
                if (materialIndex == 0)
                {

                    appliedMesh = DemoUtils.AppliyMeshNormalMaterial(appliedMesh.Geometry, ref appliedNormalMaterial);
                }
                else
                {
                    appliedMesh = DemoUtils.AppliyMeshStandardMaterial(appliedMesh.Geometry, ref appliedNormalMaterial);

                }
                materialsLib.Add(appliedNormalMaterial.type, appliedNormalMaterial);
                scene.Add(appliedMesh);
            }
            if (redrawButtonEnabled)
            {
                if (ImGui.Button("redraw"))
                {
                    RebuildGeometry();
                }
            }
            ImGui.Checkbox("castShadow", ref appliedMesh.CastShadow);
            ImGui.Checkbox("groundPlandVisible", ref groundPlane.Visible);
        }
        public override void ShowControls()
        {
            AddSettings();
            foreach (var item in materialsLib)
            {
                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }
        }
    }
}
