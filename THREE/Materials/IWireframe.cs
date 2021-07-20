using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Materials
{
    public interface IWireframe
    {
        bool Wireframe { get; set; }

        float WireframeLineWidth { get; set; }

        string WireframeLineCap { get; set; }

        string WireframeLineJoin { get; set; }
    }
}
