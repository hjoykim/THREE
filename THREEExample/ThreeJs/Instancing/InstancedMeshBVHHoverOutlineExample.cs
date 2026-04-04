using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Instancing
{
    [Example("Instancing MeshBVH Example", ExampleCategory.ThreeJs, "Instancing")]
    public class InstancedMeshBVHHoverOutlineExample : Example
    {
        private InstancedMesh instancedMesh;
        private InstancedMesh outlineMesh;
        MeshBVH bvh;
        private Raycaster raycaster = new Raycaster();
        private Vector2 mouse = new Vector2();

        private readonly Matrix4 tempMatrix = new Matrix4();
        private readonly Matrix4 outlineMatrix = new Matrix4();
        private readonly Vector3 tempPosition = new Vector3();
        private readonly Quaternion tempQuaternion = new Quaternion();
        private readonly Vector3 tempScale = new Vector3();

        private int hoveredInstanceId = -1;
        private int instanceCount = 0;

        private Color baseColor = Color.Hex(0x4aa3ff);
        private Color hoverColor = Color.Hex(0xffcc33);

        public override void Init()
        {
            base.Init();

            InitRenderer();
            InitCamera();
            InitCameraController();

            scene.Background = Color.Hex(0x20252f);

            camera.Position.Set(20, 20, 20);
            camera.LookAt(0, 0, 0);

            scene.Add(new AmbientLight(0xffffff, 0.9f));

            var dirLight = new DirectionalLight(0xffffff, 1.2f);
            dirLight.Position.Set(10, 20, 10);
            scene.Add(dirLight);

            scene.Add(new AxesHelper(5));
            scene.Add(new GridHelper(80, 80));

            CreateInstancedMeshes();

            this.MouseMove += OnMouseMove;
        }

        private void CreateInstancedMeshes()
        {
            var geometry = new TorusKnotBufferGeometry(0.45f, 0.18f, 120, 16);

            bvh = geometry.ComputeBoundsTree();
            geometry.ComputeBoundsTree();

            var material = new MeshStandardMaterial()
            {
                VertexColors = true,
                Roughness = 0.45f,
                Metalness = 0.10f
            };

            int countX = 20;
            int countY = 8;
            int countZ = 8;
            instanceCount = countX * countY * countZ;

            instancedMesh = new InstancedMesh(geometry, material, instanceCount);

            var dummy = new Object3D();
            var color = new Color();

            int index = 0;
            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int z = 0; z < countZ; z++)
                    {
                        float px = (x - countX / 2f) * 1.8f;
                        float py = (y - countY / 2f) * 1.8f;
                        float pz = (z - countZ / 2f) * 1.8f;

                        dummy.Position.Set(px, py, pz);
                        dummy.Rotation.Set(
                            (float)(0.15 * x),
                            (float)(0.20 * y),
                            (float)(0.12 * z)
                        );
                        dummy.Scale.Set(1, 1, 1);
                        dummy.UpdateMatrix();

                        instancedMesh.SetMatrixAt(index, dummy.Matrix);

                        color.Copy(baseColor);
                        instancedMesh.SetColorAt(index, color);

                        index++;
                    }
                }
            }

            instancedMesh.InstanceMatrix.NeedsUpdate = true;
            if (instancedMesh.InstanceColor != null)
                instancedMesh.InstanceColor.NeedsUpdate = true;

            scene.Add(instancedMesh);

            // -----------------------------
            // 외곽선 전용 InstancedMesh (1개만 사용)
            // -----------------------------
            var outlineMaterial = new MeshBasicMaterial()
            {
                Color = Color.Hex(0xffff00),
                Side = Constants.BackSide,
                Transparent = true,
                Opacity = 0.95f,
                DepthTest = true,
                DepthWrite = false
            };

            outlineMesh = new InstancedMesh(geometry, outlineMaterial, 1);

            // 처음에는 화면 밖으로 숨김
            var hidden = new Object3D();
            hidden.Position.Set(999999, 999999, 999999);
            hidden.UpdateMatrix();

            outlineMesh.SetMatrixAt(0, hidden.Matrix);
            outlineMesh.InstanceMatrix.NeedsUpdate = true;

            scene.Add(outlineMesh);
        }

        private void OnMouseMove(object sender, THREE.MouseEventArgs e)
        {
        
            mouse.X = (float)e.X / glControl.Width * 2f - 1f;
            mouse.Y = -((float)e.Y / glControl.Height * 2f - 1f);

            raycaster.SetFromCamera(mouse, camera);

            List<Intersection> hits;

            var inv = new Matrix4().Copy(instancedMesh.MatrixWorld).Invert();
            var localRay = new Ray(raycaster.ray.origin.Clone(), raycaster.ray.direction.Clone());
            localRay.ApplyMatrix4(inv);

            var worldScale = new Vector3().SetFromMatrixScale(instancedMesh.MatrixWorld);
            var dirScaled = localRay.direction.Clone().Multiply(worldScale);
            double scaleFactor = dirScaled.Length();
            double near = raycaster.near / scaleFactor;
            double far = raycaster.far / scaleFactor;
            hits = bvh.Raycast(localRay, instancedMesh.Material, near, far);
            hits.Sort((a, b) => a.distance.CompareTo(b.distance));

            var intersects = raycaster.IntersectObject(instancedMesh, false);

            if (intersects != null && intersects.Count > 0)
            {
                var hit = intersects[0];
                int instanceId = hit.instanceId;

                if (instanceId != hoveredInstanceId)
                {
                    RestoreHoveredColor();
                    hoveredInstanceId = instanceId;
                    ApplyHoveredColor(hoveredInstanceId);
                    UpdateOutlineFromHoveredInstance(hoveredInstanceId);
                }
            }
            else
            {
                RestoreHoveredColor();
                hoveredInstanceId = -1;
                HideOutline();
            }
        }

        private void ApplyHoveredColor(int instanceId)
        {
            if (instanceId < 0) return;

            var c = new THREE.Color();
            c.Copy(hoverColor);
            instancedMesh.SetColorAt(instanceId, c);

            if (instancedMesh.InstanceColor != null)
                instancedMesh.InstanceColor.NeedsUpdate = true;
        }

        private void RestoreHoveredColor()
        {
            if (hoveredInstanceId < 0) return;

            var c = new THREE.Color();
            c.Copy(baseColor);
            instancedMesh.SetColorAt(hoveredInstanceId, c);

            if (instancedMesh.InstanceColor != null)
                instancedMesh.InstanceColor.NeedsUpdate = true;
        }

        private void UpdateOutlineFromHoveredInstance(int instanceId)
        {
            if (instanceId < 0)
            {
                HideOutline();
                return;
            }

            instancedMesh.GetMatrixAt(instanceId, tempMatrix);

            tempMatrix.Decompose(tempPosition, tempQuaternion, tempScale);

            tempScale.MultiplyScalar(1.08f);

            outlineMatrix.Compose(tempPosition, tempQuaternion, tempScale);
            outlineMesh.SetMatrixAt(0, outlineMatrix);
            outlineMesh.InstanceMatrix.NeedsUpdate = true;
        }

        private void HideOutline()
        {
            var hidden = new Object3D();
            hidden.Position.Set(999999, 999999, 999999);
            hidden.UpdateMatrix();

            outlineMesh.SetMatrixAt(0, hidden.Matrix);
            outlineMesh.InstanceMatrix.NeedsUpdate = true;
        }

        public override void Render()
        {
            controls.Update();
            renderer.Render(scene, camera);
        }
    }
}
