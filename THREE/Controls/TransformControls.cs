using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace THREE
{

    public class TransformControls : Object3D, INotifyPropertyChanged
    {
        Vector3 _tempVector = new Vector3();
        Vector3 _tempVector2 = new Vector3();
        Quaternion _tempQuaternion = new Quaternion();
        Hashtable _unit = new Hashtable() {
            {"X", new Vector3( 1, 0, 0 ) },
            {"Y", new Vector3( 0, 1, 0 ) },
            {"Z", new Vector3( 0, 0, 1 ) }
        };


        public TransformControlsGizmo _gizmo;
        public TransformControlsPlane _plane;
        //TransformControlsPlane _plane; = new TransformControlsPlane();

        #region property
        private Camera _camera;
        public Camera camera
        {
            get { return _camera; }
            set
            {
                if (value != _camera)
                {
                    _camera = value;
                    _plane["camera"] = value;
                    _gizmo["camera"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Object3D _object3D;
        public Object3D object3D
        {
            get { return _object3D; }
            set
            {
                if (value != _object3D)
                {
                    _object3D = value;
                    _plane["object3D"] = value;
                    _gizmo["object3D"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private bool _enabled = true;
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    _plane["enabled"] = value;
                    _gizmo["enabled"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private string _axis;
        public string axis
        {
            get { return _axis; }
            set
            {
                if (value != _axis)
                {
                    _axis = value;

                    _plane["axis"] = value;
                    _gizmo["axis"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private string _mode = "translate";
        public string mode
        {
            get { return _mode; }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    _plane["mode"] = value;
                    _gizmo["mode"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private float _translationSnap;
        public float translationSnap
        {
            get { return _translationSnap; }
            set
            {
                if (value != _translationSnap)
                {
                    _translationSnap = value;
                    _plane["translationSnap"] = value;
                    _gizmo["translationSnap"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private float _rotationSnap;
        public float rotationSnap
        {
            get { return _rotationSnap; }
            set
            {
                if (value != _rotationSnap)
                {
                    _rotationSnap = value;
                    _plane["rotationSnap"] = value;
                    _gizmo["rotationSnap"] = value;

                    OnPropertyChanged();
                }
            }
        }

        private float _scaleSnap;
        public float scaleSnap
        {
            get { return _scaleSnap; }
            set
            {
                if (value != _scaleSnap)
                {
                    _scaleSnap = value;
                    _plane["scaleSnap"] = value;
                    _gizmo["scaleSnap"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private string _space = "world";
        public string space
        {
            get { return _space; }
            set
            {
                if (value != _space)
                {
                    _space = value;
                    _plane["space"] = value;
                    _gizmo["space"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private float _size = 1;
        public float size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    _plane["size"] = value;
                    _gizmo["size"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private bool _dragging = false;
        public bool dragging
        {
            get { return _dragging; }
            set
            {
                if (value != _dragging)
                {
                    _dragging = value;
                    _plane["dragging"] = value;
                    _gizmo["dragging"] = value;
                    /* add the guide axis line*/
                    if (_dragging && "translate".Equals(mode))
                        _plane.Material.Visible = true;
                    else
                        _plane.Material.Visible = false;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showX = true;
        public bool showX
        {
            get { return _showX; }
            set
            {
                if (value != _showX)
                {
                    _showX = value;
                    _plane["showX"] = value;
                    _gizmo["showX"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private bool _showY = true;
        public bool showY
        {
            get { return _showY; }
            set
            {
                if (value != _showY)
                {
                    _showY = value;
                    _plane["showY"] = value;
                    _gizmo["showY"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private bool _showZ = true;
        public bool showZ
        {
            get { return _showZ; }
            set
            {
                if (value != _showZ)
                {
                    _showZ = value;
                    _plane["showZ"] = value;
                    _gizmo["showZ"] = value;

                    OnPropertyChanged();
                }
            }
        }

        private Vector3 _worldPosition = new Vector3();
        public Vector3 worldPosition
        {
            get { return _worldPosition; }
            set
            {
                if (value != _worldPosition)
                {
                    _worldPosition = value;
                    _plane["worldPosition"] = value;
                    _gizmo["worldPosition"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _worldPositionStart = new Vector3();
        public Vector3 worldPositionStart
        {
            get { return _worldPositionStart; }
            set
            {
                if (value != _worldPositionStart)
                {
                    _worldPositionStart = value;
                    _plane["worldPositionStart"] = value;
                    _gizmo["worldPositionStart"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _worldQuaternion = new Quaternion();
        public Quaternion worldQuaternion
        {
            get { return _worldQuaternion; }
            set
            {
                if (value != _worldQuaternion)
                {
                    _worldQuaternion = value;
                    _plane["worldQuaternion"] = value;
                    _gizmo["worldQuaternion"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _worldQuaternionStart = new Quaternion();
        public Quaternion worldQuaternionStart
        {
            get { return _worldQuaternionStart; }
            set
            {
                if (value != _worldQuaternionStart)
                {
                    _worldQuaternionStart = value;
                    _plane["worldQuaternionStart"] = value;
                    _gizmo["worldQuaternionStart"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _cameraPosition = new Vector3();
        public Vector3 cameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                if (value != _cameraPosition)
                {
                    _cameraPosition = value;
                    _plane["cameraPosition"] = value;
                    _gizmo["cameraPosition"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _cameraQuaternion = new Quaternion();
        public Quaternion cameraQuaternion
        {
            get { return _cameraQuaternion; }
            set
            {
                if (value != _cameraQuaternion)
                {
                    _cameraQuaternion = value;
                    _plane["cameraQuaternion"] = value;
                    _gizmo["cameraQuaternion"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _pointStart = new Vector3();
        public Vector3 pointStart
        {
            get { return _pointStart; }
            set
            {
                if (value != _pointStart)
                {
                    _pointStart = value;
                    _plane["pointStart"] = value;
                    _gizmo["pointStart"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _pointEnd = new Vector3();
        public Vector3 pointEnd
        {
            get { return _pointEnd; }
            set
            {
                if (value != _pointEnd)
                {
                    _pointEnd = value;
                    _plane["pointEnd"] = value;
                    _gizmo["pointEnd"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _rotationAxis = new Vector3();
        public Vector3 rotationAxis
        {
            get { return _rotationAxis; }
            set
            {
                if (value != _rotationAxis)
                {
                    _rotationAxis = value;
                    _plane["rotationAxis"] = value;
                    _gizmo["rotationAxis"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private float _rotationAngle = 0;
        public float rotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                if (value != _rotationAngle)
                {
                    _rotationAngle = value;
                    _plane["rotateAngle"] = value;
                    _gizmo["rotateAngle"] = value;

                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _eye = new Vector3();
        public Vector3 eye
        {
            get { return _eye; }
            set
            {
                if (value != _eye)
                {
                    _eye = value;
                    _plane["eye"] = value;
                    _gizmo["eye"] = value;

                    OnPropertyChanged();
                }
            }
        }
        #endregion
        Vector3 _offset = new Vector3();
        Vector3 _startNorm = new Vector3();
        Vector3 _endNorm = new Vector3();
        Vector3 _cameraScale = new Vector3();


        Vector3 _parentPosition = new Vector3();
        Quaternion _parentQuaternion = new Quaternion();
        Quaternion _parentQuaternionInv = new Quaternion();
        Vector3 _parentScale = new Vector3();

        Vector3 _worldScaleStart = new Vector3();
        Quaternion _worldQuaternionInv = new Quaternion();
        Vector3 _worldScale = new Vector3();

        Vector3 _positionStart = new Vector3();
        Quaternion _quaternionStart = new Quaternion();
        Vector3 _scaleStart = new Vector3();

        Raycaster _raycaster = new Raycaster();

        Vector2 mouse = new Vector2();

        #region mouse action
        public Action<string> _mouseDownEvent;
        public Action<string> _mouseUpEvent;
        public Action<object> _changeEvent;
        public Action<object> _objectChangeEvent;
        #endregion

        Control control;

        public TransformControls(Control control, Camera camera) : base()
        {
            _gizmo = new TransformControlsGizmo(this);
            _plane = new TransformControlsPlane(this);
            this.Add(_gizmo);
            this.Add(_plane);
            this.camera = camera;

            this.control = control;

            control.MouseMove += OnPointerHover;
            control.MouseDown += OnPointerDown;
            control.MouseUp += OnPointerUp;
        }

        public override void Dispose()
        {
            base.Dispose();

            control.MouseDown += OnPointerDown;
            control.MouseMove += OnPointerHover;
            control.MouseUp += OnPointerUp;

            this.Traverse(node =>
            {
                if (node.Geometry != null) node.Geometry.Dispose();
                if (node.Material != null) node.Material.Dispose();
            });
        }


        private void OnPointerHover(object sender, MouseEventArgs e)
        {
            //var pointer = GetPointer(sender,e);
            PointerHover(sender, e);
        }
        private void PointerHover(object sender, MouseEventArgs e)
        {

            if (this.object3D == null || this.dragging == true) return;
            //if (this.dragging == true) return;
            //camera.LookAt(this.Parent.Position);
            //camera.UpdateMatrixWorld();

            mouse.X = e.X * 1.0f / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ((sender as Control).Height) * 2 + 1.0f;

            _raycaster.SetFromCamera(mouse, camera);

            var intersect = IntersectObjectWithRay(this._gizmo.picker[this.mode] as Object3D, _raycaster);

            if (intersect != null)
            {

                this.axis = intersect.object3D.Name;
                //Debug.WriteLine("axis:" + axis);

            }
            else
            {

                this.axis = null;

            }
        }
        private void OnPointerUp(object sender, MouseEventArgs e)
        {
            control.MouseMove -= OnPointerMove;

            mouse.X = e.X * 1.0f / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ((sender as Control).Height) * 2 + 1.0f;

            PointerUp(mouse, e.Button);

        }
        private void PointerUp(Vector2 pointer, MouseButtons button)
        {
            if (button == MouseButtons.None) return;

            if (this.dragging && (this.axis != null))
            {

                //_mouseUpEvent.mode = this.mode;
                //this.dispatchEvent(_mouseUpEvent);
                if (_mouseUpEvent != null)
                    _mouseUpEvent(this.mode);

            }

            this.dragging = false;
            this.axis = null;
        }
        private void OnPointerDown(object sender, MouseEventArgs e)
        {
            control.MouseMove += OnPointerMove;

            PointerHover(sender, e);

            mouse.X = e.X * 1.0f / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ((sender as Control).Height) * 2 + 1.0f;

            if (this.object3D == null || this.dragging == true || e.Button == MouseButtons.None) return;

            if (this.axis != null)
            {

                _raycaster.SetFromCamera(mouse, this.camera);

                var planeIntersect = IntersectObjectWithRay(this._plane, _raycaster, true);

                if (planeIntersect != null)
                {

                    this.object3D.UpdateMatrixWorld();
                    this.object3D.Parent.UpdateMatrixWorld();

                    this._positionStart.Copy(this.object3D.Position);
                    this._quaternionStart.Copy(this.object3D.Quaternion);
                    this._scaleStart.Copy(this.object3D.Scale);

                    this.object3D.MatrixWorld.Decompose(this.worldPositionStart, this.worldQuaternionStart, this._worldScaleStart);

                    this.pointStart.Copy(planeIntersect.point).Sub(this.worldPositionStart);

                }

                this.dragging = true;
                //_mouseDownEvent.mode = this.mode;
                //this.dispatchEvent(_mouseDownEvent);
                if (_mouseDownEvent != null)
                    _mouseDownEvent(this.mode);

            }

        }
        private void OnPointerMove(object sender, MouseEventArgs e)
        {
            if (!this.enabled) return;

            mouse.X = e.X * 1.0f / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ((sender as Control).Height) * 2 + 1.0f;

            PointerMove(mouse, e.Button);

        }
        private void PointerMove(Vector2 pointer, MouseButtons button)
        {
            if (mode == "scale")
            {

                space = "local";

            }
            else if (axis == "E" || axis == "XYZE" || axis == "XYZ")
            {

                space = "world";

            }

            if (object3D == null || axis == null || this.dragging == false || button == MouseButtons.None) return;

            _raycaster.SetFromCamera(pointer, this.camera);

            var planeIntersect = IntersectObjectWithRay(this._plane, _raycaster, true);

            if (planeIntersect == null) return;

            this.pointEnd.Copy(planeIntersect.point).Sub(this.worldPositionStart);

            if (mode == "translate")
            {

                // Apply translate

                this._offset.Copy(this.pointEnd).Sub(this.pointStart);

                if (space == "local" && axis != "XYZ")
                {

                    this._offset.ApplyQuaternion(this._worldQuaternionInv);

                }

                if (axis.IndexOf("X") == -1) this._offset.X = 0;
                if (axis.IndexOf("Y") == -1) this._offset.Y = 0;
                if (axis.IndexOf("Z") == -1) this._offset.Z = 0;

                if (space == "local" && axis != "XYZ")
                {

                    this._offset.ApplyQuaternion(this._quaternionStart).Divide(this._parentScale);

                }
                else
                {

                    this._offset.ApplyQuaternion(this._parentQuaternionInv).Divide(this._parentScale);

                }

                object3D.Position.Copy(this._offset).Add(this._positionStart);

                // Apply translation snap

                if (this.translationSnap != 0)
                {

                    if (space == "local")
                    {

                        object3D.Position.ApplyQuaternion(_tempQuaternion.Copy(this._quaternionStart).Invert());

                        if (axis.IndexOf("X") != -1)
                        {

                            object3D.Position.X = (float)System.Math.Round(object3D.Position.X / this.translationSnap) * this.translationSnap;

                        }

                        if (axis.IndexOf("Y") != -1)
                        {

                            object3D.Position.Y = (float)System.Math.Round(object3D.Position.Y / this.translationSnap) * this.translationSnap;

                        }

                        if (axis.IndexOf("Z") != -1)
                        {

                            object3D.Position.Z = (float)System.Math.Round(object3D.Position.Z / this.translationSnap) * this.translationSnap;

                        }

                        object3D.Position.ApplyQuaternion(this._quaternionStart);

                    }

                    if (space == "world")
                    {

                        if (object3D.Parent != null)
                        {

                            object3D.Position.Add(_tempVector.SetFromMatrixPosition(object3D.Parent.MatrixWorld));

                        }

                        if (axis.IndexOf("X") != -1)
                        {

                            object3D.Position.X = (float)System.Math.Round(object3D.Position.X / this.translationSnap) * this.translationSnap;

                        }

                        if (axis.IndexOf("Y") != -1)
                        {

                            object3D.Position.Y = (float)System.Math.Round(object3D.Position.Y / this.translationSnap) * this.translationSnap;

                        }

                        if (axis.IndexOf("Z") != -1)
                        {

                            object3D.Position.Z = (float)System.Math.Round(object3D.Position.Z / this.translationSnap) * this.translationSnap;

                        }

                        if (object3D.Parent != null)
                        {

                            object3D.Position.Sub(_tempVector.SetFromMatrixPosition(object3D.Parent.MatrixWorld));

                        }

                    }

                }

            }
            else if (mode == "scale")
            {

                if (axis.IndexOf("XYZ") != -1)
                {

                    var d = this.pointEnd.Length() / this.pointStart.Length();

                    if (this.pointEnd.Dot(this.pointStart) < 0) d *= -1;

                    _tempVector2.Set(d, d, d);

                }
                else
                {

                    _tempVector.Copy(this.pointStart);
                    _tempVector2.Copy(this.pointEnd);

                    _tempVector.ApplyQuaternion(this._worldQuaternionInv);
                    _tempVector2.ApplyQuaternion(this._worldQuaternionInv);

                    _tempVector2.Divide(_tempVector);

                    if (axis.IndexOf("X") == -1)
                    {

                        _tempVector2.X = 1;

                    }

                    if (axis.IndexOf("Y") == -1)
                    {

                        _tempVector2.Y = 1;

                    }

                    if (axis.IndexOf("Z") == -1)
                    {

                        _tempVector2.Z = 1;

                    }

                }

                // Apply scale

                object3D.Scale.Copy(this._scaleStart).Multiply(_tempVector2);

                if (this.scaleSnap != 0)
                {

                    if (axis.IndexOf("X") != -1)
                    {
                        var tempScale = (float)System.Math.Round(object3D.Scale.X / this.scaleSnap);
                        object3D.Scale.X = tempScale != 0 ? tempScale : this.scaleSnap;


                    }

                    if (axis.IndexOf("Y") != -1)
                    {
                        var tempScale = (float)System.Math.Round(object3D.Scale.Y / this.scaleSnap);
                        object3D.Scale.Y = tempScale != 0 ? tempScale : this.scaleSnap;

                    }

                    if (axis.IndexOf("Z") != -1)
                    {
                        var tempScale = (float)System.Math.Round(object3D.Scale.Z / this.scaleSnap);
                        object3D.Scale.Z = tempScale != 0 ? tempScale : this.scaleSnap;

                    }

                }

            }
            else if (mode == "rotate")
            {

                this._offset.Copy(this.pointEnd).Sub(this.pointStart);

                var ROTATION_SPEED = 20 / this.worldPosition.DistanceTo(_tempVector.SetFromMatrixPosition(this.camera.MatrixWorld));

                if (axis == "E")
                {

                    this.rotationAxis.Copy(this.eye);
                    this.rotationAngle = this.pointEnd.AngleTo(this.pointStart);

                    this._startNorm.Copy(this.pointStart).Normalize();
                    this._endNorm.Copy(this.pointEnd).Normalize();

                    this.rotationAngle *= (this._endNorm.Cross(this._startNorm).Dot(this.eye) < 0 ? 1 : -1);

                }
                else if (axis == "XYZE")
                {

                    this.rotationAxis.Copy(this._offset).Cross(this.eye).Normalize();
                    this.rotationAngle = this._offset.Dot(_tempVector.Copy(this.rotationAxis).Cross(this.eye)) * ROTATION_SPEED;

                }
                else if (axis == "X" || axis == "Y" || axis == "Z")
                {

                    this.rotationAxis.Copy(_unit[axis] as Vector3);

                    _tempVector.Copy(_unit[axis] as Vector3);

                    if (space == "local")
                    {

                        _tempVector.ApplyQuaternion(this.worldQuaternion);

                    }

                    this.rotationAngle = this._offset.Dot(_tempVector.Cross(this.eye).Normalize()) * ROTATION_SPEED;

                }

                // Apply rotation snap

                if (this.rotationSnap != 0) this.rotationAngle = (float)System.Math.Round(this.rotationAngle / this.rotationSnap) * this.rotationSnap;

                // Apply rotate
                if (space == "local" && axis != "E" && axis != "XYZE")
                {

                    object3D.Quaternion.Copy(this._quaternionStart);
                    object3D.Quaternion.Multiply(_tempQuaternion.SetFromAxisAngle(this.rotationAxis, this.rotationAngle)).Normalize();

                }
                else
                {

                    this.rotationAxis.ApplyQuaternion(this._parentQuaternionInv);
                    object3D.Quaternion.Copy(_tempQuaternion.SetFromAxisAngle(this.rotationAxis, this.rotationAngle));
                    object3D.Quaternion.Multiply(this._quaternionStart).Normalize();

                }

            }

            //this.dispatchEvent(_changeEvent);
            //this.dispatchEvent(_object3DChangeEvent);
            if (_changeEvent != null)
                _changeEvent(null);

            if (_objectChangeEvent != null)
                _objectChangeEvent(null);

        }

        private Intersection IntersectObjectWithRay(Object3D object3d, Raycaster raycaster, bool includeInvisible = false)
        {

            var allIntersections = raycaster.IntersectObject(object3d, true);

            for (int i = 0; i < allIntersections.Count; i++)
            {

                if (allIntersections[i].object3D.Visible || includeInvisible)
                {

                    return allIntersections[i];

                }

            }

            return null;

        }

        public override void UpdateMatrixWorld(bool force = false)
        {
            if (this.object3D != null)
            {

                this.object3D.UpdateMatrixWorld();

                if (this.object3D.Parent == null)
                {

                    Debug.Fail("TransformControls: The attached 3D object3D must be a part of the scene graph.");

                }
                else
                {

                    this.object3D.Parent.MatrixWorld.Decompose(this._parentPosition, this._parentQuaternion, this._parentScale);

                }

                this.object3D.MatrixWorld.Decompose(this.worldPosition, this.worldQuaternion, this._worldScale);

                this._parentQuaternionInv.Copy(this._parentQuaternion).Invert();
                this._worldQuaternionInv.Copy(this.worldQuaternion).Invert();

            }
            this.camera.UpdateMatrixWorld();
            this.camera.MatrixWorld.Decompose(this.cameraPosition, this.cameraQuaternion, this._cameraScale);

            this.eye.Copy(this.cameraPosition).Sub(this.worldPosition).Normalize();

            base.UpdateMatrixWorld(force);
        }
        public override Object3D Attach(Object3D object3d)
        {
            this.object3D = object3d;
            this.Visible = true;

            return this;
        }

        public Object3D Detach()
        {
            this.object3D = null;
            this.Visible = false;
            this.axis = null;

            return this;
        }

        public void Reset()
        {
            if (!this.enabled) return;

            if (this.dragging)
            {

                this.object3D.Position.Copy(this._positionStart);
                this.object3D.Quaternion.Copy(this._quaternionStart);
                this.object3D.Scale.Copy(this._scaleStart);

                if (_changeEvent != null)
                    _changeEvent(null);

                if (_objectChangeEvent != null)
                    _objectChangeEvent(null);

                //this.dispatchEvent(_changeEvent);
                //this.dispatchEvent(_objectChangeEvent);

                this.pointStart.Copy(this.pointEnd);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class TransformControlsGizmo : Object3D
    {
        Vector3 _tempVector = new Vector3();
        Euler _tempEuler = new Euler();
        Vector3 _alignVector = new Vector3(0, 1, 0);
        Vector3 _zeroVector = new Vector3(0, 0, 0);
        Matrix4 _lookAtMatrix = new Matrix4();
        Quaternion _tempQuaternion = new Quaternion();
        Quaternion _tempQuaternion2 = new Quaternion();
        Quaternion _identityQuaternion = new Quaternion();

        Vector3 _unitX = new Vector3(1, 0, 0);
        Vector3 _unitY = new Vector3(0, 1, 0);
        Vector3 _unitZ = new Vector3(0, 0, 1);

        #region member
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

        CylinderGeometry arrowGeometry;
        BoxGeometry scaleHandleGeometry;
        BufferGeometry lineGeometry;
        CylinderGeometry lineGeometry2;

        Hashtable gizmoTranslate,
         pickerTranslate,
         helperTranslate,
         gizmoRotate,
         helperRotate,
         pickerRotate,
         gizmoScale,
         pickerScale,
         helperScale;

        public Hashtable gizmo, picker, helper;
        #endregion
        TransformControls transformControls;
        public TransformControlsGizmo(TransformControls transformControls) : base()
        {
            this.transformControls = transformControls;

            type = "TransformControlsGizmo";

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

            gizmo = new Hashtable();
            picker = new Hashtable();
            helper = new Hashtable();

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


            #region translate
            gizmoTranslate = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0,0.05f,0), matRed), new Vector3(0.5f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matRed), new Vector3( -0.5f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0,0.25f,0), matRed), new Vector3(0, 0, 0), new Euler(0, 0, -(float)System.Math.PI / 2 ) }
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matGreen), new Vector3(0, 0.5f, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matGreen), new Vector3(0, -0.5f, 0), new Euler((float)System.Math.PI, 0, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0) as Geometry, matGreen)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matBlue), new Vector3(0, 0, 0.5f),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matBlue), new Vector3(0, 0, -0.5f), new Euler(-(float)System.Math.PI/2, 0, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matBlue),null, new Euler((float)System.Math.PI / 2, 0, 0) }
                    }

                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.1f,0),matWhiteTransparent.Clone()),new Vector3(0, 0, 0)}
                    }
                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matBlueTransparent.Clone()),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matRedTransparent.Clone()),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matGreenTransparent.Clone()),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                }

            };
            pickerTranslate = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0.3f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(-0.3f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)}
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,0.3f, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,-0.3f,0), new Euler(0, 0,(float)System.Math.PI)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, 0.3f), new Euler((float)System.Math.PI / 2, 0, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, -0.3f), new Euler(-(float)System.Math.PI / 2, 0, 0)}
                    }

                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.2f,0),matInvisible)},
                    }
                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.01f),matInvisible),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f),matInvisible),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                }

            };
            helperTranslate = new Hashtable()
            {
                {
                    "START", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.01f,2),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "END", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.01f,2),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "DELTA", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateHelperGeometry(),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1,0,0),null,new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                "Y", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(),matHelper.Clone()),new Vector3(0,-1,0),new Euler(0,0,(float)System.Math.PI/2),new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                "Z", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,0,-1),new Euler(0,-(float)System.Math.PI/2,0),new Vector3(1e6f,1,1),"helper"}
                    }
                }
            };
            #endregion

            #region rotate
            gizmoRotate = new Hashtable()
            {
                {
                    "XYZE",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,1),matGray),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {
                    "X",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matRed)}
                    }
                },
                {
                    "Y",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matGreen),null,new Euler(0,0,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "Z",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matBlue),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {
                    "E",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.75f,1),matYellowTransparent),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                }

            };
            helperRotate = new Hashtable()
            {
                {
                    "AXIS" ,new List<List<object>>()
                    {
                        new List<object>{new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1e3f,0,0),null,new Vector3(1e6f,1,1),"helper" }
                    }
                }
            };
            pickerRotate = new Hashtable()
            {
                {
                    "XYZE",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new SphereGeometry(0.25f,10,8),matInvisible)}
                    }
                },
                {
                    "X",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler(0,-(float)System.Math.PI/2,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "Y",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler((float)System.Math.PI/2,0,0)}
                    }
                },
                {
                    "Z",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler(0,0,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "E",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.75f,0.1f,4,24),matInvisible)}
                    }
                },
            };
            #endregion

            #region scale
            gizmoScale = new Hashtable()
            {
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0,0.04f,0), matRed),new Vector3(0.5f,0,0),new Euler(0,0,-(float)System.Math.PI/2)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0,0.25f,0), matRed),new Vector3(0,0,0),new Euler(0,0,-(float)System.Math.PI/2)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matRed),new Vector3(-0.5f,0,0),new Euler(0,0,(float)System.Math.PI/2)},
                    }
                },
                {
                    "Y", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matGreen),new Vector3(0,0.5f,0)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matGreen)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matGreen),new Vector3(0,-0.5f,0),new Euler(0,0,(float)System.Math.PI)},
                    }
                },
                {
                    "Z", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matBlue),new Vector3(0,0,0.5f),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matBlue),new Vector3(0,0,0),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matBlue),new Vector3(0,0,-0.5f),new Euler(-(float)System.Math.PI/2,0,0)},
                    }
                },
                {
                    "XY", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0.15f,0.15f,0) }
                    }
                },
                {
                    "YZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0,0.15f,0.15f),new Euler(0,(float)System.Math.PI/2,0) }
                    }
                },
                {
                    "XZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0.15f,0,0.15f),new Euler(-(float)System.Math.PI/2,0,0) }
                    }
                },
                {
                    "XYZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.1f,0.1f,0.1f),(MeshBasicMaterial)matWhiteTransparent.Clone()) }
                    }
                },
            };

            pickerScale = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0.3f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(-0.3f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)}
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,0.3f, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,-0.3f,0), new Euler(0, 0,(float)System.Math.PI)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, 0.3f), new Euler((float)System.Math.PI / 2, 0, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, -0.3f), new Euler(-(float)System.Math.PI / 2, 0, 0)}
                    }

                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.01f),matInvisible),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f),matInvisible),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.2f),matInvisible),new Vector3(0,0,0)}
                    }
                }

            };

            helperScale = new Hashtable()
            {
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1e3f,0,0),null,new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                    "Y", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,-1e3f,0),new Euler(0,0,(float)System.Math.PI/2),new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                    "Z", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,0,-1e3f),new Euler(0,-(float)System.Math.PI/2,0),new Vector3(1e6f,1,1),"helper"}
                    }
                }
            };
            #endregion
            // Gizmo creation

            gizmo.Add("translate", SetupGizmo(gizmoTranslate));
            gizmo.Add("rotate", SetupGizmo(gizmoRotate));
            gizmo.Add("scale", SetupGizmo(gizmoScale));

            helper.Add("rotate", SetupGizmo(helperRotate));
            helper.Add("translate", SetupGizmo(helperTranslate));
            helper.Add("scale", SetupGizmo(helperScale));

            picker.Add("translate", SetupGizmo(pickerTranslate));     
            picker.Add("rotate", SetupGizmo(pickerRotate));            
            picker.Add("scale", SetupGizmo(pickerScale));

            this.Add(gizmo["translate"] as Object3D);
            this.Add(gizmo["rotate"] as Object3D);
            this.Add(gizmo["scale"] as Object3D);

            this.Add(helper["translate"] as Object3D);
            this.Add(helper["rotate"] as Object3D);
            this.Add(helper["scale"] as Object3D);

            this.Add(picker["translate"] as Object3D);          
            this.Add(picker["rotate"] as Object3D);
            this.Add(picker["scale"] as Object3D);

            // Pickers should be hidden always
            (picker["translate"] as Object3D).Visible = false;
            (picker["rotate"] as Object3D).Visible = false;
            (picker["scale"] as Object3D).Visible = false;
        }

        
        private Object3D SetupGizmo(Hashtable gizmoMap)
        {

            var gizmo = new Object3D();


            foreach (string name in gizmoMap.Keys)
            {
                List<List<object>> list = (List<List<object>>)gizmoMap[name];
                for (int i = 0; i < list.Count; i++)
                {
                    Object3D object3D;
                    if (list[i][0] is Mesh) object3D = list[i][0] as Mesh;
                    else object3D = list[i][0] as Line;
                    //var position = list[1] as Vector3;
                    //const rotation = gizmoMap[name][i][2];
                    //const scale = gizmoMap[name][i][3];
                    if (list[i].Count > 4 && list[i][4] != null)
                        object3D.Tag = list[i][4] as string;

                    // name and tag properties are essential for picking and updating logic.
                    object3D.Name = name;
                    //object.tag = tag;

                    if (list[i].Count > 1 && list[i][1] != null) // position
                    {
                        var position = list[i][1] as Vector3;
                        object3D.Position.Copy(position);

                    }

                    if (list[i].Count > 2 && list[i][2] != null) // rotation
                    {
                        var rotation = list[i][2] as Euler;
                        object3D.Rotation.Set(rotation.X, rotation.Y, rotation.Z);

                    }

                    if (list[i].Count > 3 && list[i][3] != null) // scale
                    {
                        var scale = list[i][3] as Vector3;
                        object3D.Scale.Copy(scale);

                    }

                    object3D.UpdateMatrix();

                    //var tempGeometry = object3D.Geometry.Clone() as Geometry;
                    //tempGeometry.ApplyMatrix4(object3D.Matrix);
                    object3D.Geometry.ApplyMatrix4(object3D.Matrix);// = tempGeometry;
                    object3D.RenderOrder = int.MaxValue;
                    if (object3D.Tag == null || !(object3D.Tag as string).Equals("helper"))
                    {
                        object3D.Position.Set(0, 0, 0);
                        object3D.Rotation.Set(0, 0, 0);
                        object3D.Scale.Set(1, 1, 1);
                    }
                    // save original material color 
                    object3D.Material["_color"] = object3D.Material.Color.Value;
                    object3D.Material["_opacity"] = object3D.Material.Opacity;
                    gizmo.Add(object3D);

                }

            }

            return gizmo;

        }


        public override void UpdateMatrixWorld(bool force = false)
        {
            var space = (transformControls.mode == "scale") ? "local" : transformControls.space; // scale always oriented to local rotation

            var quaternion = (space == "local") ? transformControls.worldQuaternion : _identityQuaternion;

            // Show only gizmos for current transform mode
            (this.gizmo["translate"] as Object3D).Visible = this.transformControls.mode == "translate";
            (this.gizmo["rotate"] as Object3D).Visible = this.transformControls.mode == "rotate";
            (this.gizmo["scale"] as Object3D).Visible = this.transformControls.mode == "scale";

            (this.helper["translate"] as Object3D).Visible = this.transformControls.mode == "translate";
            (this.helper["rotate"] as Object3D).Visible = this.transformControls.mode == "rotate";
            (this.helper["scale"] as Object3D).Visible = this.transformControls.mode == "scale";

            List<Object3D> handles = new List<Object3D>();
            handles = handles.Concat((this.picker[transformControls.mode] as Object3D).Children);
            handles = handles.Concat((this.gizmo[transformControls.mode] as Object3D).Children);
            handles = handles.Concat((this.helper[transformControls.mode] as Object3D).Children);




            for (int i = 0; i < handles.Count; i++)
            {
                Object3D handle = handles[i];

                float factor;
                if (transformControls.camera is OrthographicCamera)
                {
                    factor = (transformControls.camera.Top - transformControls.camera.Bottom) / transformControls.camera.Zoom;
                }
                else
                {
                    factor = transformControls.worldPosition.DistanceTo(transformControls.cameraPosition) * (float)(System.Math.Min(1.9 * System.Math.Tan(System.Math.PI * transformControls.camera.Fov / 360) / transformControls.camera.Zoom, 7));
                }
                // hide aligned to camera
                handle.Visible = true;

                if (handle.Tag == null || !(handle.Tag as string).Equals("helper"))
                {
                    handle.Rotation.Set(0, 0, 0);
                    handle.Position.Copy(transformControls.worldPosition);
                    handle.Scale.Set(1, 1, 1).MultiplyScalar(factor * transformControls.size / 4);
                }


                // TODO: simplify helpers and consider decoupling from gizmo

                if (handle.Tag != null && (handle.Tag as string) == "helper")
                {

                    handle.Visible = false;

                    if (handle.Name == "AXIS")
                    {

                        handle.Position.Copy(transformControls.worldPositionStart);
                        handle.Visible = transformControls.axis != null;

                        if (transformControls.axis == "X")
                        {

                            _tempQuaternion.SetFromEuler(_tempEuler.Set(0, 0, 0));
                            handle.Quaternion.Copy(quaternion).Multiply(_tempQuaternion);

                            if (System.Math.Abs(_alignVector.Copy(_unitX).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > 0.9f)
                            {

                                handle.Visible = false;

                            }

                        }

                        if (transformControls.axis == "Y")
                        {

                            _tempQuaternion.SetFromEuler(_tempEuler.Set(0, 0, (float)System.Math.PI / 2));
                            handle.Quaternion.Copy(quaternion).Multiply(_tempQuaternion);

                            if (System.Math.Abs(_alignVector.Copy(_unitY).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > 0.9f)
                            {

                                handle.Visible = false;

                            }

                        }

                        if (transformControls.axis == "Z")
                        {

                            _tempQuaternion.SetFromEuler(_tempEuler.Set(0, (float)System.Math.PI / 2, 0));
                            handle.Quaternion.Copy(quaternion).Multiply(_tempQuaternion);

                            if (System.Math.Abs(_alignVector.Copy(_unitZ).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > 0.9f)
                            {

                                handle.Visible = false;

                            }

                        }

                        if (transformControls.axis == "XYZE")
                        {

                            _tempQuaternion.SetFromEuler(_tempEuler.Set(0, (float)System.Math.PI / 2, 0));
                            _alignVector.Copy(transformControls.rotationAxis);
                            handle.Quaternion.SetFromRotationMatrix(_lookAtMatrix.LookAt(_zeroVector, _alignVector, _unitY));
                            handle.Quaternion.Multiply(_tempQuaternion);
                            handle.Visible = transformControls.dragging;

                        }

                        if (transformControls.axis == "E")
                        {

                            handle.Visible = false;

                        }


                    }
                    else if (handle.Name == "START")
                    {

                        handle.Position.Copy(transformControls.worldPositionStart);
                        handle.Visible = transformControls.dragging;

                    }
                    else if (handle.Name == "END")
                    {

                        handle.Position.Copy(transformControls.worldPosition);
                        handle.Visible = transformControls.dragging;

                    }
                    else if (handle.Name == "DELTA")
                    {

                        handle.Position.Copy(transformControls.worldPositionStart);
                        handle.Quaternion.Copy(transformControls.worldQuaternionStart);
                        _tempVector.Set(1e-10f, 1e-10f, 1e-10f).Add(transformControls.worldPositionStart).Sub(transformControls.worldPosition).MultiplyScalar(-1);
                        Quaternion tempQuaternion = (Quaternion)transformControls.worldQuaternionStart.Clone();
                        tempQuaternion = tempQuaternion.Invert();
                        _tempVector.ApplyQuaternion(tempQuaternion);
                        if (_tempVector.X == 0) _tempVector.X = 1;
                        if (_tempVector.Y == 0) _tempVector.Y = 1;
                        if (_tempVector.Z == 0) _tempVector.Z = 1;

                        handle.Scale.Copy(_tempVector);
                        handle.Visible = transformControls.dragging;

                    }
                    else
                    {

                        handle.Quaternion.Copy(quaternion);

                        if (transformControls.dragging)
                        {

                            handle.Position.Copy(transformControls.worldPositionStart);

                        }
                        else
                        {

                            handle.Position.Copy(transformControls.worldPosition);

                        }

                        if (transformControls.axis != null)
                        {

                            handle.Visible = transformControls.axis.IndexOf(handle.Name) != -1;

                        }

                    }

                    // If updating helper, skip rest of the loop
                    continue;

                }

                handle.Quaternion.Copy(quaternion);

                if (transformControls.mode == "translate" || transformControls.mode == "scale")
                {

                    // Hide translate and scale axis facing the camera

                    float AXIS_HIDE_TRESHOLD = 0.99f;
                    float PLANE_HIDE_TRESHOLD = 0.2f;

                    if (handle.Name == "X")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitX).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > AXIS_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                    if (handle.Name == "Y")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitY).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > AXIS_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                    if (handle.Name == "Z")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitZ).ApplyQuaternion(quaternion).Dot(transformControls.eye)) > AXIS_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                    if (handle.Name == "XY")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitZ).ApplyQuaternion(Quaternion).Dot(transformControls.eye)) < PLANE_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                    if (handle.Name == "YZ")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitX).ApplyQuaternion(quaternion).Dot(transformControls.eye)) < PLANE_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                    if (handle.Name == "XZ")
                    {

                        if (System.Math.Abs(_alignVector.Copy(_unitY).ApplyQuaternion(quaternion).Dot(transformControls.eye)) < PLANE_HIDE_TRESHOLD)
                        {

                            handle.Scale.Set(1e-10f, 1e-10f, 1e-10f);
                            handle.Visible = false;

                        }

                    }

                }
                else if (transformControls.mode == "rotate")
                {

                    // Align handles to current local or world rotation

                    _tempQuaternion2.Copy(quaternion);
                    _alignVector.Copy(transformControls.eye).ApplyQuaternion(_tempQuaternion.Copy(quaternion).Invert());

                    if (handle.Name.IndexOf("E") != -1)
                    {

                        handle.Quaternion.SetFromRotationMatrix(_lookAtMatrix.LookAt(transformControls.eye, _zeroVector, _unitY));

                    }

                    if (handle.Name == "X")
                    {

                        _tempQuaternion.SetFromAxisAngle(_unitX, (float)System.Math.Atan2(-_alignVector.Y, _alignVector.Z));
                        _tempQuaternion.MultiplyQuaternions(_tempQuaternion2, _tempQuaternion);
                        handle.Quaternion.Copy(_tempQuaternion);

                    }

                    if (handle.Name == "Y")
                    {

                        _tempQuaternion.SetFromAxisAngle(_unitY, (float)System.Math.Atan2(_alignVector.X, _alignVector.Z));
                        _tempQuaternion.MultiplyQuaternions(_tempQuaternion2, _tempQuaternion);
                        handle.Quaternion.Copy(_tempQuaternion);

                    }

                    if (handle.Name == "Z")
                    {

                        _tempQuaternion.SetFromAxisAngle(_unitZ, (float)System.Math.Atan2(_alignVector.Y, _alignVector.X));
                        _tempQuaternion.MultiplyQuaternions(_tempQuaternion2, _tempQuaternion);
                        handle.Quaternion.Copy(_tempQuaternion);

                    }

                }

                // Hide disabled axes
                handle.Visible = handle.Visible && (handle.Name.IndexOf("X") == -1 || transformControls.showX);
                handle.Visible = handle.Visible && (handle.Name.IndexOf("Y") == -1 || transformControls.showY);
                handle.Visible = handle.Visible && (handle.Name.IndexOf("Z") == -1 || transformControls.showZ);
                handle.Visible = handle.Visible && (handle.Name.IndexOf("E") == -1 || (transformControls.showX && transformControls.showY && transformControls.showZ));

                // highlight selected axis
                //handle.Material["_color"] = handle.Material.Color.Value;
                //handle.Material["_opacity"] = handle.Material.Opacity;
                //handle.Material.Color = (Color)handle.Material["_color"];
                //handle.Material.Opacity = (float)handle.Material["_opacity"] ;

                // if selected
                if (transformControls.enabled && transformControls.axis != null)
                {

                    if (handle.Name == transformControls.axis)
                    {

                        handle.Material.Color = Color.Hex(0xffff00);
                        handle.Material.Opacity = 1.0f;

                    }
                    //else if (transformControls.axis.split("").some(function(a) {

                    //    return handle.name === a;

                    //} ) ) {
                    else
                    {
                        bool findIt = false;
                        for (int l = 0; l < transformControls.axis.Length; l++)
                        {
                            if (handle.Name == Char.ToString(transformControls.axis[l]))
                            {
                                findIt = true;
                                break;
                            }
                        }
                        if (findIt)
                        {
                            handle.Material.Color = Color.Hex(0xffff00);
                            handle.Material.Opacity = 1.0f;
                        }

                    }

                }
                else
                {
                    // revoke original color and opacity
                    handle.Material.Color = (Color)handle.Material["_color"];
                    handle.Material.Opacity = (float)handle.Material["_opacity"];
                }

            }

            base.UpdateMatrixWorld(force);
        }

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
            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { -1, 0, 0, 1, 0, 0 }, 3));

            return geometry;
        }
    }
    public class TransformControlsPlane : Mesh
    {
        TransformControls transformControls;

        Vector3 _tempVector = new Vector3();
        Vector3 _unitX = new Vector3(1, 0, 0);
        Vector3 _unitY = new Vector3(0, 1, 0);
        Vector3 _unitZ = new Vector3(0, 0, 1);

        Vector3 _v1 = new Vector3();
        Vector3 _v2 = new Vector3();
        Vector3 _v3 = new Vector3();

        Quaternion _identityQuaternion = new Quaternion();

        Vector3 _alignVector = new Vector3(0, 1, 0);

        Vector3 _dirVector = new Vector3();
        Matrix4 _tempMatrix = new Matrix4();

        public TransformControlsPlane(TransformControls controls) : base(new PlaneGeometry(100000, 100000, 2, 2),
            new MeshBasicMaterial()
            {
                Visible = false,
                Wireframe = true,
                Side = Constants.DoubleSide,
                Transparent = true,
                Opacity = 0.1f,
                ToneMapped = false
            })
        {
            type = "TransformControlsPlane";
            this.transformControls = controls;
        }

        public override void UpdateMatrixWorld(bool force = false)
        {

            string space = transformControls.space;

            this.Position.Copy(transformControls.worldPosition);

            if (transformControls.mode == "scale") space = "local"; // scale always oriented to local rotation

            _v1.Copy(_unitX).ApplyQuaternion(space == "local" ? transformControls.worldQuaternion : _identityQuaternion);
            _v2.Copy(_unitY).ApplyQuaternion(space == "local" ? transformControls.worldQuaternion : _identityQuaternion);
            _v3.Copy(_unitZ).ApplyQuaternion(space == "local" ? transformControls.worldQuaternion : _identityQuaternion);

            // Align the plane for current transform mode, axis and space.

            _alignVector.Copy(_v2);

            switch (transformControls.mode)
            {

                case "translate":
                case "scale":
                    switch (transformControls.axis)
                    {

                        case "X":
                            _alignVector.Copy(transformControls.eye).Cross(_v1);
                            _dirVector.Copy(_v1).Cross(_alignVector);
                            break;
                        case "Y":
                            _alignVector.Copy(transformControls.eye).Cross(_v2);
                            _dirVector.Copy(_v2).Cross(_alignVector);
                            break;
                        case "Z":
                            _alignVector.Copy(transformControls.eye).Cross(_v3);
                            _dirVector.Copy(_v3).Cross(_alignVector);
                            break;
                        case "XY":
                            _dirVector.Copy(_v3);
                            break;
                        case "YZ":
                            _dirVector.Copy(_v1);
                            break;
                        case "XZ":
                            _alignVector.Copy(_v3);
                            _dirVector.Copy(_v2);
                            break;
                        case "XYZ":
                        case "E":
                            _dirVector.Set(0, 0, 0);
                            break;

                    }

                    break;
                case "rotate":
                default:
                    // special case for rotate
                    _dirVector.Set(0, 0, 0);
                    break;
            }

            if (_dirVector.Length() == 0)
            {

                // If in rotate mode, make the plane parallel to camera
                this.Quaternion.Copy(transformControls.cameraQuaternion);

            }
            else
            {

                _tempMatrix.LookAt(_tempVector.Set(0, 0, 0), _dirVector, _alignVector);

                this.Quaternion.SetFromRotationMatrix(_tempMatrix);

            }

            base.UpdateMatrixWorld(force);
        }
    }
}
