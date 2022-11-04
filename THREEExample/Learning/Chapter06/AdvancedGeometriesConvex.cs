using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREEExample.Learning.Chapter04;
using THREEExample.Learning.Utils;
using Vector3 = THREE.Math.Vector3;

namespace THREEExample.Learning.Chapter06
{
    [Example("01.Advanced-3D-Geometries-Convex", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class AdvancedGeometriesConvex : MaterialExampleTemplate
    {
        public Mesh appliedMesh;

        public Mesh groundPlane;

        float step = 0;

        public Object3D spGroup;

        public Material appliedMaterial=null;

        int materialIndex = 0;

        public BufferGeometry geometry;

        public bool redrawButtonEnabled = true;
        public AdvancedGeometriesConvex() : base()
        {
           

        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            Init();
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
        public virtual void NewGeometry()
        {
            geometry = GeneratePoints();
        }
        public virtual void Init()
        {
            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);

            groundPlane.Position.Y = -30;

            DemoUtils.InitDefaultLighting(this.scene);



            NewGeometry();

            appliedMesh = DemoUtils.AppliyMeshNormalMaterial(geometry, ref appliedMaterial);

            materialsLib.Add(appliedMaterial.type, appliedMaterial);

            

            appliedMesh.CastShadow = true;
            scene.Add(appliedMesh);
            scene.Add(spGroup);
        }
        public virtual BufferGeometry GeneratePoints()
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
                materialsLib.Remove(appliedMaterial.type);
                if (materialIndex == 0)
                {

                    appliedMesh = DemoUtils.AppliyMeshNormalMaterial(geometry, ref appliedMaterial);
                }
                else
                {
                    appliedMesh = DemoUtils.AppliyMeshStandardMaterial(geometry, ref appliedMaterial);

                }
                materialsLib.Add(appliedMaterial.type, appliedMaterial);
                scene.Add(appliedMesh);
            }
            if (redrawButtonEnabled)
            {
                if (ImGui.Button("redraw"))
                {
                    scene.Remove(appliedMesh);
                    geometry = GeneratePoints();
                    appliedMesh = new Mesh(geometry, appliedMaterial);
                    scene.Add(appliedMesh);
                }
            }
            ImGui.Checkbox("castShadow", ref appliedMesh.CastShadow);
            ImGui.Checkbox("groundPlandVisible", ref groundPlane.Visible);
        }
        public override void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");

            AddSettings();

            foreach (var item in materialsLib)
            {
                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }

            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
    }
}
