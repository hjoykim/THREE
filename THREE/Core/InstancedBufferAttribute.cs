using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Core
{
    public class InstancedBufferAttribute<T> : BufferAttribute<T>
    {
        public int MeshPerAttribute
        {
            get
            {
                return (int)this["meshPerAttribute"];
            }
            set
            {
                this["meshPerAttribute"] = value;
            }
        }

        public InstancedBufferAttribute()
        {
            this.Add("meshPerAttribute", 1);
        }

        public InstancedBufferAttribute(T[] array, int itemSize, bool? normalized, int? meshPerAttribute)
            :base(array,itemSize,normalized)
        {
            
        }
    }
}
