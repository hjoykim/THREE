using OpenTK;
using System;
using System.Drawing;
using System.Windows.Forms;

using Quaternion = THREE.Quaternion;
using Vector2 = THREE.Vector2;
using Vector3 = THREE.Vector3;

namespace THREE
{
    public class TrackballControls
    {
        public Camera camera;
        public enum STATE : int
        {
            NONE = -1,
            ROTATE = 0,
            ZOOM = 1,
            PAN = 2,
            TOUCH_ROTATE = 3,
            TOUCH_ZOOM_PAN = 4
        }
        public bool Enabled = true;
        public float RotateSpeed = 1.0f;
        public float ZoomSpeed = 1.2f;
        public float PanSpeed = 0.3f;

        public bool NoRotate = false;
        public bool NoZoom = false;
        public bool NoPan = false;
        public bool NoRoll = false;

        public bool StaticMoving = false;
        public float DynamicDampingFactor = 0.2f;

        public float MinDistance = 0;
        public float MaxDistance = float.PositiveInfinity;

        // internal variables
        private Vector3 target = Vector3.Zero();

        private Vector3 lastPosition = Vector3.Zero();

        public STATE state = STATE.NONE;

        private Vector3 eye = Vector3.Zero();

        private Vector3 rotateStart = Vector3.Zero();

        private Vector3 rotateEnd = Vector3.Zero();

        private Vector2 zoomStart = Vector2.Zero();

        private Vector2 zoomEnd = Vector2.Zero();

        private float touchZoomDistanceStart = 0;

        private float touchZoomDistanceEnd = 0;

        private Vector2 panStart = Vector2.Zero();

        private Vector2 panEnd = Vector2.Zero();

        private Rectangle screen;

        private Vector3 target0;

        private Vector3 position0;

        private Vector3 up0;


        public TrackballControls(Control control, Camera camera)
        {
            this.camera = camera;

            screen = control.ClientRectangle;

            target0 = target;

            position0 = this.camera.Position;

            up0 = this.camera.Up;

            control.MouseDown += Control_MouseDown;

            control.MouseMove += Control_MouseMove;

            control.MouseUp += Control_MouseUp;

            control.MouseWheel += Control_MouseWheel;

            control.SizeChanged += Control_SizeChanged;

        }

        public Vector2 GetMouseOnScreen(int pageX, int pageY)
        {
            var vector = new Vector2(
                (pageX - screen.Left) / (float)screen.Width,
                (pageY - screen.Top) / (float)screen.Height);

            return vector;
        }
        public Vector3 GetMouseProjectionOnBall(int pageX, int pageY)
        {
            Vector3 mouseOnBall = new Vector3(
                (pageX - screen.Width * 0.5f - screen.Left) / (screen.Width * 0.5f),
                (screen.Height * 0.5f + screen.Top - pageY) / (screen.Height * 0.5f),
                0.0f
                );

            var length = mouseOnBall.Length();
            if (NoRoll)
            {
                if (length < MathUtils.SQRT1_2)
                    mouseOnBall.Z = (float)System.Math.Sqrt(1.0 - length * length);
                else
                    mouseOnBall.Z = 0.5f / length;
            }
            else if (length > 1.0)
                mouseOnBall.Normalize();
            else
                mouseOnBall.Z = (float)System.Math.Sqrt(1.0 - length * length);

            Vector3 camPos = camera.Position;
            eye = camPos - target;
            Vector3 upClone = Vector3.Zero().Copy(camera.Up);
            Vector3 projection;
            upClone.Normalize();

            projection = upClone.MultiplyScalar(mouseOnBall.Y);

            Vector3 cross = Vector3.Zero().Copy(camera.Up).Cross(eye);
            cross.Normalize();
            cross.MultiplyScalar(mouseOnBall.X);
            projection = projection.Add(cross);

            //  projection.add(_eye.normalize().scale(mouseOnBall.z));
            Vector3 eyeClone = Vector3.Zero().Copy(eye);
            eyeClone.Normalize();
            projection.Add(eyeClone.MultiplyScalar(mouseOnBall.Z));

            return projection;

        }
        void RotateCamera()
        {

            var axis = new Vector3();
            var quaternion = new Quaternion();

            var angle = (float)System.Math.Acos(rotateStart.Dot(rotateEnd) / rotateStart.Length() / rotateEnd.Length());

            if (angle > 0)
            {
                axis.CrossVectors(rotateStart, rotateEnd).Normalize();

                angle *= RotateSpeed;

                quaternion.SetFromAxisAngle(axis, -angle);

                eye.ApplyQuaternion(quaternion);
                camera.Up.ApplyQuaternion(quaternion);

                rotateEnd.ApplyQuaternion(quaternion);

                if (StaticMoving)
                {

                    rotateStart.Copy(rotateEnd);

                }
                else
                {

                    quaternion.SetFromAxisAngle(axis, angle * (DynamicDampingFactor - 1.0f));
                    rotateStart.ApplyQuaternion(quaternion);

                }

            }
        }
        private void ZoomCamera()
        {
            if (state == STATE.TOUCH_ZOOM_PAN)
            {
                var factor = touchZoomDistanceStart / touchZoomDistanceEnd;
                touchZoomDistanceStart = touchZoomDistanceEnd;
                eye = eye * factor;
            }
            else
            {
                var factor = (float)(1.0 + (zoomEnd.Y - zoomStart.Y) * ZoomSpeed);
                if (factor != 1.0 && factor > 0.0f)
                {
                    eye.MultiplyScalar(factor);
                }
                if (StaticMoving)
                {
                    zoomStart = new Vector2(zoomEnd.X, zoomEnd.Y);
                }
                else
                {
                    zoomStart.Y += (zoomEnd.Y - zoomStart.Y) * DynamicDampingFactor;
                }

            }
        }
        private void PanCamera()
        {
            var mouseChange = new Vector2();
            var objectUp = new Vector3();
            var pan = new Vector3();

            mouseChange.Copy(panEnd).Sub(panStart);

            if (mouseChange.LengthSq() > 0)
            {
                mouseChange.MultiplyScalar(eye.Length() * PanSpeed);
                pan.Copy(eye).Cross(camera.Up).SetLength(mouseChange.X);
                pan.Add(objectUp.Copy(camera.Up).SetLength(mouseChange.Y));

                camera.Position.Add(pan);
                target.Add(pan);

                /*    
                pan.Normalize();
                pan = Vector3.Multiply(pan, mouseChange.X);

                Vector3 upClone = new Vector3(this.camera.Up);
                upClone.Normalize();
                upClone = Vector3.Multiply(upClone, mouseChange.Y);
                pan += upClone;

                this.camera.Position = Vector3.Add(this.camera.Position, pan);  
                this.target = Vector3.Add(target, pan);
                */
                if (StaticMoving)
                {
                    panStart.Copy(panEnd);
                }
                else
                {
                    //mouseChange = panEnd - panStart;
                    //mouseChange = Vector2.Multiply(mouseChange, DynamicDampingFactor);
                    //panStart += mouseChange;
                    panStart.Add(mouseChange.SubVectors(panEnd, panStart).MultiplyScalar(DynamicDampingFactor));
                }

            }
        }
        private void CheckDistances()
        {
            if (!NoZoom || !NoPan)
            {

                if (eye.LengthSq() > MaxDistance * MaxDistance)
                {
                    eye.Normalize();
                    eye.MultiplyScalar(MaxDistance);

                    camera.Position = target + eye;
                }

                if (eye.LengthSq() < MinDistance * MinDistance)
                {
                    eye.Normalize();

                    eye.MultiplyScalar(MinDistance);

                    camera.Position = target + eye;
                }

            }
        }
        public void Update()
        {
            if (!this.Enabled) return;
            eye.SubVectors(camera.Position, target);
            if (!NoRotate)
            {
                RotateCamera();
            }

            if (!NoZoom)
            {
                ZoomCamera();
            }

            if (!NoPan)
            {
                PanCamera();
            }

            // object.position =  target + _eye;
            camera.Position = target + eye;

            CheckDistances();

            // object.lookAt( target );
            camera.LookAt(target);



            // distanceToSquared
            if ((lastPosition - camera.Position).LengthSq() > 0.0f)
            {
                //
                //   dispatchEvent( changeEvent );

                lastPosition.Copy(camera.Position);

            }


        }
        private void Control_SizeChanged(object sender, EventArgs e)
        {
            screen = (sender as Control).ClientRectangle;
            camera.Aspect = (sender as GLControl).AspectRatio;
            camera.UpdateProjectionMatrix();

        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Enabled) { return; }

            var delta = e.Delta / 40;

            zoomStart.Y += delta * 0.01f;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Enabled) return;
            state = STATE.NONE;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if ((sender as GLControl).Focused == false)
                (sender as GLControl).Focus();

            if (!Enabled) return;

            if (state == STATE.ROTATE && !NoRotate)
            {

                rotateEnd = GetMouseProjectionOnBall(e.X, e.Y);

            }
            else if (state == STATE.ZOOM && !NoZoom)
            {

                zoomEnd = GetMouseOnScreen(e.X, e.Y);

            }
            else if (state == STATE.PAN && !NoPan)
            {

                panEnd = GetMouseOnScreen(e.X, e.Y);

            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Enabled) return;


            if (state == STATE.NONE)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        state = STATE.ROTATE;
                        break;
                    case MouseButtons.Middle:
                        state = STATE.ZOOM;
                        break;
                    case MouseButtons.Right:
                        state = STATE.PAN;
                        break;
                }
            }

            if (state == STATE.ROTATE && !NoRotate)
            {

                rotateStart = GetMouseProjectionOnBall(e.X, e.Y);
                rotateEnd = rotateStart;

            }
            else if (state == STATE.ZOOM && !NoZoom)
            {

                zoomStart = GetMouseOnScreen(e.X, e.Y);
                zoomEnd = zoomStart;


            }
            else if (state == STATE.PAN && !NoPan)
            {

                panStart = GetMouseOnScreen(e.X, e.Y);
                panEnd = panStart;

            }
        }
    }
}
