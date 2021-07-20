using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE.Math;

namespace THREE.Cameras.Controlls
{
    public class FlyControls
    {
        public Camera camera;

        public float MovementSpeed = 1.0f;

        public float RollSpeed = 0.005f;

        public bool DragToLook = false;

        public bool AutoForward = false;

        private float EPS = 0.000001f;

        private Quaternion tmpQuaternion = new Quaternion();

        private int mouseStatus = 0;

        protected Control control;

        private Quaternion lastQuaternion = new Quaternion();

        private Vector3 lastPosition = Vector3.Zero();

        public struct MoveState
        {
            public float Up;
            public float Down;
            public float Left;
            public float Right;
            public float Forward;
            public float Back;
            public float PitchUp;
            public float PitchDown;
            public float YawLeft;
            public float YawRight;
            public float RollLeft;
            public float RollRight;
        }

        private Vector3 moveVector = Vector3.Zero();

        private Vector3 rotationVector = Vector3.Zero();

        private float movementSpeedMultiplier = 0.0f;

        private MoveState moveState = new MoveState() { 
            Up = 0,
            Down = 0,
            Left = 0,
            Forward = 0,
            Back = 0,
            PitchUp = 0,
            PitchDown = 0,
            YawLeft = 0,
            YawRight = 0,
            RollLeft = 0,
            RollRight = 0
        };

        public FlyControls(Control control,Camera camera)
        {
            this.control = control;
             
            this.camera = camera;

            control.KeyDown += Control_KeyDown;

            control.KeyUp += Control_KeyUp;

            control.MouseDown += Control_MouseDown;

            control.MouseMove += Control_MouseMove;

            control.MouseUp += Control_MouseUp;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
           
            if (this.DragToLook)
            {

                this.mouseStatus--;

                this.moveState.YawLeft = this.moveState.PitchDown = 0;

            }
            else
            {

                switch ( e.Button ) {

				case MouseButtons.Left: this.moveState.Forward = 0; break;
				case MouseButtons.Right: this.moveState.Back = 0; break;

                }

			    this.UpdateMovementVector();

		    }

            this.UpdateRotationVector();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.DragToLook || this.mouseStatus > 0)
            {

                //var container = this.getContainerDimensions();
                var halfWidth = control.Width / 2;
                var halfHeight = control.Height / 2;

                this.moveState.YawLeft = -( e.X  - halfWidth ) / (float)halfWidth;
			    this.moveState.PitchDown = ( e.Y - halfHeight ) / (float)halfHeight;

			    this.UpdateRotationVector();

		    }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
           
            if (this.DragToLook)
            {

                this.mouseStatus++;

            }
            else
            {
                switch ( e.Button ) {
				    case  MouseButtons.Left: this.moveState.Forward = 1; break;
				    case  MouseButtons.Right: this.moveState.Back = 1; break;
                }

			this.UpdateMovementVector();
		    }
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey: /* shift */ this.movementSpeedMultiplier = 1.1f; break;

                case Keys.W: /*W*/ this.moveState.Forward = 0; break;
                case Keys.S: /*S*/ this.moveState.Back = 0; break;

                case Keys.A: /*A*/ this.moveState.Left = 0; break;
                case Keys.D: /*D*/ this.moveState.Right = 0; break;

                case Keys.R: /*R*/ this.moveState.Up = 0; break;
                case Keys.F: /*F*/ this.moveState.Down = 0; break;

                case Keys.Up: /*up*/ this.moveState.PitchUp = 0; break;
                case Keys.Down: /*down*/ this.moveState.PitchDown = 0; break;

                case Keys.Left: /*left*/ this.moveState.YawLeft = 0; break;
                case Keys.Right: /*right*/ this.moveState.YawRight = 0; break;

                case Keys.Q: /*Q*/ this.moveState.RollLeft = 0; break;
                case Keys.E: /*E*/ this.moveState.RollRight = 0; break;
            }

            this.UpdateMovementVector();
            this.UpdateRotationVector();
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Alt)
                return;

            switch (e.KeyCode)
            {
                case Keys.ShiftKey : /* shift */ this.movementSpeedMultiplier = 0.1f; break;

                case Keys.W : /*W*/ this.moveState.Forward = 1; break;
                case Keys.S: /*S*/ this.moveState.Back = 1; break;

                case Keys.A: /*A*/ this.moveState.Left = 1; break;
                case Keys.D: /*D*/ this.moveState.Right = 1; break;

                case Keys.R: /*R*/ this.moveState.Up = 1; break;
                case Keys.F: /*F*/ this.moveState.Down = 1; break;

                case Keys.Up: /*up*/ this.moveState.PitchUp = 1; break;
                case Keys.Down: /*down*/ this.moveState.PitchDown = 1; break;

                case Keys.Left: /*left*/ this.moveState.YawLeft = 1; break;
                case Keys.Right: /*right*/ this.moveState.YawRight = 1; break;

                case Keys.Q: /*Q*/ this.moveState.RollLeft = 1; break;
                case Keys.E: /*E*/ this.moveState.RollRight = 1; break;
            }

            this.UpdateMovementVector();
            this.UpdateRotationVector();
        }

        public void Update(float delta)
        {
            var moveMult = delta * MovementSpeed;
            var rotMult = delta * RollSpeed;

            camera.TranslateX(moveVector.X * moveMult);
            camera.TranslateY(moveVector.Y * moveMult);
            camera.TranslateZ(moveVector.Z * moveMult);

            tmpQuaternion.Set(rotationVector.X * rotMult, rotationVector.Y * rotMult, rotationVector.Z * rotMult, 1);
            tmpQuaternion.Normalize();
            camera.Quaternion.Multiply(tmpQuaternion);
            //camera.Rotation.SetFromQuaternion(camera.Quaternion, camera.Rotation.Order);
            if (
                lastPosition.DistanceToSquared(camera.Position) > EPS ||
                8 * (1 - lastQuaternion.Dot(camera.Quaternion)) > EPS
            )
            {

                //dispatchEvent(changeEvent);
                //camera.Rotation.SetFromQuaternion(camera.Quaternion, camera.Rotation.Order);
                lastQuaternion.Copy(camera.Quaternion);
                lastPosition.Copy(camera.Position);

            }

        }

        private void UpdateMovementVector()
        {
            var forward = (this.moveState.Forward!=0 || (this.AutoForward && this.moveState.Back==0)) ? 1 : 0;

            this.moveVector.X = (-this.moveState.Left + this.moveState.Right);
            this.moveVector.Y = (-this.moveState.Down + this.moveState.Up);
            this.moveVector.Z = (-forward + this.moveState.Back);
        }

        private void UpdateRotationVector()
        {
            this.rotationVector.X = (-this.moveState.PitchDown + this.moveState.PitchUp);
            this.rotationVector.Y = (-this.moveState.YawRight + this.moveState.YawLeft);
            this.rotationVector.Z = (-this.moveState.RollRight + this.moveState.RollLeft);
        }
    }
}
