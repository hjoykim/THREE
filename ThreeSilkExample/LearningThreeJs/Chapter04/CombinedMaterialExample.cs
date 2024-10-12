using ImGuiNET;
using System;
using System.Collections.Generic;
using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("03.Combined-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class CombinedMaterialExample : DepthMaterialExample
    {

        Color ControlColor = Color.Hex(0x00ff00);

        public CombinedMaterialExample() : base()
        {
            scene.OverrideMaterial = null;
        }

        public override void Init()
        {
            base.Init();

            AddGuiControlsAction = () =>
            {
                AddBasicMaterialSettings(null, "");
            };

        }

        public override void AddCube()
        {
            var cubeSize = (int)Math.Ceiling((Decimal)(3 + MathUtils.random.Next(5)));

            var cubeGeometry = new BoxGeometry(cubeSize, cubeSize, cubeSize);
            var cubeMaterial = new MeshDepthMaterial();
            var colorMaterial = new MeshBasicMaterial()
            {
                Color = ControlColor,
                Transparent = true,
                Blending = Constants.MultiplyBlending
            };
            var cube = SceneUtils.CreateMultiMaterialObject(cubeGeometry,new List<Material>() {colorMaterial,cubeMaterial });
            cube.Children[1].Scale.Set(0.99f, 0.99f, 0.99f);
            cube.CastShadow = true;

            cube.Position.X = -60 + (float)Math.Round((float)MathUtils.random.Next(120));
            cube.Position.Y = (float)Math.Round((float)MathUtils.random.Next(20));
            cube.Position.Z = -100 + (float)Math.Round((float)MathUtils.random.Next(200));

            scene.Add(cube);

            numberOfObjects = scene.Children.Count;
        }
 
        public override void AddBasicMaterialSettings(Material material, string name)
        {
            //base.AddBasicMaterialSettings(material, name);
            System.Numerics.Vector3 color = new System.Numerics.Vector3(ControlColor.R, ControlColor.G, ControlColor.B);
            if(ImGui.ColorPicker3("color",ref color))
            {
                ControlColor = new Color(color.X, color.Y, color.Z);
            }

           
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
    }
}
