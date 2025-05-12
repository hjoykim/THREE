using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using THREE;

namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing With Shadow", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancingWithShadow : Example
    {
        public OrbitControls orbitControl;
        public InstancingWithShadow() { }
        public override void InitLighting()
        {
            var light = new THREE.AmbientLight(0x808080);
            var light2 = new THREE.PointLight(0xff4040, 1.5f, 0);
            var pointLightHelper = new THREE.PointLightHelper(light2, 1.0f);

            light2.Position.Set(10, 20, 10);

            light2.CastShadow = true;
            light2.Shadow.Camera.Near = 0.1f;
            light2.Shadow.Camera.Far = 200;
            light2.Shadow.Bias = 0.005f; // reduces self-shadowing on double-sided objects
            light2.Shadow.MapSize.Width = 512;  // default
            light2.Shadow.MapSize.Height = 512; // default

            scene.Add(light);
            scene.Add(light2);
            scene.Add(pointLightHelper);
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(45,glControl.Width / glControl.Height,0.1f,1000);
            this.camera.Position.Set(30,30,30);
        }
        public override void InitCameraController()
        {
            orbitControl = new OrbitControls(this, camera);
            orbitControl.EnableDamping = true;
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            
        }
        public override void Init()
        {
            base.Init();
            var geometry = new THREE.PlaneBufferGeometry(100, 100, 1);
            geometry.RotateX((float)-Math.PI * 0.5f); // this is how you can do it
            var material = new THREE.MeshLambertMaterial(){                 
                      Side = Constants.DoubleSide
                    };
            material.Color = THREE.Color.Hex(0x888800);
            
            var plane = new THREE.Mesh(geometry, material);
            plane.ReceiveShadow = true;
            this.scene.Add(plane);

            var boxgeometry = new THREE.BoxBufferGeometry(2, 2, 2);

            var instancedGeometry = new THREE.InstancedBufferGeometry();

            var offset = new THREE.InstancedBufferAttribute<float>(new float[300], 3);
            var orientation = new THREE.InstancedBufferAttribute<float>(new float[400],4);
            instancedGeometry.MaxInstanceCount = 100;
            instancedGeometry.SetAttribute("offset", offset);
            instancedGeometry.SetAttribute("orientation", orientation);

            for (var i = 0; i < 100; i++)
            {
                offset.SetXYZ(
                  i,
                  50 - MathUtils.NextFloat() * 100,
                  MathUtils.NextFloat() * 10,
                  50 - MathUtils.NextFloat() * 100
                );
            }
            offset.NeedsUpdate = true;
            instancedGeometry.SetAttribute("position", boxgeometry.Attributes["position"] as BufferAttribute<float>);
            instancedGeometry.SetAttribute("normal", boxgeometry.Attributes["normal"] as BufferAttribute<float>);
            instancedGeometry.SetAttribute("uv", boxgeometry.Attributes["uv"] as BufferAttribute<float>);
            instancedGeometry.Index = boxgeometry.Index;

            var instanceMaterial = new THREE.MeshLambertMaterial();
            instanceMaterial.OnBeforeCompile = (Hashtable parameters, IGLRenderer renderer) =>
            {
                parameters["vertexShader"] =
                @"
                    attribute vec3 offset;
                    attribute vec4 orientation;

                    vec3 applyQuaternionToVector(vec4 q, vec3 v)
                    {
                        return v + 2.0 * cross(q.xyz, cross(q.xyz, v) + q.w * v);
                    }
                " + parameters["vertexShader"];
                parameters["vertexShader"] = (parameters["vertexShader"] as string).Replace(
                  "#include <project_vertex>",
                  @"                   
                    vec3 vPosition = applyQuaternionToVector(orientation, transformed);
                    vec4 mvPosition = modelViewMatrix * vec4(vPosition, 1.0);
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(offset + vPosition, 1.0);
                    ");

            };

            instanceMaterial.Color = THREE.Color.Hex(0x4400ff);
            Mesh object3d = new THREE.Mesh(instancedGeometry, instanceMaterial);
            object3d.FrustumCulled = false;
            object3d.CustomDistanceMaterial = new THREE.MeshDistanceMaterial(){
                DepthPacking = Constants.RGBADepthPacking,
                AlphaTest= 0.5f
            };
            object3d.CustomDistanceMaterial.OnBeforeCompile =(Hashtable parameters,IGLRenderer renderer) => 
            {
                // app specific instancing shader code
                if (parameters.ContainsKey("depthPacking")) parameters["depthPacking"] = 3201;
                parameters["vertexShader"] =
                //@"  #define DEPTH_PACKING 3201
                @"    attribute vec3 offset;
                    attribute vec4 orientation;
                    vec3 applyQuaternionToVector(vec4 q, vec3 v)
                    {
                        return v + 2.0 * cross(q.xyz, cross(q.xyz, v) + q.w * v);
                    }"        +parameters["vertexShader"];

                parameters["vertexShader"] = (parameters["vertexShader"] as string).Replace(
                  "#include <project_vertex>",
                  @"                    
                    vec3 vPosition = offset + applyQuaternionToVector(orientation, transformed);
                    vec4 mvPosition = modelMatrix * vec4(vPosition, 1.0);
                    transformed = vPosition;
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(transformed, 1.0);");

                //parameters["fragmentShader"] =
                //  "#define DEPTH_PACKING 3201" + "\n" + parameters["fragmentShader"];
            };

            object3d.CastShadow = true;
            object3d.ReceiveShadow = true;
            scene.Add(object3d);
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                orbitControl.Enabled = true;
            else
                orbitControl.Enabled = false;

            orbitControl.Update();
            renderer.Render(scene, camera);
        }
    }
}
