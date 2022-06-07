using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREEExample.Learning.Chapter04
{
    [Example("05.Mesh-Face-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class MeshFaceMaterialExample : BasicMeshMaterialExample
    {
        public Mesh group;

        public MeshFaceMaterialExample() : base()
        {

        }
        public override void BuildGeometry()
        {
            AddSpotLight();

            group = new Mesh();
            // add all the rubik cube elements
            var mats = new List<Material>();
            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0x009e60) });

            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0x0051ba) });

            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0xffd500) });
            // mats.push(new THREE.MeshBasicMaterial({
            // color: 0xffd500
            // }));
            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0xff5800) });
            // mats.push(new THREE.MeshBasicMaterial({
            // color: 0xff5800
            // }));
            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0xC41E3A) });
            // mats.push(new THREE.MeshBasicMaterial({
            // color: 0xC41E3A
            // }));
            mats.Add(new MeshBasicMaterial() { Color = Color.Hex(0xffffff) });
            // mats.push(new THREE.MeshBasicMaterial({
            // color: 0xffffff
            // }));

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        var cubeGeom = new BoxGeometry(2.9f, 2.9f, 2.9f);
                        var cube = new Mesh(cubeGeom, mats);
                        cube.Position.Set(x * 3 - 3, y * 3 - 3, z * 3 - 3);

                        group.Add(cube);
                    }
                }
            }


            group.Scale.Copy(new Vector3(2, 2, 2));
            // call the render function
            scene.Add(group);

        }
        public override void Render()
        {
            controls.Update();
            renderer.Render(scene, camera);

            group.Rotation.Y = step += rotationSpeed;
            group.Rotation.Z = step -= rotationSpeed;
            group.Rotation.X = step += rotationSpeed;
        }
       
    }
}
