using ImGuiNET;
using Rhino.DocObjects;
using Rhino.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using THREE;

namespace THREEExample.ThreeJs.Loader
{    
    [Example("loader_3dm", ExampleCategory.ThreeJs, "loader")]
    public class Rhino3dmLoaderExample : Example
    {
        public Rhino3dmLoaderExample() :base() { }
        public override void InitCamera()
        {           
            camera.Fov = 60.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 1.0f;
            camera.Far = 1000.0f;
            camera.Position.Set(26, -40, 5);
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var directionalLight = new THREE.DirectionalLight(0xffffff, 2);
            directionalLight.Position.Set(0, 0, 2);
            scene.Add(directionalLight);
        }
        public override void Init()
        {
            Object3D.DefaultUp = new THREE.Vector3(0, 0, 1);

            base.Init();

            string filePath = "../../../../assets/models/3dm/Rhino_Logo.3dm";

            Rhino3dmLoader loader = new Rhino3dmLoader();
            Object3D object3d = loader.Load(filePath);

            scene.Add(object3d);

            List<Layer> layerList = object3d.UserData["layers"] as List<Layer>;
            bool[] visible = new bool[layerList.Count];
            for (int i = 0; i < layerList.Count; i++)
                visible[i] = layerList[i].IsVisible;

            AddGuiControlsAction = () =>
            {
                for(int i=0;i<layerList.Count;i++)
                {
                    var layer = layerList[i];
                    string name = layer.Name;
                    
                    
                    if (ImGui.Checkbox(name, ref visible[i]))
                    {
                        scene.Traverse((o) =>
                        {
                            if (o.UserData.ContainsKey("attributes"))
                            {                               
                                var attributes = o.UserData["attributes"] as ObjectAttributes;
                                var layerName = layerList[attributes.LayerIndex].Name;
                                if (layerName==name)
                                {
                                    o.Visible = visible[i];
                                    layer.IsVisible = visible[i];
                                }
                            }
                        });
                    }

                }                
               
            };
        }

    }

}
