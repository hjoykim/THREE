using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Core
{
    public struct InstancedGroups
    {
        public int Start;

        public int Count;

        public int Instances;
    }
    public class InstancedBufferGeometry : BufferGeometry
    {
        public new List<InstancedGroups> Groups = new List<InstancedGroups>();

        public int? MaxInstanceCount;

        public InstancedBufferGeometry() : base()
        {

        }
        public override void AddGroup(int start,int count,int instances)
        {
            this.Groups.Add(new InstancedGroups { Start = start, Count = count, Instances = instances });
        }
    }
}
