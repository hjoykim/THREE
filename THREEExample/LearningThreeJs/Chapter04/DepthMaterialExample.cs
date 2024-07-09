using ImGuiNET;
using OpenTK;
using System;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter04
{
    [Example("02.Depth-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class DepthMaterialExample : MaterialExampleTemplate
    {
        public float rotationSpeed = 0.001f;
        public int numberOfObjects = 0;

        public DepthMaterialExample() : base()
        {
            scene.OverrideMaterial = new MeshDepthMaterial();
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Near = 50;
            camera.Far = 110;
            camera.Position.Set(-50, 40, 50);
            camera.LookAt(scene.Position);
        }
        public override void Init()
        {
            base.Init();
            materialsLib.Add("DepthMaterial", scene.OverrideMaterial);

            for (int i = 0; i < 10; i++)
                AddCube();

            AddGuiControlsAction = MaterialsGUIControls;
        }

        public override void Render()
        {
            scene.Traverse(o =>
            {
                if(o is Mesh)
                {
                    o.Rotation.X += rotationSpeed;
                    o.Rotation.Y += rotationSpeed;
                    o.Rotation.Z += rotationSpeed;
                }
            });
            base.Render();
        }
        public virtual void AddCube()
        {
            var cubeSize = (int)Math.Ceiling((Decimal)(3 + MathUtils.random.Next(5)));

            var cubeGeometry = new BoxGeometry(cubeSize, cubeSize, cubeSize);
            var cubeMaterial = new MeshLambertMaterial() { Color = Color.Hex(MathUtils.random.Next(10) * 0xffffff) };
            var cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;

            cube.Position.X = -60 + (float)Math.Round((float)MathUtils.random.Next(120));
            cube.Position.Y = (float)Math.Round((float)MathUtils.random.Next(20));
            cube.Position.Z = -100 + (float)Math.Round((float)MathUtils.random.Next(200));

            scene.Add(cube);

            numberOfObjects = scene.Children.Count;
        }
        public virtual void RemoveCube()
        {
            var allChildren = scene.Children;

            var lastObject = allChildren[allChildren.Count - 1];
            if(lastObject is Mesh)
            {
                scene.Remove(lastObject);
                numberOfObjects = scene.Children.Count;
            }
        }
        public override void AddBasicMaterialSettings(Material material, string name)
        {
            //base.AddBasicMaterialSettings(material, name);
            ImGui.Checkbox("wireframe", ref material.Wireframe);
            ImGui.SliderFloat("wireframeLinewidth", ref material.WireframeLineWidth, 0, 20);
            ImGui.SliderFloat("rotationSpeed", ref rotationSpeed, 0, 20);
            if(ImGui.Button("addCube"))
            {
                AddCube();
            }
            if(ImGui.Button("removeCube"))
            {
                RemoveCube();
            }
            if (ImGui.SliderFloat("cameraNear", ref camera.Near, 0, 100))
                camera.UpdateProjectionMatrix();

            if(ImGui.SliderFloat("cameraFar",ref camera.Far,50,200))
            {
                camera.UpdateProjectionMatrix();
            }
        }

        public override void AddSpecificMaterialSettings(Material material, string name)
        {
            //base.AddSpecificMaterialSettings(material, name);
        }
    }
}
