using ImGuiNET;
using OpenTK;
using THREE;
using THREEExample.ThreeImGui;
using Vector2 = THREE.Vector2;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter11
{
    [Example("10-shader-pass-advanced", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class ShaderPassAdvancedExample : EffectComposerTemplate
    {
        ShaderPass bleachFilter;
        ShaderPass FXAAShader;
        ShaderPass focusShader;
        public ShaderPassAdvancedExample() : base()
        {

        }
        public override void InitLighting()
        {
            base.InitLighting();
            var dirLight = new DirectionalLight(Color.Hex(0xffffff));
            dirLight.Position.Set(30, 30, 30);
            dirLight.Intensity = 0.8f;
            scene.Add(dirLight);

            // add spotlight for the shadows
            var spotLight = new SpotLight(Color.Hex(0xffffff));
            spotLight.CastShadow = true;
            spotLight.Position.Set(-30, 30, -100);
            spotLight.Target.Position.X = -10;
            spotLight.Target.Position.Z = -10;
            spotLight.Intensity = 0.6f;
            spotLight.Shadow.MapSize.Width = 4096;
            spotLight.Shadow.MapSize.Height = 4096;
            spotLight.Shadow.Camera.Fov = 120;
            spotLight.Shadow.Camera.Near = 1;
            spotLight.Shadow.Camera.Far = 200;

            scene.Add(spotLight);
        }
        public override void Init()
        {
            base.Init();
            var plane = new BoxBufferGeometry(1600, 1600, 0.1f, 40, 40);

            var cube = new Mesh(plane, new MeshPhongMaterial
            {
                Color = Color.Hex(0xffffff),
                Map = TextureLoader.Load("../../../../assets/textures/general/floor-wood.jpg"),
                NormalScale = new Vector2(0.6f, 0.6f)
            });
            cube.Material.Map.WrapS = Constants.RepeatWrapping;
            cube.Material.Map.WrapT = Constants.RepeatWrapping;

            cube.Rotation.X = (float)System.Math.PI / 2;
            cube.Material.Map.Repeat.Set(80, 80);

            cube.ReceiveShadow = true;
            cube.Position.Z = -150;
            cube.Position.X = -150;
            scene.Add(cube);

            var range = 3;
            var stepX = 8;
            var stepZ = 8;
            for (var i = -25; i < 5; i++)
            {
                for (var j = -15; j < 15; j++)
                {
                    cube = new Mesh(new BoxBufferGeometry(3, 4, 3),
                            new MeshPhongMaterial
                            {
                                Color = MathUtils.NextColor(),
                                Opacity = 0.8f,
                                Transparent = true
                            });
                    cube.Position.X = i * stepX + (float)(MathUtils.random.NextDouble() - 0.5f) * range;
                    cube.Position.Z = j * stepZ + (float)(MathUtils.random.NextDouble() - 0.5f) * range;
                    cube.Position.Y = (float)(MathUtils.random.NextDouble() - 0.5f) * 2;
                    cube.CastShadow = true;
                    scene.Add(cube);
                }
            }

            bleachFilter = new ShaderPass(new BleachBypassShader());
            bleachFilter.Enabled = false;

            //var edgeShader = new THREE.ShaderPass(THREE.EdgeShader);
            //edgeShader.enabled = false;

            FXAAShader = new ShaderPass(new FXAAShader());
            FXAAShader.Enabled = false;

            focusShader = new ShaderPass(new FocusShader());
            focusShader.Enabled = false;

            renderPass = new RenderPass(scene, camera);
            var effectCopy = new ShaderPass(new CopyShader());
            effectCopy.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(bleachFilter);
            //composer.addPass(edgeShader);
            composer.AddPass(FXAAShader);
            composer.AddPass(focusShader);
            composer.AddPass(effectCopy);

            AddGuiControlsAction = () =>
            {
                AddBleachControl(bleachFilter);
                AddFXAAShaderControl(FXAAShader);
                AddFocusShaderControl(focusShader);
            };
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls.Update();

            composer.Render();

        }
       
        float bleachOpacity = 1.0f;
        private void AddBleachControl(ShaderPass pass)
        {
            if (ImGui.TreeNode("bleachFilter"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("opacity", ref bleachOpacity, 0.0f, 1.0f))
                {
                    (pass.uniforms["opacity"] as GLUniform)["value"] = bleachOpacity;
                }

                ImGui.TreePop();
            }
        }
       
        private void AddFXAAShaderControl(ShaderPass pass)
        {
            if (ImGui.TreeNode("FXAAShader"))
            {
                if (ImGui.Checkbox("enabled", ref pass.Enabled))
                {
                    (pass.uniforms["resolution"] as GLUniform)["value"] = new Vector2(1.0f/glControl.Width,1.0f/glControl.Height);
                }
                ImGui.TreePop();
            }
        }
        float sampleDistance = 0.94f;
        float waveFactor = 0.00125f;
        private void AddFocusShaderControl(ShaderPass pass)
        {
            if (ImGui.TreeNode("focusShader"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("sampleDistance", ref sampleDistance, 0.0f, 2.0f))
                {
                    (pass.uniforms["sampleDistance"] as GLUniform)["value"] = sampleDistance;
                }
                if (ImGui.SliderFloat("waveFactor", ref waveFactor, 0.0f, 0.005f))
                {
                    (pass.uniforms["waveFactor"] as GLUniform)["value"] = waveFactor;
                }
                ImGui.TreePop();
            }
        }
    }
}
