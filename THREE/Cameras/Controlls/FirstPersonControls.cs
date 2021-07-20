using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREE.Math;
using THREE.Renderers;
namespace THREE.Cameras.Controlls
{
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

        public FirstPersonControls(Control control, Camera camera)
        {
            this.camera = camera;
            this.screen = control.ClientRectangle;

            control.MouseDown += Control_MouseDown;

            control.MouseMove += Control_MouseMove;

            control.MouseUp += Control_MouseUp;

            //control.MouseWheel += Control_MouseWheel;

            control.SizeChanged += Control_SizeChanged;

            control.KeyDown += Control_KeyDown;

            control.KeyUp += Control_KeyUp;
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up :
                case Keys.W :
                    moveForward = true;
                    break;
                case Keys.Left :
                case Keys.A :
                    moveLeft = true;
                    break;
                case Keys.Down :
                case Keys.S :
                    moveBackward = true;
                    break;
                case Keys.Right :
                case Keys.D :
                    moveRight = true;
                    break;
                case Keys.R :
                    moveUp = true;
                    break;
                case Keys.F :
                    moveDown = true;
                    break;
                
            }
            e.Handled = true;
        }
        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    moveForward = false;
                    break;
                case Keys.Left:
                case Keys.A:
                    moveLeft = false;
                    break;
                case Keys.Down:
                case Keys.S:
                    moveBackward = false;
                    break;
                case Keys.Right:
                case Keys.D:
                    moveRight = false;
                    break;
                case Keys.R:
                    moveUp = false;
                    break;
                case Keys.F:
                    moveDown = false;
                    break;

            }
            e.Handled = true;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseX = e.X - this.viewHalfX;
            this.mouseY = e.Y - this.viewHalfY;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.ActiveLook)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        this.moveForward = true;
                        break;
                    case MouseButtons.Right:
                        this.moveBackward = true;
                        break;
                }
            }

            this.mouseDragOn = false;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.ActiveLook)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left :
                        this.moveForward = true;
                        break;
                    case MouseButtons.Right :
                        this.moveBackward = true;
                        break;
                }
            }
            this.mouseDragOn = true;
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            this.screen = (sender as Control).ClientRectangle;
            this.screen = (sender as Control).ClientRectangle;
            this.camera.Aspect = (sender as OpenTK.GLControl).AspectRatio;
            this.camera.UpdateProjectionMatrix();

            this.viewHalfX = screen.Width / 2;
            this.viewHalfY = screen.Height / 2;
        }

        public void Update(float delta)
        {
            if (this.Enabled == false) return;

            if (this.HeightSpeed)
            {
                float y = this.camera.Position.Y.Clamp(this.HeightMin, this.HeightMax);
                float heightDelta = y - this.HeightMin;
                this.AutoSpeedFactor = delta * (heightDelta * this.HeightCoef);
            }
            else
            {
                this.AutoSpeedFactor = 0.0f;
            }

            var actualMoveSpeed = delta * this.MovementSpeed;
            
            if(this.moveForward || this.AutoForward && !this.moveBackward)
                this.camera.Position.Z = this.camera.Position.Z - (actualMoveSpeed + this.AutoSpeedFactor);
            
            if(this.moveBackward)
                this.camera.Position.Z = this.camera.Position.Z + actualMoveSpeed;

            if(this.moveLeft)
                this.camera.Position.X = this.camera.Position.X - actualMoveSpeed;

            if(this.moveRight)
                this.camera.Position.X = this.camera.Position.X + actualMoveSpeed;

            if(this.moveUp)
                this.camera.Position.Y = this.camera.Position.Y + actualMoveSpeed;

            if(this.moveDown)
                this.camera.Position.Y = this.camera.Position.Y - actualMoveSpeed;


            var actualLookSpeed = delta * this.LookSpeed;

            if (!this.ActiveLook)
            {
                actualMoveSpeed = 0;
            }

            float verticalLookRatio = 1;

            if (this.ConstrainVertical)
            {
                verticalLookRatio = (float)System.Math.PI / (this.VerticalMax - this.VerticalMin);
            }

            this.lon += this.mouseX * actualLookSpeed;
            if (this.LookVertical)
                this.lat -= this.mouseY * actualLookSpeed * verticalLookRatio;

            this.lat = System.Math.Max(-85, System.Math.Min(85, this.lat));
            this.phi = THREE.Math.TMath.DegToRad(90 - this.lat);
            this.theta = THREE.Math.TMath.DegToRad(this.lon);

            if (this.ConstrainVertical)
            {
                this.phi = THREE.Math.TMath.mapLinear(this.phi, 0,System.Math.PI, this.VerticalMin, this.VerticalMax);
            }

            var targetPosition = this.target;
            var position = this.camera.Position;

            targetPosition.X = position.X + (float)(100 * System.Math.Sin(this.phi) * System.Math.Cos(this.theta));
            targetPosition.Y = position.Y + (float)(100 * System.Math.Cos(this.phi));
            targetPosition.Z = position.Z + (float)(100 * System.Math.Sin(this.phi) * System.Math.Sin(this.theta));

            this.camera.LookAt(targetPosition);
        }
    }
}
