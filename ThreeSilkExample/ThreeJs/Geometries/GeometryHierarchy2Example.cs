using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("Hierachy2", ExampleCategory.ThreeJs, "Geometry")]
    public class GeometryHierarchy2Example : GeometryHierarchyExample
    {
        THREE.Mesh root = null;
        public GeometryHierarchy2Example() : base()
        {

        }
        public override void BuildScene()
        {
            var geometry = new THREE.BoxBufferGeometry(100, 100, 100);
            var material = new THREE.MeshNormalMaterial();

            root = new THREE.Mesh(geometry,material);
            root.Position.X = 1000;
            scene.Add(root);
			var amount = 200;
			THREE.Mesh object3d;
			THREE.Mesh parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.X = 100;

				parent.Add(object3d);
				parent = object3d;

			}

			parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.X = -100;

				parent.Add(object3d);
				parent = object3d;

			}

			parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.Y = -100;

				parent.Add(object3d);
				parent = object3d;

			}

			parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.Y = 100;

				parent.Add(object3d);
				parent = object3d;

			}

			parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.Z = -100;

				parent.Add(object3d);
				parent = object3d;

			}

			parent = root;

			for (var i = 0; i < amount; i++)
			{

				object3d = new THREE.Mesh(geometry, material);
				object3d.Position.Z = 100;

				parent.Add(object3d);
				parent = object3d;

			}
			this.MouseMove += OnMouseMove;
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

			root.Traverse( o => {

				o.Rotation.X = rx;
				o.Rotation.Y = ry;
				o.Rotation.Z = rz;

			} );

			renderer.Render(scene, camera);
		}
    }
}
