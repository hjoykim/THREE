using System;
using System.Windows.Forms;
using System.Windows.Input;
using THREE;
using static THREE.Constants;

namespace THREE
{
    public enum STATE
    {
        NONE = -1,
        ROTATE = 0,
        DOLLY = 1,
        PAN = 2,
        TOUCH_ROTATE = 3,
        TOUCH_PAN = 4,
        TOUCH_DOLLY_PAN = 5,
        DOUCH_DOLLY_ROTATE = 6
    };
    public enum ControlMouseButtons
    {
        LEFT = MOUSE.ROTATE,
        MIDDLE = MOUSE.DOLLY,
        RIGHT = MOUSE.PAN
    }
    public class OrbitControls : IDisposable
    {

        public ControlMouseButtons mouseButtons;

        private Camera camera;

        private Control control;

        public bool Enabled = true;

        public Vector3 target = new Vector3();


        public float MinDistance = 0;
        public float MaxDistance = float.PositiveInfinity;

        public float MinZoom = 0;
        public float MaxZoom = float.PositiveInfinity;

        public float MinPolarAngle = 0;
        public float MaxPolarAngle = (float)System.Math.PI;

        public float MinAzimuthAngle = float.NegativeInfinity;
        public float MaxAzimuthAngle = float.PositiveInfinity;

        public bool EnableDamping = false;
        public float DampingFactor = 0.05f;

        public bool EnableZoom = true;
        public float ZoomSpeed = 1.0f;

        public bool EnableRotate = true;
        public float RotateSpeed = 1.0f;

        public bool EnablePan = true;
        public float PanSpeed = 1.0f;

        public bool ScreenSpacePanning = true;
        public float KeyPanSpeed = 7.0f;

        public bool AutoRotate = false;
        public float AutoRotateSpeed = 2.0f;

        public bool EnableKeys = true;

        private Vector3 target0;
        private Vector3 position0;
        private float zoom0;

        private STATE state = STATE.NONE;
        private float EPS = 0.000001f;
        private Spherical spherical = new Spherical();
        private Spherical sphericalDelta = new Spherical();
        private float scale = 1;
        private Vector3 panOffset = Vector3.Zero();
        private bool zoomChanged = false;

        Vector2 rotateStart = Vector2.Zero();
        Vector2 rotateEnd = Vector2.Zero();
        Vector2 rotateDelta = Vector2.Zero();

        Vector2 panStart = Vector2.Zero();
        Vector2 panEnd = Vector2.Zero();
        Vector2 panDelta = Vector2.Zero();

        Vector2 dollyStart = Vector2.Zero();
        Vector2 dollyEnd = Vector2.Zero();
        Vector2 dollyDelta = Vector2.Zero();

        public OrbitControls(Control control, Camera camera)
        {
            this.camera = camera;
            this.control = control;

            this.control.MouseDown += OnPointerDown; ;
            //this.control.MouseMove += Control_MouseMove;
            //this.control.MouseUp += Control_MouseUp;
            this.control.MouseWheel += Control_MouseWheel;
            this.control.KeyDown += Control_KeyDown;


            target0 = target.Clone();
            position0 = camera.Position.Clone();
            zoom0 = camera.Zoom;

        }
        #region Dispose      
        public event EventHandler<EventArgs> Disposed;
        public virtual void Dispose()
        {
            control.MouseDown -= OnPointerDown;
            //this.control.MouseMove -= Control_MouseMove;
            //this.control.MouseUp -= Control_MouseUp;
            this.control.MouseWheel -= Control_MouseWheel;
            this.control.KeyDown -= Control_KeyDown;

            Dispose(disposed);

        }
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private bool disposed;


        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            try
            {
                this.RaiseDisposed();
                this.disposed = true;
            }
            finally
            {
                this.disposed = true;
            }
        }
        #endregion
        public float GetAutoRotationAngle()
        {
            return 2 * (float)System.Math.PI / 60 / 60 * AutoRotateSpeed;
        }
        public float GetZoomScale()
        {

            return (float)System.Math.Pow(0.95f, ZoomSpeed);

        }

        public void RotateLeft(float angle)
        {

            sphericalDelta.Theta -= angle;

        }

        public void RotateUp(float angle)
        {

            sphericalDelta.Phi -= angle;

        }

        public void PanLeft(float distance, Matrix4 objectMatrix)
        {
            Vector3 v = Vector3.Zero();
            v.SetFromMatrixColumn(objectMatrix, 0); // get X column of objectMatrix
            v.MultiplyScalar(-distance);

            panOffset.Add(v);

        }

        public void PanUp(float distance, Matrix4 objectMatrix)
        {
            Vector3 v = Vector3.Zero();

            if (ScreenSpacePanning == true)
            {

                v.SetFromMatrixColumn(objectMatrix, 1);

            }
            else
            {

                v.SetFromMatrixColumn(objectMatrix, 0);
                v.CrossVectors(camera.Up, v);

            }

            v.MultiplyScalar(distance);

            panOffset.Add(v);
        }

        public void Pan(float deltaX, float deltaY)
        {
            Vector3 offset = Vector3.Zero();
            if (camera is PerspectiveCamera)
            {

                // perspective
                var position = camera.Position;
                offset.Copy(position).Sub(target);
                var targetDistance = offset.Length();

                // half of the fov is center to top of screen
                targetDistance *= (float)System.Math.Tan(camera.Fov / 2 * System.Math.PI / 180.0);

                // we use only clientHeight here so aspect ratio does not distort speed
                PanLeft(2 * deltaX * targetDistance / control.ClientSize.Height, camera.Matrix);
                PanUp(2 * deltaY * targetDistance / control.ClientSize.Height, camera.Matrix);

            }
            else if (camera is OrthographicCamera)
            {
                var ocamera = camera as OrthographicCamera;
                // orthographic
                PanLeft(deltaX * (ocamera.CameraRight - ocamera.Left) / camera.Zoom / control.ClientSize.Width, camera.Matrix);
                PanUp(deltaY * (ocamera.Top - ocamera.Bottom) / camera.Zoom / control.ClientSize.Height, camera.Matrix);

            }
            else
            {
                // camera neither orthographic nor perspective
                //console.warn('WARNING: OrbitControls.js encountered an unknown camera type - pan disabled.');
                EnablePan = false;
            }
        }

        public void DollyOut(float dollyScale)
        {
            if (camera is PerspectiveCamera)
            {

                scale /= dollyScale;

            }
            else if (camera is OrthographicCamera)
            {
                camera.Zoom = System.Math.Max(MinZoom, System.Math.Min(MaxZoom, camera.Zoom * dollyScale));
                camera.UpdateProjectionMatrix();
                zoomChanged = true;

            }
            else
            {

                //console.warn('WARNING: OrbitControls.js encountered an unknown camera type - dolly/zoom disabled.');
                EnableZoom = false;

            }
        }
        public void DollyIn(float dollyScale)
        {
            if (camera is PerspectiveCamera)
            {

                scale *= dollyScale;

            }
            else if (camera is OrthographicCamera)
            {

                camera.Zoom = System.Math.Max(MinZoom, System.Math.Min(MaxZoom, camera.Zoom / dollyScale));
                camera.UpdateProjectionMatrix();
                zoomChanged = true;

            }
            else
            {
                //console.warn('WARNING: OrbitControls.js encountered an unknown camera type - dolly/zoom disabled.');
                EnableZoom = false;

            }
        }

        private void handleMouseDownRotate(System.Windows.Forms.MouseEventArgs e)
        {
            rotateStart.Set(e.X, e.Y);
        }
        private void handleMouseDownDolly(System.Windows.Forms.MouseEventArgs e)
        {
            dollyStart.Set(e.X, e.Y);
        }
        private void handleMouseDownPan(System.Windows.Forms.MouseEventArgs e)
        {
            panStart.Set(e.X, e.Y);
        }
        private void handleMouseMoveRotate(System.Windows.Forms.MouseEventArgs e)
        {
            rotateEnd.Set(e.X, e.Y);
            rotateDelta.SubVectors(rotateEnd, rotateStart).MultiplyScalar(RotateSpeed);


            RotateLeft(2 * (float)System.Math.PI * rotateDelta.X / control.ClientSize.Height);
            RotateUp(2 * (float)System.Math.PI * rotateDelta.Y / control.ClientSize.Height);

            rotateStart.Copy(rotateEnd);

            Update();

        }

        private void handleMouseMoveDolly(System.Windows.Forms.MouseEventArgs e)
        {
            dollyEnd.Set(e.X, e.Y);
            dollyDelta.SubVectors(dollyEnd, dollyStart);

            if (dollyDelta.Y > 0)
            {
                DollyOut(GetZoomScale());
            }
            else if (dollyDelta.Y < 0)
            {
                DollyIn(GetZoomScale());
            }

            dollyStart.Copy(dollyEnd);

            Update();
        }

        private void handleMouseMovePan(System.Windows.Forms.MouseEventArgs e)
        {
            panEnd.Set(e.X, e.Y);
            panDelta.SubVectors(panEnd, panStart).MultiplyScalar(PanSpeed);

            Pan(panDelta.X, panDelta.Y);

            panStart.Copy(panEnd);

            Update();
        }

        private void handleMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            //
        }

        private void handleMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                DollyIn(GetZoomScale());
            }
            else if (e.Delta > 0)
            {
                DollyOut(GetZoomScale());
            }

            Update();
        }
        private void handleKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            var needsUpdate = false;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    Pan(0, KeyPanSpeed);
                    needsUpdate = true;
                    break;
                case Keys.Down:
                    Pan(0, -KeyPanSpeed);
                    needsUpdate = true;
                    break;
                case Keys.Left:
                    Pan(KeyPanSpeed, 0);
                    needsUpdate = true;
                    break;
                case Keys.Right:
                    Pan(-KeyPanSpeed, 0);
                    needsUpdate = true;
                    break;
            }

            if (needsUpdate)
            {
                Update();
            }
        }
        //private void handleTouchStartRotate()
        //private void handleTouchStartPan()
        //private void handleTouchStartDolly()
        //private void handleTouchStartDollyPen()
        //private void handleTouchStartDollyRotate()
        //private void handleTouchMoveRotate()
        //private void handleTouchMovePan();
        //private void handleTouchMoveDolly();
        //private void handleTouchMoveDollyPan();
        //private void handleTouchMoveDollyRotate();
        //private void handleTouchEnd();

        public float GetAzimuthalAngle()
        {
            return spherical.Theta;
        }

        public void SaveState()
        {
            target0.Copy(target);
            position0.Copy(camera.Position);
            zoom0 = camera.Zoom;
        }
        public void Reset()
        {
            target.Copy(target0);
            camera.Position.Copy(position0);
            camera.Zoom = zoom0;

            camera.UpdateProjectionMatrix();

            Update();

            state = STATE.NONE;
        }
        public bool Update()
        {
            var offset = new Vector3();

            var quat = new Quaternion().SetFromUnitVectors(camera.Up, new Vector3(0, 1, 0));
            var quatInverse = (quat.Clone() as Quaternion).Invert();

            var lastPosition = new Vector3();
            var lastQuaternion = new Quaternion();

            var twoPI = 2 * (float)System.Math.PI;

            var position = camera.Position;

            offset.Copy(position).Sub(target);

            // rotate offset to "y-axis-is-up" space
            offset.ApplyQuaternion(quat);

            // angle from z-axis around y-axis
            spherical.SetFromVector3(offset);

            if (AutoRotate && state == STATE.NONE)
            {

                RotateLeft(GetAutoRotationAngle());

            }

            if (EnableDamping)
            {

                spherical.Theta += sphericalDelta.Theta * DampingFactor;
                spherical.Phi += sphericalDelta.Phi * DampingFactor;

            }
            else
            {

                spherical.Theta += sphericalDelta.Theta;
                spherical.Phi += sphericalDelta.Phi;

            }

            // restrict theta to be between desired limits

            var min = MinAzimuthAngle;
            var max = MaxAzimuthAngle;

            if (!float.IsInfinity(min) && !float.IsInfinity(max))
            {

                if (min < -System.Math.PI) min += twoPI; else if (min > System.Math.PI) min -= twoPI;

                if (max < -System.Math.PI) max += twoPI; else if (max > System.Math.PI) max -= twoPI;

                if (min < max)
                {

                    spherical.Theta = System.Math.Max(min, System.Math.Min(max, spherical.Theta));

                }
                else
                {

                    spherical.Theta = spherical.Theta > (min + max) / 2 ?
                        System.Math.Max(min, spherical.Theta) :
                        System.Math.Min(max, spherical.Theta);

                }

            }

            // restrict phi to be between desired limits
            spherical.Phi = System.Math.Max(MinPolarAngle, System.Math.Min(MaxPolarAngle, spherical.Phi));

            spherical.makeSafe();


            spherical.Radius *= scale;

            // restrict radius to be between desired limits
            spherical.Radius = System.Math.Max(MinDistance, System.Math.Min(MaxDistance, spherical.Radius));

            // move target to panned location

            if (EnableDamping == true)
            {

                target.AddScaledVector(panOffset, DampingFactor);

            }
            else
            {

                target.Add(panOffset);

            }

            offset.SetFromSpherical(spherical);

            // rotate offset back to "camera-up-vector-is-up" space
            offset.ApplyQuaternion(quatInverse);

            position.Copy(target).Add(offset);

            camera.LookAt(target);

            if (EnableDamping == true)
            {

                sphericalDelta.Theta *= 1 - DampingFactor;
                sphericalDelta.Phi *= 1 - DampingFactor;

                panOffset.MultiplyScalar(1 - DampingFactor);

            }
            else
            {

                sphericalDelta.Set(0, 0, 0);

                panOffset.Set(0, 0, 0);

            }

            scale = 1;

            // update condition is:
            // min(camera displacement, camera rotation in radians)^2 > EPS
            // using small-angle approximation cos(x/2) = 1 - x^2 / 8

            if (zoomChanged ||
                lastPosition.DistanceToSquared(camera.Position) > EPS ||
                8 * (1 - lastQuaternion.Dot(camera.Quaternion)) > EPS)
            {

                lastPosition.Copy(camera.Position);
                lastQuaternion.Copy(camera.Quaternion);
                zoomChanged = false;

                return true;

            }

            return false;
        }


        private void Control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Control_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Enabled == false || EnableZoom == false || state != STATE.NONE && state != STATE.ROTATE) return;

            handleMouseWheel(e);
        }

        private void Control_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Enabled == false) return;

            handleMouseUp(e);

            control.MouseMove -= onPointerMove;
            control.MouseUp -= onPointerUp;
        }

        private void Control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Enabled == false) return;

            switch (state)
            {
                case STATE.ROTATE:
                    if (EnableRotate == false) return;

                    handleMouseMoveRotate(e);

                    break;
                case STATE.DOLLY:
                    if (EnableZoom == false) return;
                    handleMouseMoveDolly(e);
                    break;
                case STATE.PAN:
                    if (EnablePan == false) return;
                    handleMouseMovePan(e);
                    break;
            }
        }

        private void Control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Enabled==false) return;
            int mouseAction;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    mouseAction = (int)ControlMouseButtons.LEFT;
                    break;
                case MouseButtons.Middle:
                    mouseAction = (int)ControlMouseButtons.MIDDLE;
                    break;
                case MouseButtons.Right:
                    mouseAction = (int)ControlMouseButtons.RIGHT;
                    break;
                default:
                    mouseAction = -1;
                    break;
            }

            MOUSE action = (MOUSE)Enum.ToObject(typeof(MOUSE), mouseAction);
            switch (action)
            {
                case MOUSE.DOLLY:
                    if (EnableZoom == false) return;

                    handleMouseDownDolly(e);

                    state = STATE.DOLLY;

                    break;

                case MOUSE.ROTATE:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        if (EnablePan == false) return;

                        handleMouseDownPan(e);

                        state = STATE.PAN;
                    }
                    else
                    {
                        if (EnableRotate == false) return;

                        handleMouseDownRotate(e);

                        state = STATE.ROTATE;
                    }
                    break;

                case MOUSE.PAN:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        if (EnableRotate == false) return;

                        handleMouseDownRotate(e);

                        state = STATE.ROTATE;
                    }
                    else
                    {
                        if (EnablePan == false) return;

                        handleMouseDownPan(e);

                        state = STATE.PAN;
                    }
                    break;
                default:
                    state = STATE.NONE;
                    break;
            }

            if (state != STATE.NONE)
            {
                control.MouseMove += onPointerMove;
                control.MouseUp += onPointerUp;
            }
        }

        private void onPointerMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control_MouseMove(sender, e);
        }

        private void onPointerUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control_MouseUp(sender, e);
        }

        private void OnPointerDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control_MouseDown(sender, e);
        }

    }
}
