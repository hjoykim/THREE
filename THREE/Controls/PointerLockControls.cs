using System;
using System.Windows.Forms;
using THREE;

namespace THREE
{
    public class PointerLockControls
    {
        private Camera camera;

        private const float YAW = -90.0f;

        private const float PITCH = 0.0f;

        private const float SPEED = 2.5f;

        private const float SENSITIVITY = 0.1f;

        private const float ZOOM = 45.0f;

        public float Yaw;

        public float Pitch;

        public float MovementSpeed;

        public float MouseSensitivity;


        public Vector3 worldUp;

        private int oldX = 0;
        private int oldY = 0;
        private bool firstMouse = true;

        public PointerLockControls(Control control, Camera camera)
        {
            this.camera = camera;

            Yaw = YAW;

            Pitch = PITCH;

            this.camera.Front = new Vector3(0.0f, 0.0f, -1.0f);

            MovementSpeed = SPEED;

            MouseSensitivity = SENSITIVITY;



            worldUp = camera.Up;

            control.MouseMove += Control_MouseMove;
            control.MouseWheel += Control_MouseWheel;
            control.SizeChanged += Control_SizeChanged;

            control.KeyDown += Control_KeyUp;


        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {

            float velocity = 2.5f * MovementSpeed;
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    camera.Position += camera.Front * velocity;
                    break;
                case Keys.Left:
                case Keys.A:
                    camera.Position -= camera.Right * velocity;
                    break;
                case Keys.Down:
                case Keys.S:
                    camera.Position -= camera.Front * velocity;
                    break;
                case Keys.Right:
                case Keys.D:
                    camera.Position += camera.Right * velocity;
                    break;
                case Keys.R:
                    camera.Position += camera.Up * velocity;
                    break;
                case Keys.F:
                    camera.Position -= camera.Up * velocity;
                    break;

            }
            updateCameraVectors();

        }


        public void Update()
        {
            //updateCameraVectors();
            camera.LookAt(camera.Position + camera.Front);
        }
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.Identity().LookAt(camera.Position, camera.Position + camera.Front, camera.Up);
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            camera.Aspect = (sender as OpenTK.GLControl).AspectRatio;
            camera.UpdateProjectionMatrix();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (firstMouse)
            {
                oldX = e.X;
                oldY = e.Y;
                firstMouse = false;
            }
            int xoffset = e.X - oldX;
            int yoffset = oldY - e.Y;

            Yaw += xoffset * MouseSensitivity;

            Pitch += yoffset * MouseSensitivity;

            if (Pitch > 89.0f)
                Pitch = 89.0f;

            if (Pitch < -89.0f)
                Pitch = -89.0f;

            updateCameraVectors();

            oldX = e.X;
            oldY = e.Y;

        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {

            int yoffset = e.Delta;

            if (camera.Fov >= 1.0f && camera.Fov <= 45.0f)
                camera.Fov -= yoffset;
            if (camera.Fov <= 1.0f)
                camera.Fov = 1.0f;
            if (camera.Fov >= 45.0f)
                camera.Fov = 45.0f;

            camera.UpdateProjectionMatrix();

        }
        private void updateCameraVectors()
        {
            Vector3 front = Vector3.Zero();

            front.X = (float)System.Math.Cos(MathUtils.DegToRad(Yaw)) * (float)System.Math.Cos(MathUtils.DegToRad(Pitch));
            front.Y = (float)System.Math.Sin(MathUtils.DegToRad(Pitch));
            front.Z = (float)System.Math.Sin(MathUtils.DegToRad(Yaw)) * (float)System.Math.Cos(MathUtils.DegToRad(Pitch));

            camera.Front = front.Normalize();
            camera.Right = Vector3.Zero().Copy(camera.Front).Cross(worldUp).Normalize();
            camera.Up = Vector3.Zero().Copy(camera.Right).Cross(camera.Front).Normalize();
        }
    }
}
