using System.Collections.Generic;

namespace THREE
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

        public int InstanceCount = int.MaxValue;

        public InstancedBufferGeometry() : base()
        {

        }
        protected InstancedBufferGeometry(InstancedBufferGeometry source)
        {
            Copy(source);            
        }
        public new InstancedBufferGeometry Clone()
        {
            return new InstancedBufferGeometry(this);
        }
        public InstancedBufferGeometry Copy(InstancedBufferGeometry source)
        {
            this.Groups = new List<InstancedGroups>(source.Groups);
            this.MaxInstanceCount = source.MaxInstanceCount;
            this.InstanceCount = source.InstanceCount;

            return this;
        }
        public override void AddGroup(int start,int count,int instances)
        {
            this.Groups.Add(new InstancedGroups { Start = start, Count = count, Instances = instances });
        }
    }
}
