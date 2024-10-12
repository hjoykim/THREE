using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example.Chapter03
{
    [Example("03.Point-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class PointLightExample : Example
    {
        AmbientLight ambientLight;
        PointLight pointLight;
        PointLightHelper helper;
        CameraHelper shadowHelper;
        Mesh sphereLightMesh;

        float step = 0;
        float invert = 1;
        float phase = 0;
        float rotationSpeed = 0.01f;
        float bouncingSpeed = 0.03f;
        float intensity = 1;
        float distance;

        public PointLightExample() : base() { }

        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }
        public override void InitLighting()
        {
            base.InitLighting();
            ambientLight = new AmbientLight(THREE.Color.Hex(0x0c0c0c));
            scene.Add(ambientLight);

            pointLight = new PointLight(THREE.Color.Hex(0xccffcc));
            pointLight.Decay = 0.1f;
            pointLight.CastShadow = true;
            scene.Add(pointLight);
            this.distance = pointLight.Distance;
        }
        public override void Init()
        {
            base.Init();

            DemoUtils.AddHouseAndTree(scene);            

            helper = new PointLightHelper(pointLight);
            //scene.Add(helper);
            shadowHelper = new CameraHelper(pointLight.Shadow.Camera);
            //scene.Add(shadowHelper);
            var sphereLight = new SphereGeometry(0.2f);
            var sphereLightMaterial = new MeshBasicMaterial() { Color = THREE.Color.Hex(0xac6c25) };

            sphereLightMesh = new Mesh(sphereLight, sphereLightMaterial);
            sphereLightMesh.Position.Set(3, 0, 5);
            scene.Add(sphereLightMesh);

            AddGuiControlsAction = () =>
            {
                if (ImGui.TreeNode("Light Colors"))
                {
                    System.Numerics.Vector3 acolor = new System.Numerics.Vector3(ambientLight.Color.R, ambientLight.Color.G, ambientLight.Color.B);
                    if (ImGui.ColorPicker3("ambientColor", ref acolor))
                    {
                        ambientLight.Color = new THREE.Color(acolor.X, acolor.Y, acolor.Z);
                    }
                    System.Numerics.Vector3 pcolor = new System.Numerics.Vector3(pointLight.Color.R, pointLight.Color.G, pointLight.Color.B);
                    if (ImGui.ColorPicker3("pointLightColor", ref pcolor))
                    {
                        pointLight.Color = new THREE.Color(pcolor.X, pcolor.Y, pcolor.Z);
                    }
                    ImGui.TreePop();
                }
                ImGui.SliderFloat("intensity", ref pointLight.Intensity, 0, 3);
                ImGui.SliderFloat("distance", ref pointLight.Distance, 0, 100);
            };
        }
        public override void Render()
        {
            helper.Update();
            shadowHelper.Update();
            pointLight.Position.Copy(sphereLightMesh.Position);
            controls.Update();

            if (phase > 2 * System.Math.PI)
            {
                invert = invert * -1;
                phase -= (float)(2 * System.Math.PI);
            }
            else
            {
                phase += rotationSpeed;
            }
            sphereLightMesh.Position.Z = (float)(25 * System.Math.Sin(phase));
            sphereLightMesh.Position.X = (float)(14 * System.Math.Cos(phase));
            sphereLightMesh.Position.Y = 15;

            if (invert < 0)
            {
                var pivot = 14;

                sphereLightMesh.Position.X = (invert * (sphereLightMesh.Position.X - pivot)) + pivot;
            }

            base.Render();
        }
    }
}
