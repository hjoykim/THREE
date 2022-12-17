using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Geometries;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREEExample.Three.Clipping
{
    [Example("clipping",ExampleCategory.ThreeJs,"clipping")]
    public class ClippingExample:ExampleTemplate
    {

		long startTime;
		Mesh object3d;
		MeshPhongMaterial material;
		OrbitControls ocontrol;
		Plane localPlane, globalPlane;
		List<Plane> globalPlanes, Empty;
		bool globalPlaneEnabled = false;

        public ClippingExample() : base()
        {
			stopWatch.Start();
			startTime = stopWatch.ElapsedMilliseconds;
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
			renderer.SetClearColor(0x000000, 1);
			
		}
        public override void InitCameraController()
        {
			ocontrol = new OrbitControls(glControl, camera);
			ocontrol.target.Set(0, 1, 0);
			ocontrol.Update();
        }
        public override void InitLighting()
        {
			var spotLight = new SpotLight(0xffffff);
			spotLight.Angle = (float)Math.PI / 5;
			spotLight.Penumbra = 0.2f;
			spotLight.Position.Set(2, 3, 3);
			spotLight.CastShadow = true;
			spotLight.Shadow.Camera.Near = 3;
			spotLight.Shadow.Camera.Far = 10;
			spotLight.Shadow.MapSize.Width = 1024;
			spotLight.Shadow.MapSize.Height = 1024;
			scene.Add(spotLight);

			var dirLight = new DirectionalLight(0x55505a, 1);
			dirLight.Position.Set(0, 3, 0);
			dirLight.CastShadow = true;
			dirLight.Shadow.Camera.Near = 1;
			dirLight.Shadow.Camera.Far = 10;

			dirLight.Shadow.Camera.CameraRight = 1;
			dirLight.Shadow.Camera.Left = -1;
			dirLight.Shadow.Camera.Top = 1;
			dirLight.Shadow.Camera.Bottom = -1;

			dirLight.Shadow.MapSize.Width = 1024;
			dirLight.Shadow.MapSize.Height = 1024;
			scene.Add(dirLight);
		}
        public override void InitCamera()
        {
			camera = new PerspectiveCamera(36, glControl.AspectRatio, 0.25f, 16);

			camera.Position.Set(0, 1.3f, 3);
		}
		public override void Init()
		{
			base.Init();

			// ***** Clipping planes: *****

			localPlane = new Plane(new Vector3(0, -1, 0), 0.8f);
			globalPlane = new Plane(new Vector3(-1, 0, 0), 0.1f);

			// Geometry

			material = new MeshPhongMaterial()
			{
				Color = Color.Hex(0x80ee10),
				Shininess = 100,
				Side = Constants.DoubleSide,

				// ***** Clipping setup (material): *****
				ClippingPlanes = new List<Plane>() { localPlane },
				ClipShadows = true

			};
			var geometry = new TorusKnotBufferGeometry(0.4f, 0.08f, 95, 20);

			object3d = new Mesh(geometry, material);
			object3d.CastShadow = true;
			scene.Add(object3d);

			var ground = new Mesh(
				new PlaneBufferGeometry(9, 9, 1, 1),
				new MeshPhongMaterial() { Color = Color.Hex(0xa0adaf), Shininess = 150 }
			);

			ground.Rotation.X = -(float)Math.PI / 2; // rotates X/Y to X/Z
			ground.ReceiveShadow = true;
			scene.Add(ground);

			// ***** Clipping setup (renderer): *****
			globalPlanes = new List<Plane> { globalPlane };
			Empty = new List<Plane>();
            renderer.ClippingPlanes = Empty;
            renderer.LocalClippingEnabled = true;


			AddGuiControlsAction = ShowControls;
        }

        public override void Render()
        {
			

			if (!imGuiManager.ImWantMouse) ocontrol.Enabled = true;
			else ocontrol.Enabled = false;
			ocontrol.Update();
			renderer.Render(scene,camera);
			base.ShowGUIControls();

			var time = (float)stopWatch.Elapsed.TotalSeconds;
			object3d.Position.Y = 0.8f;
			object3d.Rotation.X = time * 0.5f;
			object3d.Rotation.Y = time * 0.2f;
			object3d.Scale.SetScalar((float)Math.Cos(time) * 0.125f + 0.875f);
		}

		private void ShowControls()
        {
			if(ImGui.TreeNode("Local Clipping"))
            {
				if(ImGui.Checkbox("Enabled", ref renderer.LocalClippingEnabled))
                {
					material.NeedsUpdate = true;
                }
				if(ImGui.Checkbox("Shadows", ref material.ClipShadows))
                {
					material.NeedsUpdate = true;
                }
				ImGui.SliderFloat("Plane", ref localPlane.Constant, 0.3f, 1.25f);
				ImGui.TreePop();
            }
			if(ImGui.TreeNode("Global Clipping"))
            {
				if(ImGui.Checkbox("Enabled",ref globalPlaneEnabled))
                {
					if (globalPlaneEnabled)
					{
						renderer.ClippingPlanes = globalPlanes;
						renderer.LocalClippingEnabled = true;
					}
					else renderer.ClippingPlanes = Empty;                    			                    
                }
				ImGui.SliderFloat("Plane", ref globalPlane.Constant, -0.4f, 3);
				ImGui.TreePop();
            }
        }
    }
}
