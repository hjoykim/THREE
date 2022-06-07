using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE.Cameras;
using THREE.Core;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;

namespace THREEExample.Three.Misc.Controls
{
    public class TransformControlsMine : Object3D
    {
        public Group gizmo;
        public Object3D helperTranslate;
        public Object3D pickerTranslate;
        MeshBasicMaterial gizmoMaterial,
            matInvisible,
            matRed,
            matGreen,
            matBlue,
            matRedTransparent,
            matGreenTransparent,
            matBlueTransparent,
            matWhiteTransparent,
            matYellowTransparent,
            matYellow,
            matGray;
        LineBasicMaterial gizmoLineMaterial, matHelper;
        Camera camera;
        Control glControl;
        public TransformControlsMine(Control glControl, Camera camera) : base()
        {
            this.camera = camera;
            this.glControl = glControl;
            #region matrial initial
            gizmoMaterial = new MeshBasicMaterial()
            {
                DepthTest = false,
                DepthWrite = false,
                Fog = false,
                ToneMapped = false,
                Transparent = true

            };

            gizmoLineMaterial = new LineBasicMaterial()
            {
                DepthTest = false,
                DepthWrite = false,
                Fog = false,
                ToneMapped = false,
                Transparent = true

            };
            matInvisible = gizmoMaterial.Clone();
            matInvisible.Opacity = 0.15f;

            matHelper = gizmoLineMaterial.Clone();
            matHelper.Opacity = 0.5f;

            matRed = gizmoMaterial.Clone();
            matRed.Color = Color.Hex(0xff0000);

            matGreen = gizmoMaterial.Clone();
            matGreen.Color = Color.Hex(0x00ff00);

            matBlue = gizmoMaterial.Clone();
            matBlue.Color = Color.Hex(0x0000ff);

            matRedTransparent = gizmoMaterial.Clone();
            matRedTransparent.Color = Color.Hex(0xff0000);
            matRedTransparent.Opacity = 0.5f;

            matGreenTransparent = gizmoMaterial.Clone();
            matGreenTransparent.Color = Color.Hex(0x00ff00);
            matGreenTransparent.Opacity = 0.5f;

            matBlueTransparent = gizmoMaterial.Clone();
            matBlueTransparent.Color = Color.Hex(0x0000ff);
            matBlueTransparent.Opacity = 0.5f;

            matWhiteTransparent = gizmoMaterial.Clone();
            matWhiteTransparent.Color = Color.Hex(0xffffff);
            matWhiteTransparent.Opacity = 0.25f;

            matYellowTransparent = gizmoMaterial.Clone();
            matYellowTransparent.Color = Color.Hex(0xffff00);
            matYellowTransparent.Opacity = 0.25f;

            matYellow = gizmoMaterial.Clone();
            matYellow.Color = Color.Hex(0xffff00);

            matGray = gizmoMaterial.Clone();
            matGray.Color = Color.Hex(0x787878);
            #endregion

            helperTranslate = GetHelperTranslate();
            pickerTranslate = GetPickerTranslate();
            gizmo = new Group();
            gizmo.Add(helperTranslate);
            gizmo.Add(pickerTranslate);
            this.Add(gizmo);
        }
        Vector3 worldPosition = new Vector3();
        Vector3 cameraPosition = new Vector3();
        Quaternion cameraQuaternion = new Quaternion();
        Vector3 cameraScale = new Vector3();
        float size = 1;
        /*
        public override void UpdateMatrixWorld(bool force = false)
        {

            camera.UpdateMatrixWorld();
            camera.MatrixWorld.Decompose(cameraPosition, cameraQuaternion, cameraScale);
            List<Object3D> handles = new List<Object3D>();

            handles = handles.Concat(pickerTranslate.Children);
            handles = handles.Concat(helperTranslate.Children);

            for(int i=0;i<handles.Count;i++)
            {
                Object3D handle = handles[i];
                handle.Visible = true;

                handle.Rotation.Set(0, 0, 0);
                handle.Position.Copy(worldPosition);

                float factor;
                if (camera is OrthographicCamera)
                {
                    factor = (camera.Top - camera.Bottom) / camera.Zoom;
                }
                else
                {
                    factor = worldPosition.DistanceTo(cameraPosition) * (float)(Math.Min(1.9 * Math.Tan(Math.PI * camera.Fov / 360) / camera.Zoom, 7));
                }
                handle.Scale.Set(1, 1, 1).MultiplyScalar(factor * size / 4);
            }
            base.UpdateMatrixWorld(force);
        }
        */
        private TorusGeometry CircleGeometry(float radius, float arc)
        {
            var geometry = new TorusGeometry(radius, 0.0075f, 3, 64, arc * (float)System.Math.PI * 2);
            geometry.RotateY((float)System.Math.PI / 2);
            geometry.RotateX((float)System.Math.PI / 2);
            return geometry;
        }

        // Special geometry for transform helper. If scaled with position vector it spans from [0,0,0] to position
        private BufferGeometry TranslateHelperGeometry()
        {

            var geometry = new BufferGeometry();

            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { 0, 0, 0, 1, 1, 1 }, 3));

            return geometry;

        }
        private BufferGeometry TranslateLineGeometry()
        {
            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { 0, 0, 0, 1, 0, 0 }, 3));

            return geometry;
        }
        private Group GetHelperTranslate()
        {
            #region when parsing from Hashtable, it's occurring error, so I define method for getting helper translate
            var object3d = new Group();

            var start = new Mesh(new OctahedronGeometry(0.01f, 2), matHelper) { Name = "START", Tag = "helper" };
            object3d.Add(start);

            var end = new Mesh(new OctahedronGeometry(0.01f, 2), matHelper) { Name = "END", Tag = "helper" };
            object3d.Add(end);

            var delta = new Mesh(TranslateHelperGeometry(), matHelper) { Name = "DELTA", Tag = "helper" };
            object3d.Add(delta);

            var lineGeometry = TranslateLineGeometry();
            var xline = new Line(TranslateLineGeometry(), matHelper.Clone()) { Name = "X", Tag = "helper", Position = new Vector3(-1e3f, 0, 0), Scale = new Vector3(1e6f, 1, 1) };
            xline.UpdateMatrix();
            object3d.Add(xline);

            var yline = new Line(TranslateLineGeometry(), matHelper.Clone()) { Name = "Y", Tag = "helper", Position = new Vector3(0, -1e3f, 0), Scale = new Vector3(1e6f, 1, 1) };
            yline.Rotation.Set(0, 0, (float)Math.PI / 2);
            yline.UpdateMatrix();
            object3d.Add(yline);

            var zline = new Line(TranslateLineGeometry(), matHelper.Clone()) { Name = "Z", Tag = "helper", Position = new Vector3(0, 0, -1e3f), Scale = new Vector3(1e6f, 1, 1) }; ;
            zline.Rotation.Set(0, -(float)Math.PI / 2, 0);
            zline.UpdateMatrix();
            object3d.Add(zline);

            object3d.Tag = "translate";

            return object3d;

            #endregion
        }
        private Group GetPickerTranslate()
        {
            var object3d = new Group();

            var xpicker1 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "X", Position = new Vector3(0.3f, 0, 0) };
            xpicker1.Rotation.Set(0, 0, -(float)Math.PI / 2);
            xpicker1.UpdateMatrix();
            object3d.Add(xpicker1);

            var xpicker2 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "X", Position = new Vector3(-0.3f, 0, 0) };
            xpicker2.Rotation.Set(0, 0, (float)Math.PI / 2);
            xpicker2.UpdateMatrix();
            object3d.Add(xpicker2);

            var ypicker1 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "Y", Position = new Vector3(0, 0.3f, 0) };
            ypicker1.UpdateMatrix();
            object3d.Add(ypicker1);

            var ypicker2 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "Y", Position = new Vector3(0, -0.3f, 0) };
            ypicker2.Rotation.Set(0, 0, (float)Math.PI / 2);
            ypicker2.UpdateMatrix();
            object3d.Add(ypicker2);

            var zpicker1 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "Z", Position = new Vector3(0, 0, 0.3f) };
            xpicker1.Rotation.Set(0, 0, (float)Math.PI / 2);
            xpicker1.UpdateMatrix();
            object3d.Add(zpicker1);

            var zpicker2 = new Mesh(new CylinderGeometry(0.2f, 0, 0.6f, 4), matInvisible) { Name = "Z", Position = new Vector3(0, 0, -0.3f) };
            xpicker2.Rotation.Set(0, 0, -(float)Math.PI / 2);
            xpicker2.UpdateMatrix();
            object3d.Add(zpicker2);
           

            var xyzpicker = new Mesh(new OctahedronGeometry(0.2f, 0), matInvisible) { Name = "XYZ" };
            object3d.Add(xyzpicker);

            var xypicker = new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible) { Name = "XY", Position = new Vector3(0.15f, 0.15f, 0) };
            object3d.Add(xypicker);

            var yzpicker = new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible) { Name = "YZ", Position = new Vector3(0, 0.15f, 0.15f) };
            yzpicker.Rotation.Set(0, (float)Math.PI / 2, 0);
            yzpicker.UpdateMatrix();
            object3d.Add(yzpicker);

            var xzpicker = new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible) { Name = "XZ", Position = new Vector3(0.15f, 0, 0.15f) };
            xzpicker.Rotation.Set(-(float)Math.PI / 2, 0, 0);
            xypicker.UpdateMatrix();
            object3d.Add(yzpicker);

            object3d.Tag = "picker";
            return object3d;

        }
    }
}
