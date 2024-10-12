using System;
using System.Collections.Generic;
using THREE;

namespace THREE.Silk.Example.Learning.Utils
{
    public class DemoUtils
    {
        public static void AddHouseAndTree(Scene scene)
        {
            CreateBoundingWall(scene);
            CreateGroundPlane(scene);
            CreateHouse(scene);
            CreateTree(scene);
        }
        private static void CreateBoundingWall(Scene scene)
        {
            var wallLeft = new BoxGeometry(70, 2, 2);
            var wallRight = new BoxGeometry(70, 2, 2);
            var wallTop = new BoxGeometry(2, 2, 50);
            var wallBottom = new BoxGeometry(2, 2, 50);

            var wallMaterial = new MeshPhongMaterial() { Color = THREE.Color.Hex(0xa0522d) };

            var wallLeftMesh = new Mesh(wallLeft, wallMaterial);
            var wallRightMesh = new Mesh(wallRight, wallMaterial);
            var wallTopMesh = new Mesh(wallTop, wallMaterial);
            var wallBottomMesh = new Mesh(wallBottom, wallMaterial);

            wallLeftMesh.Position.Set(15, 1, -25);
            wallRightMesh.Position.Set(15, 1, 25);
            wallTopMesh.Position.Set(-19, 1, 0);
            wallBottomMesh.Position.Set(49, 1, 0);

            scene.Add(wallLeftMesh);
            scene.Add(wallRightMesh);
            scene.Add(wallBottomMesh);
            scene.Add(wallTopMesh);
        }

        internal static Mesh AppliyMeshNormalMaterial(THREE.Geometry geometry, Material material = null)
        {

            if (material == null || material.type != "MeshNormalMaterial")
            {
                material = new MeshNormalMaterial();
                material.Side = Constants.DoubleSide;
            }
            return new Mesh(geometry, material);
        }

        internal static void InitDefaultDirectionalLighting(Scene scene,Vector3 initialPosition=null)
        {
            var position = (initialPosition != null) ? initialPosition : new Vector3(100, 200, 200);

            var dirLight = new DirectionalLight(THREE.Color.Hex(0xffffff));
            dirLight.Position.Copy(position);
            dirLight.Shadow.MapSize.Width = 2048;
            dirLight.Shadow.MapSize.Height = 2048;
            dirLight.CastShadow = true;

            dirLight.Shadow.Camera.Left = -200;
            dirLight.Shadow.Camera.CameraRight = 200;
            dirLight.Shadow.Camera.Top = 200;
            dirLight.Shadow.Camera.Bottom = -200;

            scene.Add(dirLight);

            var ambientLight = new AmbientLight(THREE.Color.Hex(0x343434));
            ambientLight.Name = "ambientLight";
            scene.Add(ambientLight);
        }

        private static void CreateGroundPlane(Scene scene)
        {
            // create the ground plane
            var planeGeometry = new PlaneGeometry(70, 50);
            var planeMaterial = new MeshPhongMaterial() { Color = THREE.Color.Hex(0x9acd32) };
            var plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            // rotate and position the plane
            plane.Rotation.X = -0.5f * (float)Math.PI;
            plane.Position.X = 15;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);
        }
        private static void CreateHouse(Scene scene)
        {
            var roof = new ConeGeometry(5, 4);
            var baseGeom = new CylinderGeometry(5, 5, 6);

            // create the mesh
            var roofMesh = new Mesh(roof, new MeshPhongMaterial() { Color = THREE.Color.Hex(0x8b7213) });
            var baseMesh = new Mesh(baseGeom, new MeshPhongMaterial() { Color = THREE.Color.Hex(0xffe4c4) });

            roofMesh.Position.Set(25, 8, 0);
            baseMesh.Position.Set(25, 3, 0);

            roofMesh.ReceiveShadow = true;
            baseMesh.ReceiveShadow = true;
            roofMesh.CastShadow = true;
            baseMesh.CastShadow = true;

            scene.Add(roofMesh);
            scene.Add(baseMesh);
        }
        private static void CreateTree(Scene scene)
        {
            var trunk = new BoxGeometry(1, 8, 1);
            var leaves = new SphereGeometry(4);

            var trunkMesh = new Mesh(trunk, new MeshPhongMaterial() { Color = THREE.Color.Hex(0x8b4513) });

            var leavesMesh = new Mesh(leaves, new MeshPhongMaterial() { Color = THREE.Color.Hex(0x00ff00) });

            trunkMesh.Position.Set(-10, 4, 0);

            leavesMesh.Position.Set(-10, 12, 0);

            trunkMesh.CastShadow = true;

            trunkMesh.ReceiveShadow = true;

            leavesMesh.CastShadow = true;

            leavesMesh.ReceiveShadow = true;

            scene.Add(trunkMesh);

            scene.Add(leavesMesh);
        }

        public static Mesh AddGroundPlane(Scene scene)
        {
            var planeGeometry = new PlaneGeometry(60, 20, 120, 120);
            var planeMaterial = new MeshPhongMaterial() { Color = THREE.Color.Hex(0xffffff) };
            Mesh plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = -0.5f * (float)System.Math.PI;
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            return plane;
        }

        public static Mesh AddDefaultCube(Scene scene)
        {
            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshLambertMaterial()
            {
                Color = THREE.Color.Hex(0xff0000)
            };
            var cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;

            // position the cube
            cube.Position.X = -4;
            cube.Position.Y = 3;
            cube.Position.Z = 0;

            // add the cube to the scene
            scene.Add(cube);

            return cube;
        }

        public static Mesh AddDefaultSphere(Scene scene)
        {
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshLambertMaterial()
            {
                Color = THREE.Color.Hex(0x7777ff)
            };
            var sphere = new Mesh(sphereGeometry, sphereMaterial);

            // position the sphere
            sphere.Position.X = 20;
            sphere.Position.Y = 0;
            sphere.Position.Z = 2;
            sphere.CastShadow = true;

            // add the sphere to the scene
            scene.Add(sphere);

            return sphere;
        }

        //public void ComputeNormalsGroup(Group group)
        //{
        //    group.Traverse(o =>
        //    {
        //        if (o is Mesh)
        //        {
        //            var tempGeom = new Geometry();
        //            tempGeom.FromBufferGeometry((BufferGeometry)o.Geometry);
        //            tempGeom.ComputeFaceNormals();
        //            tempGeom.MergeVertices();
        //            tempGeom.ComputeFlatVertexNormals();

        //            tempGeom.NormalsNeedUpdate = true;

        //            o.Geometry = tempGeom;
        //        }
        //    });
        //}
        public static void ComputeNormalsGroup(Object3D group)
        {
            //group.Traverse(o =>
            //{
            //    if (o is Mesh)
            //    {
            //        var tempGeom = new Geometry();
            //        tempGeom.FromBufferGeometry((BufferGeometry)o.Geometry);
            //        tempGeom.ComputeFaceNormals();
            //        tempGeom.MergeVertices();
            //        tempGeom.ComputeFlatVertexNormals();

            //        tempGeom.NormalsNeedUpdate = true;

            //        o.Geometry = tempGeom;
            //    }
            //});
            if (group is Mesh)
            {
                var tempGeom = new THREE.Geometry();
                tempGeom.FromBufferGeometry((BufferGeometry)group.Geometry);
                tempGeom.ComputeFaceNormals();
                tempGeom.MergeVertices();
                tempGeom.ComputeVertexNormals();

                tempGeom.NormalsNeedUpdate = true;

                // group = new THREE.BufferGeometry();
                // group.fromGeometry(tempGeom);
                group.Geometry = tempGeom;
            }
            else if (group is Group)
            {
                group.Children.ForEach(child => { ComputeNormalsGroup(child); });
            }
        }
        //public void SetMaterialGroup(Material material, Group group)
        //{
        //    group.Traverse(o =>
        //    {
        //        o.Material = material;
        //        if (o is Mesh && o.Materials.Count > 1)
        //        {
        //            for (var i = 0; i < o.Materials.Count; i++)
        //                o.Materials[i] = material;
        //        }
        //    });
        //}
        public static void SetMaterialGroup(Material material, Object3D group)
        {
            //group.Traverse(o =>
            //{
            //    o.Material = material;
            //    if(o is Mesh && o.Materials.Count>1)
            //    {
            //        for (var i = 0; i < o.Materials.Count; i++)
            //            o.Materials[i] = material;
            //    }
            //});

            if (group is Mesh)
            {
                group.Material = material;
            }
            else if (group is Group)
            {
                group.Children.ForEach(child => { SetMaterialGroup(material, child); });
            }
        }

        public static Mesh AddLargeGroundPlane(Scene scene, bool useTexture = false)
        {
            var planeGeometry = new PlaneGeometry(10000, 10000);
            var planeMaterial = new MeshPhongMaterial() { Color = THREE.Color.Hex(0xffffff) };

            if (useTexture)
            {
                planeMaterial.Map = TextureLoader.Load("../../../../assets/textures/general/floor-wood.jpg");
                planeMaterial.Map.WrapS = Constants.RepeatWrapping;
                planeMaterial.Map.WrapT = Constants.RepeatWrapping;
                planeMaterial.Map.Repeat.Set(80, 80);
            }

            var plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            // rotate and position the plane
            plane.Rotation.X = (float)(-0.5f * Math.PI);
            plane.Position.X = 0;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);

            return plane;
        }

        public static void InitDefaultLighting(Scene scene, Vector3 initialPosition = null)
        {
            var position = (initialPosition != null) ? initialPosition : new Vector3(-10, 30, 40);

            var spotLight = new SpotLight(THREE.Color.Hex(0xffffff));
            spotLight.Position.Copy(position);
            spotLight.Shadow.MapSize.Width = 2048;
            spotLight.Shadow.MapSize.Height = 2048;
            spotLight.Shadow.Camera.Fov = 15;
            spotLight.CastShadow = true;
            spotLight.Decay = 2;
            spotLight.Penumbra = 0.05f;
            spotLight.Name = "spotLight";

            scene.Add(spotLight);

            var ambientLight = new AmbientLight(THREE.Color.Hex(0x343434));
            ambientLight.Name = "ambientLight";
            scene.Add(ambientLight);
        }
        public static Mesh CreateMultiMaterialObject(THREE.Geometry geometry, List<Material> materials)
        {
            Mesh mesh = new Mesh();

            for (int i = 0; i < materials.Count; i++)
            {
                mesh.Add(new Mesh(geometry, materials[i]));
            }
            return mesh;
        }
        public static Mesh AppliyMeshNormalMaterial(THREE.Geometry geometry, ref Material material )
        {

            if (material == null || material.type != "MeshNormalMaterial")
            {
                material = new MeshNormalMaterial();
                material.Side = Constants.DoubleSide;
            }
            return new Mesh(geometry, material);
        }

        public static Mesh AppliyMeshStandardMaterial(THREE.Geometry geometry, ref Material material)
        {

            if (material == null || material.type != "MeshStandardMaterial")
            {
                material = new MeshStandardMaterial() { Color = THREE.Color.Hex(0xff0000) };
                material.Side = Constants.DoubleSide;
            }

            return new Mesh(geometry, material);
        }

        public static Shape DrawShape()
        {
            var shape = new Shape();

            // startpoint
            shape.MoveTo(10, 10, 0);

            // straight line upwards
            shape.LineTo(10, 40, 0);

            // the top of the figure, curve to the right
            shape.BezierCurveTo(15, 25, 25, 25, 30, 40);

            // spline back down
            shape.SplineThru(new List<Vector3>()

            {   new Vector3(32, 30,0),
                new Vector3(28, 20,0),
                new Vector3(30, 10,0),
             });

            // curve at the bottom
            shape.QuadraticCurveTo(20, 15, 10, 10);

            // add 'eye' hole one
            var hole1 = new THREE.Path();
            hole1.AbsEllipse(16, 24, 2, 3, 0, (float)Math.PI * 2, true);
            shape.Holes.Add(hole1);

            // add 'eye hole 2'
            var hole2 = new THREE.Path();
            hole2.AbsEllipse(23, 24, 2, 3, 0, (float)Math.PI * 2, true);
            shape.Holes.Add(hole2);

            // add 'mouth'
            var hole3 = new THREE.Path();
            hole3.AbsArc(20, 16, 2, 0, (float)Math.PI, true);
            shape.Holes.Add(hole3);

            // return the shape
            return shape;
        }
        public static void SetRandomColors(Object3D object3D)
        {
            var children = object3D.Children;
            if (children.Count > 0)
            {
                foreach (var e in children)
                {
                    SetRandomColors(e);
                }
            }
            else
            {
                if (object3D is Mesh)
                {
                    if (object3D.Materials.Count > 1)
                    {
                        foreach (var m in object3D.Materials)
                        {
                            m.Color = new THREE.Color().Random();
                            if (m.Name.IndexOf("building") == 0)
                            {
                                m.Emissive = new THREE.Color(0x444444);
                                m.Transparent = true;
                                m.Opacity = 0.8f;
                            }
                        }
                    }
                    else
                    {
                        object3D.Material.Color = new THREE.Color().Random();
                        if (object3D.Material.Name.IndexOf("building") == 0)
                        {
                            object3D.Material.Emissive = new THREE.Color(0x444444);
                            object3D.Material.Transparent = true;
                            object3D.Material.Opacity = 0.8f;
                        }
                    }
                }
            }
        }
    }
}
