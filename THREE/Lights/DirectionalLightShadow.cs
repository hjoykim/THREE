using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Cameras;

namespace THREE.Lights
{
    public class DirectionalLightShadow : LightShadow
    {
        public DirectionalLightShadow() : base(new OrthographicCamera(-5,5,5,-5,0.5f,500))
        {

        }
    }
}
