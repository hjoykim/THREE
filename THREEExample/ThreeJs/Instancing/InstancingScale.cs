using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;
using ImGuiNET;
namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing Scale", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancingScale : Example
    {
        public OrbitControls orbitControl;
        public InstancingScale() { }
        public override void InitLighting()
        {
            var light = new DirectionalLight(0xffffff, 1);
            light.Position.Set(10, 10, 10);
            scene.Add(light);
        }
        public override void InitCamera()
        {
            camera.Fov = 75.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 100.0f;
            camera.Position.X = 10;
            camera.Position.Y = 10;
            camera.Position.Z = 20;
        }
        public override void InitCameraController()
        {
            orbitControl = new OrbitControls(this,camera);
            orbitControl.EnableDamping = true;
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }
        public override void Init()
        {
            base.Init();
            // Geometry & Material
            var geometry = new THREE.SphereGeometry(0.5f, 16, 16);
            var material = new THREE.MeshStandardMaterial() { Color = Color.Hex(0x0077ff) };

            // Instanced Mesh
            float size = 10;
            float count = size * size * size;
            var instancedMesh = new THREE.InstancedMesh(geometry, material,(int)count);
            scene.Add(instancedMesh);

            Matrix4 matrix = new THREE.Matrix4();
            int index = 0;
            for (int x = 0; x<size; x++) {
                  for (int y = 0; y<size; y++) {
                        for (int z = 0; z<size; z++) {
                          matrix.SetPosition(x - size / 2, y - size / 2, z - size / 2);
                          instancedMesh.SetMatrixAt(index++, matrix);
                        }
                    }
            }
            instancedMesh.InstanceMatrix.NeedsUpdate = true;

            AddGuiControlsAction = () =>
            {
                if (ImGui.Button("scale"))
                {
                    for (var i = 0; i < count; i++)
                    {
                        instancedMesh.GetMatrixAt(i, matrix);
                        matrix.Scale(new THREE.Vector3(2, 2, 2));
                        instancedMesh.SetMatrixAt(i, matrix);
                    }
                    instancedMesh.InstanceMatrix.NeedsUpdate = true;
                }
            };
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                orbitControl.Enabled = true;
            else
                orbitControl.Enabled = false;

            orbitControl.Update();
            renderer.Render(scene, camera);
        }
    }
}
