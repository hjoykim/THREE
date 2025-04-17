using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing Example", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancingExample : Example
    {
        public InstancingExample()
        {

        }
        public override void InitCamera()
        {
            camera = new PerspectiveCamera(60, glControl.AspectRatio, 1, 1000);
            camera.Position.Set(0,8,13);            
        }
        public override void Init()
        {
            base.Init();
            var cylGeom = new THREE.CylinderBufferGeometry(0.5f, 1, 2, 8);
            var cylMat = new THREE.MeshBasicMaterial() { Color = Color.ColorName(ColorKeywords.red), PolygonOffset = true, PolygonOffsetFactor = 1 };
            var cylinder = new THREE.InstancedMesh(cylGeom, cylMat, 100);
            var dummy = new THREE.Object3D();
            var mat4 = new THREE.Matrix4();
            var counter = 0;
            List<float> pos = new List<float>();
            for (var z = 0; z < 10; z++) {
                for (var x = 0; x < 10; x++) {
                    dummy.Position = new Vector3().Set(-4.5f + x, 0, -4.5f + z).MultiplyScalar(2);
                    dummy.UpdateMatrix();
                    cylinder.SetMatrixAt(counter, dummy.Matrix);
                    pos.Add(dummy.Position.X, dummy.Position.Y, dummy.Position.Z);
                    counter++;
                }
            }
            cylinder.InstanceMatrix.NeedsUpdate = true;
            scene.Add(cylinder);

            var lineGeom = new THREE.EdgesGeometry(cylGeom);
            var instancedLineGeom = new THREE.InstancedBufferGeometry();
            instancedLineGeom.Copy(lineGeom);

            instancedLineGeom.InstanceCount = int.MaxValue;
            instancedLineGeom.SetAttribute("instPos", new THREE.InstancedBufferAttribute<float>(pos.ToArray(), 3));

            var lineMat = new THREE.LineBasicMaterial() {
                Color = Color.ColorName(ColorKeywords.yellow),            
                OnBeforeCompile = (Hashtable parameters, IGLRenderer render) =>
                {
                    parameters["vertexShader"] = string.Format(
                        @"attribute vec3 instPos;
                        {0}", (parameters["vertexShader"] as string).Replace("#include <begin_vertex>", @"#include <begin_vertex>
                        transformed += instPos;"));

                }
            };

            var lines = new THREE.LineSegments(instancedLineGeom, lineMat);
            scene.Add(lines);

        }

        public override void Render()
        {
            base.Render();
        }
    }
    
}

