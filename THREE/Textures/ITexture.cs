using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Textures
{
    public interface ITexture
    {
        int WrapS { get; set; }

        int WrapT { get; set; }

        int MagFilter { get; set; }

        int MinFilter { get; set; }

        int Type { get; set; }

        int Anisotropy { get; set; }

        int glTexture { get; set; }

        bool NeedsUpdate { get; set; }
    }
}
