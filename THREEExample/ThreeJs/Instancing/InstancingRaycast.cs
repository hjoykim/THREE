using Microsoft.VisualBasic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing Raycast", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancingRaycast : Example
    {
        int amount = 10;
        int count;
        Raycaster raycaster = new Raycaster();
        Vector2 mouse = new Vector2(1,1);
        InstancedMesh mesh;
        THREE.Color color = new THREE.Color(1,1,1);
        public InstancingRaycast() 
        {
            count = (int)Math.Pow(amount, 3);
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(60, glControl.AspectRatio, 0.1f, 100);
            camera.Position.Set(amount, amount, amount);
            camera.LookAt(0, 0, 0);
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var light1 = new THREE.HemisphereLight(Color.Hex(0xffffff), Color.Hex(0x000088));
            light1.Position.Set(-1, 1.5f, 1);
            scene.Add(light1);

            var light2 = new THREE.HemisphereLight(Color.Hex(0xffffff), Color.Hex(0x880000), 0.5f);
            light2.Position.Set(-1, -1.5f, -1);
            scene.Add(light2);

        }
        public override void InitRenderer()
        {
            //base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }
        public override void Init()
        {
            base.Init();
            
            var geometry = new THREE.IcosahedronBufferGeometry(0.5f, 3);
            var material = new THREE.MeshPhongMaterial();
            mesh = new InstancedMesh(geometry, material,count);

            int i = 0;
            int offset = (amount - 1) / 2;

            Matrix4 matrix = new THREE.Matrix4();

            for (int x = 0; x < amount; x++)
            {

                for (int y = 0; y < amount; y++)
                {

                    for (int z = 0; z < amount; z++)
                    {
                        matrix.SetPosition(offset - x, offset - y, offset - z);

                        mesh.SetMatrixAt(i, matrix);
                        mesh.SetColorAt(i, color);
                        i++;
                    }

                }

            }

            scene.Add(mesh);
            this.MouseMove += OnMouseMove;
        }
        private void OnMouseMove(object sender,THREE.MouseEventArgs e)
        {
            mouse.X =  e.X*1.0f / ClientRectangle.Width  * 2 - 1.0f;
            mouse.Y = - e.Y*1.0f / ClientRectangle.Height  * 2 + 1.0f;
        }
        public override void Render()
        {
            raycaster.SetFromCamera(mouse, camera);

            var intersection = raycaster.IntersectObject(mesh);

            if (intersection.Count > 0)
            {

                var instanceId = intersection[0].instanceId;

                mesh.SetColorAt(instanceId, new Color().Random());
                mesh.InstanceColor.NeedsUpdate = true;

            }
            base.Render();
        }
    }
}
