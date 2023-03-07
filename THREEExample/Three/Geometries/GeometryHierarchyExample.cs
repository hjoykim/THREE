using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE;
namespace THREEExample.Three.Geometries
{
    [Example("Hierachy", ExampleCategory.ThreeJs, "geometry")]
    public class GeometryHierarchyExample : ExampleTemplate
    {
        Group group;
        public int windowHalfX, windowHalfY;
        public int mouseX = 0;
        public int mouseY = 0;
        public GeometryHierarchyExample() : base()
        {
            scene.Background = Color.Hex(0xffffff);
            scene.Fog = new Fog(0xffffff, 1, 10000);
            stopWatch.Start();
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(60, glControl.AspectRatio, 1, 10000);
            camera.Position.Z = 500;
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            windowHalfX = clientSize.Width / 2;
            windowHalfY = clientSize.Height / 2;

        }
        public override void Init()
        {
            base.Init();

            BuildScene();
        }
        public virtual void BuildScene()
        {
            var geometry = new THREE.BoxBufferGeometry(100, 100, 100);
            var material = new THREE.MeshNormalMaterial();

            group = new Group();

            for (var i = 0; i < 1000; i++)
            {

                var mesh = new Mesh(geometry, material);
                mesh.Position.X = MathUtils.NextFloat() * 2000 - 1000;
                mesh.Position.Y = MathUtils.NextFloat() * 2000 - 1000;
                mesh.Position.Z = MathUtils.NextFloat() * 2000 - 1000;

                mesh.Rotation.X = MathUtils.NextFloat() * 2 * (float)Math.PI;
                mesh.Rotation.Y = MathUtils.NextFloat() * 2 * (float)Math.PI;

                mesh.MatrixAutoUpdate = false;
                mesh.UpdateMatrix();

                group.Add(mesh);

            }

            scene.Add(group);

            glControl.MouseMove += OnMouseMove;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            mouseX = ( e.X - windowHalfX ) * 10;
            mouseY = ( e.Y - windowHalfY ) * 10;
        }

        public override void Render()
        {
            var time = stopWatch.ElapsedMilliseconds * 0.001f;

            var rx = (float)Math.Sin(time * 0.7) * 0.5f;
            var ry = (float)Math.Sin(time * 0.3) * 0.5f;
            var rz = (float)Math.Sin(time * 0.2) * 0.5f;

            camera.Position.X += (mouseX - camera.Position.X) * 0.05f;
            camera.Position.Y += (-mouseY - camera.Position.Y) * 0.05f;

            camera.LookAt(scene.Position);

            group.Rotation.X = rx;
            group.Rotation.Y = ry;
            group.Rotation.Z = rz;
            base.Render();
        }
    }
}
