using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;

namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing Dynamic", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancingDynamic : Example
    {
        int amount = 10;
        int count;
        Object3D dummy = new Object3D();
        InstancedMesh mesh;
        public InstancingDynamic() :base() 
        {
            count = (int)Math.Pow(amount, 3);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0x000000);
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(60, ClientRectangle.Width / ClientRectangle.Height, 0.1f, 100);
            camera.Position.Set(amount * 0.9f, amount * 0.9f, amount * 0.9f);
            camera.LookAt(0, 0, 0);
        }
        public override void Init()
        {
            base.Init();

            var loader = new THREE.BufferGeometryLoader();
            BufferGeometry geometry = loader.Load("../../../../assets/models/json/suzanne_buffergeometry.json");

            geometry.ComputeVertexNormals();
            geometry.Scale(0.5f, 0.5f, 0.5f);

            Geometry geometry1 = new Geometry();
            // transfrom buffer geometry to geometry
            // I can't find yet why buffergeometry with index is not rendered on this THREE library
            // Now continue debugging.
            // Anybody who can find out this solution of this bug. please let me know
            geometry1.FromBufferGeometry(geometry); 

            var material = new THREE.MeshNormalMaterial();
            mesh = new THREE.InstancedMesh(geometry1, material, count);
            mesh.InstanceMatrix.SetUsage(Constants.DynamicDrawUsage); // will be updated every frame
            scene.Add(mesh);

            AddGuiControlsAction = () =>
            {
                ImGui.SliderInt("count", ref mesh.InstanceCount, 0, count);
            };
        }
        public override void Render()
        {
            base.Render();
            SetInstanceMatrix(GetDelta());
        }
        private void SetInstanceMatrix(float delta)
        {
            var time = delta ;

            mesh.Rotation.X = (float)Math.Sin(time / 4);
            mesh.Rotation.Y = (float)Math.Sin(time / 2);

            int i = 0;
            int offset = (amount - 1) / 2;

            for (int x = 0; x < amount; x++)
            {

                for (int y = 0; y < amount; y++)
                {
                    for (int z = 0; z < amount; z++)
                    {
                        dummy.Position.Set(offset - x, offset - y, offset - z);
                        dummy.Rotation.Y = (float)(Math.Sin(x / 4 + time) + Math.Sin(y / 4 + time) + Math.Sin(z / 4 + time));
                        dummy.Rotation.Z = dummy.Rotation.Y * 2;

                        dummy.UpdateMatrix();

                        mesh.SetMatrixAt(i++, dummy.Matrix);

                    }

                }

            }

            mesh.InstanceMatrix.NeedsUpdate = true;
           
        }
    }
}
