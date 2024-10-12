using ImGuiNET;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("06.Mesh-Properties", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class MeshPropertiesExample : Example
    {
        Mesh cube;
        float translateX = 0;
        float translateY = 0;
        float translateZ = 0;
        public MeshPropertiesExample() : base() { }

        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0xEEEEEE), 1);
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var ambientLight = new AmbientLight(THREE.Color.Hex(0x0c0c0c));
            scene.Add(ambientLight);

            var spotLight = new SpotLight(THREE.Color.Hex(0xffffff));

            spotLight.Position.Set(-40, 30, 30);
            spotLight.CastShadow = true;

            scene.Add(spotLight);
        }
        public override void Init()
        {
            base.Init();

            camera.LookAt(scene.Position);

            var planeGeometry = new PlaneGeometry(60, 40, 1, 1);
            MeshLambertMaterial planeMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0xffffff) };

            var plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 0;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);

            var material = new MeshLambertMaterial() { Color = THREE.Color.Hex(0x44ff44) };
            var geom = new BoxGeometry(5, 8, 3);
            cube = new Mesh(geom, material);
            cube.Position.Y = 4;
            cube.CastShadow = true;
            scene.Add(cube);

            AddGuiControlsAction = () =>
            {
                if(ImGui.TreeNode("scale"))
                {
                    ImGui.SliderFloat("scaleX", ref cube.Scale.X, 0, 5);
                    ImGui.SliderFloat("scaleY", ref cube.Scale.Y, 0, 5);
                    ImGui.SliderFloat("scaleZ", ref cube.Scale.Z, 0, 5);
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("position"))
                {
                    ImGui.SliderFloat("positionX", ref cube.Position.X, -10, 10);
                    ImGui.SliderFloat("postionY", ref cube.Position.Y, -4, 20);
                    ImGui.SliderFloat("positionZ", ref cube.Position.Z, -10, 10);
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("rotation"))
                {
                    float rotationX = cube.Rotation.X;
                    float rotationY = cube.Rotation.Y;
                    float rotationZ = cube.Rotation.Z;
                    if (ImGui.SliderFloat("rotationX", ref rotationX, -4, 4))                    
                        cube.Rotation.X = rotationX;                    
                    if(ImGui.SliderFloat("postionY", ref rotationY, -4, 4))
                        cube.Rotation.Y = rotationY;
                    if(ImGui.SliderFloat("rotationZ", ref rotationZ, -4, 4))
                        cube.Rotation.Z = rotationZ;

                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("translate"))
                {
                    ImGui.SliderFloat("translateX", ref translateX, -10, 10);
                    ImGui.SliderFloat("translateY", ref translateY, -10, 10);
                    ImGui.SliderFloat("translateZ", ref translateZ, -10, 10);
                    if(ImGui.Button("translate"))
                    {
                        cube.TranslateX(translateX);
                        cube.TranslateY(translateY);
                        cube.TranslateZ(translateZ);
                    }
                    ImGui.TreePop();
                }

                ImGui.Checkbox("visiable", ref cube.Visible);
            };
        }
    }
}
