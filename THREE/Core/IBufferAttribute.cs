using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Core
{
    public interface IBufferAttribute
    {
        bool NeedsUpdate { get; set; }
        
        int Buffer { get; set; }

        int Length { get; }

        int ItemSize { get; }

        Type Type { get; }
    }
}
