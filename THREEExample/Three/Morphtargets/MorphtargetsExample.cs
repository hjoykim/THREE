using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Geometries;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;

namespace THREEExample.Three.Morphtargets
{
    [Example("Morphtargets", ExampleCategory.ThreeJs, "Morphtargets")]
    public class MorphtargetsExample : THREEExampleTemplate
    {
        Mesh mesh;
        public MorphtargetsExample() : base() { }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 0, 500);
            camera.LookAt(Vector3.Zero());
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var light = new PointLight(Color.Hex(0xffffff));
            light.Position.Z = 500;
            scene.Add(light);

            var amblight = new AmbientLight(Color.Hex(0x111111));
            scene.Add(amblight);
        }
        private void CreateObject()
        {
           
            var geometry = new BoxGeometry(100, 100, 100);
            var material = new MeshLambertMaterial() { Color = Color.Hex(0xff0000), MorphTargets = true };

            // construct 8 blend shapes

            for (var i = 0; i < 8; i++)
            {

                var vertices = new List<Vector3>();

                for (var v = 0; v < geometry.Vertices.Count; v++)
                {

                    vertices.Add(geometry.Vertices[v].Clone());

                    if (v == i)
                    {

                        vertices[vertices.Count - 1].X *= 2;
                        vertices[vertices.Count - 1].Y *= 2;
                        vertices[vertices.Count - 1].Z *= 2;

                    }

                }

                geometry.MorphTargets.Add("target" + i, vertices);

            }
            var bufferGeometry = new BufferGeometry().FromGeometry(geometry);

            mesh = new Mesh(bufferGeometry, material);

            scene.Add(mesh);
        }
        public override void Init()
        {
            base.Init();
            scene.Background = Color.Hex(0x222222);
            scene.Fog = new Fog(Color.Hex(0x000000), 1, 15000);

            CreateObject();
            //mesh.MorphTargetInfluences[0] = 0.5f;
            AddGuiControlsAction = MorphTargetsControl;
        }
        public override void Render()
        {
            mesh.Rotation.Y += 0.001f;
            base.Render();
        }

        float influence1 = 0;
        float influence2 = 0;
        float influence3 = 0;
        float influence4 = 0;
        float influence5 = 0;
        float influence6 = 0;
        float influence7 = 0;
        float influence8 = 0;

        private void MorphTargetsControl()
        {
            if(ImGui.SliderFloat("influence1",ref influence1, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[0] = influence1;
            }
            if (ImGui.SliderFloat("influence2", ref influence2, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[1] = influence2;
            }
            if (ImGui.SliderFloat("influence3", ref influence3, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[2] = influence3;
            }
            if (ImGui.SliderFloat("influence4", ref influence4, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[3] = influence4;
            }
            if (ImGui.SliderFloat("influence5", ref influence5, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[4] = influence5;
            }
            if (ImGui.SliderFloat("influence6", ref influence6, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[5] = influence6;
            }
            if (ImGui.SliderFloat("influence7", ref influence7, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[6] = influence7;
            }
            if (ImGui.SliderFloat("influence8", ref influence8, 0.0f, 1.0f))
            {
                mesh.MorphTargetInfluences[7] = influence8;
            }
        }
    }
}
