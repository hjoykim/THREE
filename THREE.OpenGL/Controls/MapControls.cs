using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using static THREE.Constants;

namespace THREE
{
    [Serializable]
    public class MapControls : OrbitControls
    {
        public MapControls(IControlsContainer control, Camera camera) : base(control, camera)
        {
            this.ControlMouseButtons = new Dictionary<MouseButton, MOUSE>
            {
                {MouseButton.Left,MOUSE.PAN},
                {MouseButton.Middle,MOUSE.DOLLY},
                {MouseButton.Right,MOUSE.ROTATE}
            };

            this.ScreenSpacePanning = false;
        }

    }
}
