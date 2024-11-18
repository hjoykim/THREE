using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using THREE;

namespace THREEExample.ThreeJs.Loader
{  
    [Example("objLoader", ExampleCategory.ThreeJs, "loader")]
    public class ObjLoaderExample : LightHintExample
    {

        public Group loadObj()
        {
            Group group = new Group();
            string objPath = "../../../../assets/models/gopher/";
            var objFiles = System.IO.Directory.GetFiles(objPath, "*.obj");
            Dictionary<string, object> list = new Dictionary<string, object>() { { "1", new MeshBasicMaterial() }, { "2", new MeshLambertMaterial() } };

            foreach (string thisfileobj in objFiles)
            {               
                var L = new THREE.OBJLoader();
                var O1 = L.Load(thisfileobj);
                group.Add(O1);
                group.Traverse((e) =>
                {
                    if (e is Mesh)
                    {
                        //e.Material.FlatShading = false;
                        //var tempGeom = new Geometry();
                        //if (e.Geometry is BufferGeometry)
                        //{
                        //    (e.Geometry as BufferGeometry).deleteAttribute("normal");
                        //    tempGeom.FromBufferGeometry((BufferGeometry)e.Geometry);
                        //    tempGeom.MergeVertices();
                        //    tempGeom.NormalsNeedUpdate = true;
                        //    (e.Geometry as BufferGeometry).FromGeometry(tempGeom);
                        //    (e.Geometry as BufferGeometry).ComputeVertexNormals();
                        //}
                    }
                });

            }
            return group;
        }
        private Object3D GenerateEdges(Object3D group)
        {
            var lineMaterial = new LineBasicMaterial() { Color = THREE.Color.Hex(0x000000), LineWidth = 2 };
            Object3D edges = new Object3D();
            group.Traverse((o) =>
            {
                if (o is Mesh)
                {
                    var mesh = (Mesh)o;

                    if (mesh.Geometry != null)
                    {
                        var edgeGeometry = new EdgesGeometry(mesh.Geometry, 0.1f);
                        var edgeObject = new LineSegments(edgeGeometry, lineMaterial);
                        edges.Add(edgeObject);
                    }
                }
            });
            return edges;
        }
        public override void BuildScene()
        {
            //string pathFolder = @"E:\01.OpenGLWorkSpace\00.ThreeProjects\THREE_240923\assets\models\obj\1M.OBJ";
            //string fileNameDat = System.IO.Path.Combine(pathFolder, "model.dat");
            //NodeObjExport rootNodeFile = JsonSerializer.Deserialize<NodeObjExport>(System.IO.File.ReadAllText(fileNameDat,System.Text.Encoding.UTF8));

            scene.Background = THREE.Color.ColorName(ColorKeywords.slategray);

            var group = loadObj();
            //var edges = GenerateEdges(group);
            scene.Add(group);
            //scene.Add(edges);

            FitModelToWindow(scene,true);
        }
        
    }
    
}
