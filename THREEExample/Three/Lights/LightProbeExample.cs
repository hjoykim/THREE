﻿using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Textures;
using THREEExample;

namespace THREEExample.Three.Lights
{
    [Example("lightProbe",ExampleCategory.ThreeJs,"lightprobe")]
    public class LightProbeExample : ExampleTemplate
    {
        struct API
        {
            public float lightProbeIntensity;
            public float directionalLightIntensity;
            public float envMapIntensity;
        }
        API api;
        Mesh mesh;
        LightProbe lightProbe;
        DirectionalLight directionalLight;
        CubeTexture cubeTexture;

        public LightProbeExample() : base()
        {
            api.lightProbeIntensity = 1.0f;
            api.directionalLightIntensity = 0.2f;
            api.envMapIntensity = 1.0f;
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.ToneMapping = Constants.NoToneMapping;
            renderer.outputEncoding = Constants.sRGBEncoding;
        }
        public override void InitLighting()
        {
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 40;
            camera.Near = 1;
            camera.Far = 1000;
            camera.Position.Set(0, 0, 30);
        }
        public override void Init()
        {
            base.Init();
            directionalLight = new DirectionalLight(Color.Hex(0xffffff), api.directionalLightIntensity);
            directionalLight.Position.Set(10, 10, 10);
            scene.Add(directionalLight);

            var urls = GenCubeUrls("../../../assets/textures/cube/pisa/", ".png");

            cubeTexture = CubeTextureLoader.Load(urls);
            cubeTexture.Encoding = Constants.sRGBEncoding;
            scene.Background = cubeTexture;

            lightProbe = LightProbeGenerator.FromCubeTexture(cubeTexture);
            scene.Add(lightProbe);

            var geometry = new SphereBufferGeometry(5, 64, 32);
            //var geometry = new TorusKnotBufferGeometry( 4, 1.5, 256, 32, 2, 3 );

            var material = new MeshStandardMaterial()
            {
                Color = Color.Hex(0xffffff),
                Metalness = 0,
                Roughness = 0,
                EnvMap = cubeTexture,
                EnvMapIntensity = api.envMapIntensity,
            };

            // mesh
            mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            AddGuiControlsAction = ShowLightControls;
        }
      
        private List<string> GenCubeUrls(string prefix, string postfix)
        {

            return new List<string>(){
                prefix + "px" + postfix, prefix + "nx" + postfix,
                prefix + "py" + postfix, prefix + "ny" + postfix,
                prefix + "pz" + postfix, prefix + "nz" + postfix
            };

        }
        private void ShowLightControls()
        {
            if (ImGui.TreeNode("Intensity"))
            {
                ImGui.SliderFloat("lightProbe", ref lightProbe.Intensity, 0.0f, 1.0f);
                ImGui.SliderFloat("directional light", ref directionalLight.Intensity, 0.0f, 1.0f);
                ImGui.SliderFloat("envMap", ref mesh.Material.EnvMapIntensity, 0.0f, 1.0f);

                ImGui.TreePop();
            }
        }
}
}