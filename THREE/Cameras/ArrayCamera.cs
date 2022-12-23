using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    public class ArrayCamera : PerspectiveCamera
    {
        public List<Camera> Cameras = new List<Camera>();


        public ArrayCamera() : base()
        {

        }
        public ArrayCamera(List<Camera> array)
            : base()
        {
            this.Cameras = array;
        }        
    }
}
