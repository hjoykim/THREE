using System;
using System.Linq;
using System.Diagnostics;
using THREE;
using ImGuiNET;
using Color = THREE.Color;
namespace THREEExample.Three.camera
{
    [Example("cinematic", ExampleCategory.ThreeJs, "camera")]
    public class CinematicCameraExample : Example
    {
        Vector2 mouse = new Vector2();
        float radius = 100, theta = 0;
        Raycaster raycaster;
        Object3D INTERSECTED;
        public CinematicCameraExample() : base() 
        {
            scene.Background = Color.Hex(0xf0f0f0);
        }
        public override void InitCamera()
        {
            camera = new CinematicCamera(this,60, glControl.AspectRatio, 1, 1000);
            (camera as CinematicCamera).SetLens(5);
            camera.Position.Set(2, 1, 500);
            (camera as CinematicCamera).postprocessing["enabled"] = true;
        }

        public override void InitLighting()
        {
            var light = new DirectionalLight(0xffffff, 0.35f);
            light.Position.Set(1, 1, 1).Normalize();
            scene.Add(light);
        }
        public override void InitCameraController()
        {
        }
        public override void Init()
        {
            base.Init();

            var geometry = new BoxBufferGeometry(30, 30, 30);

            for (var i = 0; i < 1500; i++)
            {

                var object3d = new Mesh(geometry, new MeshLambertMaterial() { Color = MathUtils.NextColor() });

                object3d.Position.X = MathUtils.NextFloat() * 800 - 400;
                object3d.Position.Y = MathUtils.NextFloat() * 800 - 400;
                object3d.Position.Z = MathUtils.NextFloat() * 800 - 400;

                scene.Add(object3d);

            }
            foreach(var object3d in scene.Children)
            {
                if (object3d.Material is ShaderMaterial)
                    Debug.WriteLine("Bingo");
            }

            (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalLength"] as GLUniform)["value"] = 15.0f;
            (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalDepth"] as GLUniform)["value"] = 3.0f;

            raycaster = new Raycaster();
            this.MouseMove += OnMouseMove;

            AddGuiControlsAction = AddCinematicCameraControls;
        }

        public void OnMouseMove(object sender,THREE.MouseEventArgs e)
        {
            mouse.X = e.X / (glControl.Width * 1.0f) * 2 - 1;
            mouse.Y = -e.Y / (glControl.Height * 1.0f) * 2 + 1;

        }
        public override void Render()
        {
            theta += 0.1f;

            camera.Position.X = radius * (float)Math.Sin(MathUtils.DegToRad(theta));
            camera.Position.Y = radius * (float)Math.Sin(MathUtils.DegToRad(theta));
            camera.Position.Z = radius * (float)Math.Cos(MathUtils.DegToRad(theta));
            camera.LookAt(scene.Position);

            camera.UpdateMatrixWorld();

            // find intersections

            raycaster.SetFromCamera(mouse, camera);

            var intersects = raycaster.IntersectObjects(scene.Children);

            if (intersects.Count > 0)
            {

                var targetDistance = intersects[0].distance;

                (camera as CinematicCamera).FocusAt(targetDistance); // using Cinematic camera focusAt method

                if (INTERSECTED != intersects[0].object3D ) {

                    if (INTERSECTED!=null) INTERSECTED.Material.Emissive = Color.Hex((int)INTERSECTED["currentHex"]);

                    INTERSECTED = intersects[0].object3D;
                    INTERSECTED["currentHex"] = INTERSECTED.Material.Emissive.Value.GetHex();
                    INTERSECTED.Material.Emissive = Color.Hex(0xff0000);

                }

            }
            else
            {

                if (INTERSECTED!=null) INTERSECTED.Material.Emissive = Color.Hex((int)INTERSECTED["currentHex"]);

                INTERSECTED = null;

            }

            //

            if ((bool)(camera as CinematicCamera).postprocessing["enabled"])
            {                

                (camera as CinematicCamera).RenderCinematic(scene, renderer);
                      
            }
            else
            {

                scene.OverrideMaterial = null;

                renderer.Clear();
                renderer.Render(scene, camera);

            }
            ShowGUIControls();

        }
        private void AddCinematicCameraControls()
        {
            float focalLength = (float)(((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalLength"] as GLUniform)["value"];
            if(ImGui.SliderFloat("focalLength", ref focalLength, 1.0f, 135f))
            {
                (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalLength"] as GLUniform)["value"] = focalLength;               

            }
            float fstop = (float)(((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["fstop"] as GLUniform)["value"];
            if (ImGui.SliderFloat("focalstop", ref fstop, 1.8f, 22f))
            {
                (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["fstop"] as GLUniform)["value"] = fstop;

            }
            float focalDepth = (float)(((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalDepth"] as GLUniform)["value"];
            if (ImGui.SliderFloat("focalDepth", ref focalDepth, 1.8f, 22f))
            {
                (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["focalDepth"] as GLUniform)["value"] = focalDepth;

            }
            int showFocus = (int)(((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["showFocus"] as GLUniform)["value"];
            bool showFocusFlag = showFocus >= 1 ? true : false;
            if (ImGui.Checkbox("showFocus", ref showFocusFlag))
            {
                (((camera as CinematicCamera).postprocessing["bokeh_uniforms"] as GLUniforms)["showFocus"] as GLUniform)["value"] = showFocusFlag?1:0;

            }
        }
    }
}
