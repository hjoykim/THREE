using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE;
namespace THREEExample.Three.Geometries
{
    [Example("Terrain Raycast", ExampleCategory.ThreeJs, "geometry")]
    public class GeometryTerrainRaycastExample : GeometryTerrainExample
    {
        OrbitControls orbitControl;
       

        Vector2 mouse = new Vector2();

        Raycaster raycaster = new Raycaster();

        Mesh helper;

        public GeometryTerrainRaycastExample() : base()
        {
            
        }

       
        public override void InitCameraController()
        {
            orbitControl = new OrbitControls(glControl, camera);
            
            orbitControl.target.Y = Data[worldHalfWidth + worldHalfDepth * worldWidth] + 500;
            camera.Position.Y = orbitControl.target.Y + 2000;
            camera.Position.X = 2000;
            orbitControl.Update();
        }
        public override void Init()
        {
            base.Init();

            glControl.MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            mouse.X = (1.0f * e.X / glControl.Width) * 2 - 1;
            mouse.Y = -(1.0f * e.Y / glControl.Height) * 2 + 1;
            raycaster.SetFromCamera(mouse, camera);

            // See if the ray from the camera into the world hits one of our meshes
            var intersects = raycaster.IntersectObject(mesh);

            // Toggle rotation bool for meshes that we clicked
            if (intersects.Count > 0)
            {

                helper.Position.Set(0, 0, 0);
                helper.LookAt(intersects[0].face.Normal);
                helper.Position.Copy(intersects[0].point);

            }
        }

        public override void BuildScene()
        {
            base.BuildScene();

            var coneGeometry = new ConeBufferGeometry(20, 100, 3);
            coneGeometry.Translate(0, 50, 0);
            coneGeometry.RotateX((float)Math.PI / 2);
            helper = new Mesh(coneGeometry, new MeshNormalMaterial());
            scene.Add(helper);
        }
       
        public override void Render()
        {

            renderer.Render(scene, camera);
            ShowGUIControls();
        }
        public override void Resize(ResizeEventArgs clientSize)
        {
            if (renderer != null)
                renderer.Resize(clientSize.Width, clientSize.Height);

            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();

        }
    }


}
