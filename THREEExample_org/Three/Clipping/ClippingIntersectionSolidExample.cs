using ImGuiNET;
using System.Collections.Generic;
using THREE;

namespace THREEExample.Three.Clipping
{
    [Example("clipping intersection solid", ExampleCategory.ThreeJs, "clipping")]
    public class ClippingIntersectionSolidExample : ExampleTemplate
    {
        bool clipIntersection = true;
        List<Plane> clipPlanes;
        Group group, helpers;
        float planeConstant = 0;
        public ClippingIntersectionSolidExample() : base()
        {
            clipPlanes = new List<Plane>
            {
                new Plane(new Vector3(0, 1, 0), 0),
                new Plane(new Vector3(0, -1, 0), 0),
                new Plane(new Vector3(1, 0, 0), 0),
                new Plane(new Vector3(-1, 0, 0), 0),
                new Plane(new Vector3(0, 0, 1), 0),
            };
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(0, 1);
            renderer.LocalClippingEnabled = true;
        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(40, glControl.AspectRatio, 1, 200);
            camera.Position.Set(-1.5f, 2.5f, 3.0f);

            this.scene.Add(camera);
        }
        public override void InitLighting()
        {
            var light = new HemisphereLight(0xffffff, 0x080808, 1.5f);
            light.Position.Set(-1.25f, 1, 1.25f);
            scene.Add(light);
        }
        private void SetupModel()
        {
            group = new Group();

            for (var i = 1; i <= 30; i += 2)
            {

                var geometry = new SphereBufferGeometry(1.0f * i / 30, 48, 24);

                var material = new MeshLambertMaterial()
                {

                    Color = new Color().SetHSL(MathUtils.NextFloat(), 0.5f, 0.5f),
                    Side = Constants.DoubleSide,
                    FlatShading=true,
                    ClippingPlanes = clipPlanes,
                    ClipIntersection = clipIntersection


                };

                group.Add(new Mesh(geometry, material));

            }

            scene.Add(group);

            // helpers

            helpers = new Group();
            for(int i = 0; i < clipPlanes.Count; i++)
            {
                helpers.Add(new PlaneHelper(clipPlanes[i], 2, 0xffffff));
            }
           
            helpers.Visible = false;
            scene.Add(helpers);
        }
        public override void Init()
        {
            base.Init();

            SetupModel();

            AddGuiControlsAction = ShowControls;
        }
        private void ShowControls()
        {
            if (ImGui.Checkbox("clipIntersection", ref clipIntersection))
            {
                var children = group.Children;

                for (var i = 0; i < children.Count; i++)
                {

                    children[i].Material.ClipIntersection = clipIntersection;
                    children[i].Material.NeedsUpdate = true;

                }
            }
            if (ImGui.SliderFloat("planeConstant", ref planeConstant, -1.0f, 1.0f))
            {
                for (var j = 0; j < clipPlanes.Count; j++)
                {

                    clipPlanes[j].Constant = planeConstant;

                }

            }
            ImGui.Checkbox("showHelpers", ref helpers.Visible);
        }
    }
}

