using System;
using System.Drawing;
using THREE;
using Silk.NET.Input;

namespace THREE.Silk
{
    [Serializable]
    public class FirstPersonControls
    {
        private Camera camera;

        public bool Enabled = true;

        public float MovementSpeed = 1.0f;

        public float LookSpeed = 0.005f;

        public bool LookVertical = true;

        public bool AutoForward = false;

        public bool ActiveLook = true;

        public bool HeightSpeed = false;

        public float HeightCoef = 1.0f;
        public float HeightMin = 0.0f;
        public float HeightMax = 1.0f;

        public bool ConstrainVertical = false;
        public float VerticalMin = 0;
        public float VerticalMax = (float)System.Math.PI;

        public float AutoSpeedFactor = 0.0f;

        private int mouseX = 0;
        private int mouseY = 0;

        public float lat = 0;
        public float lon = 0;
        private float phi = 0;
        private float theta = 0;

        private bool moveForward = false;
        private bool moveBackward = false;
        private bool moveLeft = false;
        private bool moveRight = false;
        private bool moveUp = false;
        private bool moveDown = false;
        private bool mouseDragOn = false;

        private Rectangle screen;

        private Vector3 target = Vector3.Zero();

        private int viewHalfX = 0;
        private int viewHalfY = 0;
        private IControlsContainer glControl;
        public FirstPersonControls(IControlsContainer control, Camera camera)
        {
            this.camera = camera;
            this.glControl = control;
            screen = control.ClientRectangle;

            control.MouseDown += Control_MouseDown;

            control.MouseMove += Control_MouseMove;

            control.MouseUp += Control_MouseUp;

            //control.MouseWheel += Control_MouseWheel;

            control.SizeChanged += Control_SizeChanged;

            control.KeyDown += Control_KeyDown;

            control.KeyUp += Control_KeyUp;
        }

        private void Control_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    moveForward = true;
                    break;
                case Key.Left:
                case Key.A:
                    moveLeft = true;
                    break;
                case Key.Down:
                case Key.S:
                    moveBackward = true;
                    break;
                case Key.Right:
                case Key.D:
                    moveRight = true;
                    break;
                case Key.R:
                    moveUp = true;
                    break;
                case Key.F:
                    moveDown = true;
                    break;

            }
        }
        private void Control_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    moveForward = false;
                    break;
                case Key.Left:
                case Key.A:
                    moveLeft = false;
                    break;
                case Key.Down:
                case Key.S:
                    moveBackward = false;
                    break;
                case Key.Right:
                case Key.D:
                    moveRight = false;
                    break;
                case Key.R:
                    moveUp = false;
                    break;
                case Key.F:
                    moveDown = false;
                    break;

            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X - viewHalfX;
            mouseY = e.Y - viewHalfY;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (ActiveLook)
            {
                switch (e.Button)
                {
                    case MouseButton.Left:
                        moveForward = true;
                        break;
                    case MouseButton.Right:
                        moveBackward = true;
                        break;
                }
            }

            mouseDragOn = false;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (ActiveLook)
            {
                switch (e.Button)
                {
                    case MouseButton.Left:
                        moveForward = true;
                        break;
                    case MouseButton.Right:
                        moveBackward = true;
                        break;
                }
            }
            mouseDragOn = true;
        }

        private void Control_SizeChanged(object sender, ResizeEventArgs e)
        {
            HandleResize();
        }
        public void HandleResize()
        {
            screen = glControl.ClientRectangle;
            viewHalfX = screen.Width / 2;
            viewHalfY = screen.Height / 2;
        }
        public void Update(float delta)
        {
            if (Enabled == false) return;

            if (HeightSpeed)
            {
                float y = camera.Position.Y.Clamp(HeightMin, HeightMax);
                float heightDelta = y - HeightMin;
                AutoSpeedFactor = delta * (heightDelta * HeightCoef);
            }
            else
            {
                AutoSpeedFactor = 0.0f;
            }

            var actualMoveSpeed = delta * MovementSpeed;

            if (moveForward || AutoForward && !moveBackward)
                camera.Position.Z = camera.Position.Z - (actualMoveSpeed + AutoSpeedFactor);

            if (moveBackward)
                camera.Position.Z = camera.Position.Z + actualMoveSpeed;

            if (moveLeft)
                camera.Position.X = camera.Position.X - actualMoveSpeed;

            if (moveRight)
                camera.Position.X = camera.Position.X + actualMoveSpeed;

            if (moveUp)
                camera.Position.Y = camera.Position.Y + actualMoveSpeed;

            if (moveDown)
                camera.Position.Y = camera.Position.Y - actualMoveSpeed;


            var actualLookSpeed = delta * LookSpeed;

            if (!ActiveLook)
            {
                actualMoveSpeed = 0;
            }

            float verticalLookRatio = 1;

            if (ConstrainVertical)
            {
                verticalLookRatio = (float)System.Math.PI / (VerticalMax - VerticalMin);
            }

            lon += mouseX * actualLookSpeed;
            if (LookVertical)
                lat -= mouseY * actualLookSpeed * verticalLookRatio;

            lat = System.Math.Max(-85, System.Math.Min(85, lat));
            phi = MathUtils.DegToRad(90 - lat);
            theta = MathUtils.DegToRad(lon);

            if (ConstrainVertical)
            {
                phi = MathUtils.mapLinear(phi, 0, System.Math.PI, VerticalMin, VerticalMax);
            }

            var targetPosition = target;
            var position = camera.Position;

            targetPosition.X = position.X + (float)(100 * System.Math.Sin(phi) * System.Math.Cos(theta));
            targetPosition.Y = position.Y + (float)(100 * System.Math.Cos(phi));
            targetPosition.Z = position.Z + (float)(100 * System.Math.Sin(phi) * System.Math.Sin(theta));

            camera.LookAt(targetPosition);
        }
    }
}
