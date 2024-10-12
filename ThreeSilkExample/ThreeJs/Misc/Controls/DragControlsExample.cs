
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example.Controls
{
    [Example("Controls-Drag",ExampleCategory.ThreeJs,"Controls")]
    public class DragControlsExample : ControlsExampleTemplate
    {
        Vector2 mouse = new Vector2();

        Raycaster raycaster = new Raycaster();

        DragControls dragControls;

        Group group;

        List<Object3D> objects = new List<Object3D>();

        bool enableSelection = false;

        public DragControlsExample() : base()
        {

        }

        public override void InitCamera()
        {
            camera = new PerspectiveCamera(70, this.AspectRatio, 1, 5000);
            camera.Position.Z = 1000;
        }

        public override void InitLighting()
        {
            var light = new AmbientLight(Color.Hex(0x505050));
            scene.Add(light);

            var spotlight = new SpotLight(Color.Hex(0xffffff), 1.5f);
            spotlight.Position.Set(0, 500, 2000);
            spotlight.Angle = (float)System.Math.PI / 9;
            spotlight.CastShadow = true;
            spotlight.Shadow.Camera.Near = 1000;
            spotlight.Shadow.Camera.Far = 4000;
            spotlight.Shadow.MapSize.Width = 1024;
            spotlight.Shadow.MapSize.Height = 1024;

            scene.Add(spotlight);

        }

        public override void Init()
        {
            base.Init();

            scene.Background = Color.Hex(0xf0f0f0);
            group = new Group();
            scene.Add(group);
            var geometry = new BoxGeometry(40, 40, 40);

            for (int i = 0; i < 200; i++)
            {
                var object3d = new Mesh(geometry, new MeshLambertMaterial() { Color = new Color().Random() });

                object3d.Position.X = (float)MathUtils.random.NextDouble() * 1000 - 500;
                object3d.Position.Y = (float)MathUtils.random.NextDouble() * 600 - 300;
                object3d.Position.Z = (float)MathUtils.random.NextDouble() * 800 - 400;

                object3d.Rotation.X = (float)MathUtils.random.NextDouble() * 2 * (float)Math.PI;
                object3d.Rotation.Y = (float)MathUtils.random.NextDouble() * 2 * (float)Math.PI;
                object3d.Rotation.Z = (float)MathUtils.random.NextDouble() * 2 * (float)Math.PI;

                object3d.Scale.X = (float)MathUtils.random.NextDouble() * 2 + 1;
                object3d.Scale.Y = (float)MathUtils.random.NextDouble() * 2 + 1;
                object3d.Scale.Z = (float)MathUtils.random.NextDouble() * 2 + 1;

                object3d.CastShadow = true;
                object3d.ReceiveShadow = true;

                scene.Add(object3d);

                objects.Add(object3d);

            }

            dragControls = new DragControls(this, objects, camera);
         
            this.MouseDown += OnMouseClick;
            this.KeyUp += OnKeyUp;
            this.KeyDown += OnKeyDown;

            dragControls.DragStart += OnDragStart;
            dragControls.DragEnd += OnDragEnd;
        }
        private void OnDragEnd(Object3D obj)
        {
            controls.Enabled = true;
        }
        private void OnDragStart(Object3D obj)
        {
            controls.state = TrackballControls.STATE.NONE;
            controls.Enabled = false;
        }
        private void OnKeyUp(object sender,KeyboardKeyEventArgs e)
        {
            enableSelection = false;
        }
        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            enableSelection = (e.Key == Key.ShiftRight) ? true : false;
        }

        private void OnMouseClick(object sender,MouseEventArgs e)
        {
            if(enableSelection==true && e.Button==MouseButton.Left)
            {
                var draggableObjects = dragControls.objects;
                mouse.X = e.X * 1.0f / ClientRectangle.Width * 2 - 1.0f;
                mouse.Y = -e.Y * 1.0f / ClientRectangle.Height * 2 + 1.0f;

                raycaster.SetFromCamera(mouse, camera);
                var intersections = raycaster.IntersectObjects(objects, true);

                if (intersections.Count > 0)
                {
                    var object3d = intersections[0].object3D;
                    if(group.Children.Contains(object3d))
                    {
                        object3d.Material.Emissive = Color.Hex(0x000000);
                        scene.Attach(object3d);
                    }
                    else
                    {
                        object3d.Material.Emissive = Color.Hex(0xaaaaaa);
                        group.Attach(object3d);
                    }
                    dragControls.TransformGroup = true;
                    draggableObjects.Add(group);
                    objects.Insert(0,group);
                }
                if(group.Children.Count==0)
                {
                    dragControls.TransformGroup = false;
                    objects.Remove(group);

                }
            }
        }
    }
}
