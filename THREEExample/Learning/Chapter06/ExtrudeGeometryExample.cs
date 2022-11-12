using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Extras.Core;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter06
{
    [Example("03.Extrude-Geometry", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class ExtrudeGeometryExample : ExampleTemplateWithMaterial
    {
        public Mesh appliedMesh;

        public Mesh groundPlane;

        float step = 0;

       

        public BufferGeometry geometry;

        int amount = 2;
        int depth =2;
        int bevelThickness = 2;
        float bevelSize = 0.5f;
        bool bevelEnabled = false;
        int bevelSegments = 3;
        int curveSegments = 12;
        int steps = 1;

        public Material appliedMaterial = null;

        public int materialIndex = 0;

        public bool rebuildGeometry = false;

        public ExtrudeGeometryExample() : base() { }
        public override void Init()
        {
            base.Init();



            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);

            groundPlane.Position.Y = -30;

            DemoUtils.InitDefaultLighting(this.scene);           

            geometry = BuildGeometry();
          
            appliedMesh = DemoUtils.AppliyMeshNormalMaterial(geometry);

            appliedMesh.CastShadow = true;

            scene.Add(appliedMesh);

            AddGuiControlsAction = ShowControls;
        }

        public override void Render()
        {
            appliedMesh.Rotation.Y = step += 0.001f;
            appliedMesh.Rotation.X = step;
            appliedMesh.Rotation.Z = step;

            base.Render();
        }
        private Shape DrawShape()
        {
            var shape = new Shape();

            // startpoint
            shape.MoveTo(10, 10, 0);

            // straight line upwards
            shape.LineTo(10, 40, 0);

            // the top of the figure, curve to the right
            shape.BezierCurveTo(15, 25, 25, 25, 30, 40);

            // spline back down
            shape.SplineThru(new List<Vector3>()
            {
                new Vector3(32, 30,0),
                new Vector3(28, 20,0),
                new Vector3(30, 10,0),

              });

            // curve at the bottom
            shape.QuadraticCurveTo(20, 15, 10, 10);

            // add 'eye' hole one
            var hole1 = new Path();
            hole1.AbsEllipse(16, 24, 2, 3, 0, (float)Math.PI * 2, true);
            shape.Holes.Add(hole1);

            // add 'eye hole 2'
            var hole2 = new Path();
            hole2.AbsEllipse(23, 24, 2, 3, 0, (float)Math.PI * 2, true);
            shape.Holes.Add(hole2);

            // add 'mouth'
            var hole3 = new Path();
            hole3.AbsArc(20, 16, 2, 0, (float)Math.PI, true);
            shape.Holes.Add(hole3);

            // return the shape
            return shape;

        }

        public virtual void AddParameterSettings()
        {
            rebuildGeometry = false;
            if (ImGui.SliderInt("amount", ref amount, 0, 20)) rebuildGeometry = true;
            if (ImGui.SliderInt("bevelThickness", ref bevelThickness, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderFloat("bevelSize", ref bevelSize, 0.0f, 10.0f)) rebuildGeometry = true;
            if (ImGui.SliderInt("bevelSegments", ref bevelSegments, 0, 30)) rebuildGeometry = true;
            if (ImGui.SliderInt("curveSegments", ref curveSegments, 1, 20)) rebuildGeometry = true;
            if (ImGui.SliderInt("steps", ref steps, 1, 5)) rebuildGeometry = true;
        }
        private void ShowControls()
        {
            AddParameterSettings();
            AddMaterialSettings();
        }
        public virtual BufferGeometry BuildGeometry()
        {
            Hashtable options = new Hashtable()
            {
                {"amount",amount },
                {"depth",depth },
                {"bevelThickness",bevelThickness},
                {"bevelSize",bevelSize },
                {"bevelEnabled",bevelEnabled },
                {"bevelSegments",bevelSegments },
                {"curveSegments",curveSegments },
                {"steps",steps }
            };
            BufferGeometry geom = new ExtrudeBufferGeometry(DrawShape(), options);
            geom.ApplyMatrix(new Matrix4().MakeTranslation(-20, 0, 0));
            geom.ApplyMatrix(new Matrix4().MakeScale(0.4f, 0.4f, 0.4f));

            return geom;
        }
        public virtual void RebuildGeometry()
        {
            scene.Remove(appliedMesh);
            appliedMesh.Geometry = BuildGeometry();
            scene.Add(appliedMesh);
        }
        public virtual void AddMaterialSettings()
        {
            if (ImGui.Combo("appliedMaterial", ref materialIndex, "meshNormal\0meshStandard\0"))
            {
                scene.Remove(appliedMesh);
                if (materialIndex == 0)
                {

                    appliedMesh = DemoUtils.AppliyMeshNormalMaterial(geometry, ref appliedMaterial);
                }
                else
                {
                    appliedMesh = DemoUtils.AppliyMeshStandardMaterial(geometry, ref appliedMaterial);

                }
                scene.Add(appliedMesh);
            }

            if (rebuildGeometry)
                RebuildGeometry();

            ImGui.Checkbox("groundPlandVisible", ref groundPlane.Visible);
            ImGui.Checkbox("castShadow", ref appliedMesh.CastShadow);
          
            AddBasicMaterialSettings(appliedMesh.Material, "THREE.Material");
            AddSpecificMaterialSettings(appliedMesh.Material, "THREE.MeshStandardMaterial");
        }
        public override void AddMetalness(Material material)
        {
            ImGui.SliderFloat("metalness", ref material.Metalness, 0.0f, 1.0f);
        }
        public override void AddRoughness(Material material)
        {
            ImGui.SliderFloat("roughness", ref material.Roughness, 0.0f, 1.0f);
        }
        public override void AddWireframeLineProperty(Material material)
        {
        }
    }
}
