using ImGuiNET;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example.Learning.Chapter06
{
    [Example("08-Binary-Operation", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class BinaryOperationExample : Example
    {
        Mesh result;

        Mesh groundPlane;

        Mesh sphere1;
        Mesh sphere2;
        Mesh cube;
        int sphereAction = 0;
        int cubeAction = 0;

        bool rotateResult = false;
        bool hideWireframes = false;
        public BinaryOperationExample() : base()
        {

        }

        private Mesh CreateMesh(THREE.Geometry geometry)
        {
            var wireframeMaterial = new MeshBasicMaterial() { Transparent = true, Opacity = 0.5f, WireframeLineWidth = 0.5f, Wireframe = true };

            return new Mesh(geometry, wireframeMaterial);
        }
        private void BuildGeometry()
        {
            groundPlane = DemoUtils.AddLargeGroundPlane(this.scene);

            groundPlane.Position.Y = -30;

            DemoUtils.InitDefaultLighting(this.scene);


            sphere1 = CreateMesh(new SphereGeometry(5, 20, 30));

            sphere1.Position.X = -2;

            sphere2 = CreateMesh(new SphereGeometry(5, 20, 30));

            sphere2.Position.Set(3, 0, 0);

            cube = CreateMesh(new BoxGeometry(5, 5, 5));

            cube.Position.X = -7;

            scene.Add(sphere1);
            scene.Add(sphere2);
            scene.Add(cube);

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 20, 20);
        }

        public override void Init()
        {
            base.Init();
            BuildGeometry();

            AddGuiControlsAction = ShowControls;
        }

        public override void Render()
        {
            if (rotateResult && result != null)
            {
                result.Rotation.Y += 0.4f;
                result.Rotation.Z -= 0.005f;
            }

            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            renderer.Render(scene, camera);
        }
        private void ShowControls()
        {
            if (ImGui.TreeNode("Sphere1"))
            {
                ImGui.SliderFloat("sphere1PosX", ref sphere1.Position.X, -15, 15);
                ImGui.SliderFloat("sphere1PosY", ref sphere1.Position.Y, -15, 15);
                ImGui.SliderFloat("sphere1PosZ", ref sphere1.Position.Z, -15, 15);
                if (ImGui.SliderFloat("sphere1Scale", ref sphere1.Scale.X, 0, 10))
                {
                    sphere1.Scale.Y = sphere1.Scale.Z = sphere1.Scale.X;
                }

                ImGui.TreePop();
            }
            if (ImGui.TreeNode("Sphere2"))
            {
                ImGui.SliderFloat("sphere2PosX", ref sphere2.Position.X, -15, 15);
                ImGui.SliderFloat("sphere2PosY", ref sphere2.Position.Y, -15, 15);
                ImGui.SliderFloat("sphere2PosZ", ref sphere2.Position.Z, -15, 15);
                if (ImGui.SliderFloat("sphere2Scale", ref sphere2.Scale.X, 0, 10))
                {
                    sphere2.Scale.Y = sphere2.Scale.Z = sphere2.Scale.X;
                }

                ImGui.Combo("actionSphere", ref sphereAction, "subtract\0intersect\0union\0none\0");
                ImGui.TreePop();
            }
            if (ImGui.TreeNode("cube"))
            {
                ImGui.SliderFloat("cubePosX", ref cube.Position.X, -15, 15);
                ImGui.SliderFloat("cubePosY", ref cube.Position.Y, -15, 15);
                ImGui.SliderFloat("cubePosZ", ref cube.Position.Z, -15, 15);
                ImGui.SliderFloat("ScaleX", ref cube.Scale.X, 0, 10);
                ImGui.SliderFloat("ScaleY", ref cube.Scale.Y, 0, 10);
                ImGui.SliderFloat("ScaleZ", ref cube.Scale.Z, 0, 10);
                ImGui.Combo("actionCube", ref sphereAction, "subtract\0intersect\0union\0none\0");
                ImGui.TreePop();
            }
            if (ImGui.Button("showResult"))
            {
                showResult();
            }
            ImGui.Checkbox("rotateResult", ref rotateResult);
            if (ImGui.Checkbox("hideWireframes", ref hideWireframes))
            {
                if (hideWireframes)
                {
                    sphere1.Material.Visible = false;
                    sphere2.Material.Visible = false;
                    cube.Material.Visible = false;
                }
                else
                {
                    sphere1.Material.Visible = true;
                    sphere2.Material.Visible = true;
                    cube.Material.Visible = true;
                }
            }
        }

        private void showResult()
        {
            if (result != null)
            {
                scene.Remove(result);
            }
            var sphere1BSP = new ThreeBSP(sphere1);
            var sphere2BSP = new ThreeBSP(sphere2);
            var cube2BSP = new ThreeBSP(cube);

            ThreeBSP resultBSP = null;
            switch (sphereAction)
            {
                case 0:
                    resultBSP = sphere1BSP.Subtract(sphere2BSP);
                    break;
                case 1:
                    resultBSP = sphere1BSP.Intersect(sphere2BSP);
                    break;
                case 2:
                    resultBSP = sphere1BSP.Union(sphere2BSP);
                    break;
                case 3:
                    break;
            }
            if (resultBSP == null) resultBSP = sphere1BSP;

            switch (cubeAction)
            {
                case 0:
                    resultBSP = resultBSP.Subtract(cube2BSP);
                    break;
                case 1:
                    resultBSP = resultBSP.Intersect(cube2BSP);
                    break;
                case 2:
                    resultBSP = resultBSP.Union(cube2BSP);
                    break;
                case 3:
                    break;
            }

            if (sphereAction != 3 && cubeAction != 3)
            {
                result = resultBSP.ToMesh();
                result.Geometry.ComputeFaceNormals();
                result.Geometry.ComputeVertexNormals();
                scene.Add(result);
            }
        }
    }
}
