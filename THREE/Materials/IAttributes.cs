using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Renderers.gl;

namespace THREE.Materials
{
    public interface IAttributes 
    {
        GLAttributes Attributes { get; set; }
    }
}
