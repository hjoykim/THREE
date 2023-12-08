using System.Collections.Generic;

namespace THREE
{
    public class GLBufferAttribute : Dictionary<object,object>
    {
        public int Version = 0;
        public int Buffer
        {
            get
            {
                return (int)this["buffer"];
            }
            set
            {
                this["buffer"] = value;
            }
        }
        
        public int Type
        {
            get
            {
                return (int)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        public int ItemSize
        {
            get
            {
                return (int)this["itemSize"];
            }
            set
            {
                this["itemSize"] = value;
            }
        }
        public bool NeedsUpdate
        {
            get
            {
                return (bool)this["needsUpdate"];
            }
            set
            {
                this.Version++;
                this["needsUpdate"] = value;
            }
        }

        public int ElementSize
        {
            get
            {
                return (int)this["elementSize"];
            }
            set
            {
                this["elementSize"] = value;
            }
        }

        public int count
        {
            get
            {
                return (int)this["count"];
            }
            set
            {
                this["count"] = value;
            }
        }

        bool isGLBufferAttribute = true;


        public GLBufferAttribute(int buffer,int type,int itemSize,int elementSize,int count)
        {
            this.Buffer = buffer;
            this.Type = type;
            this.ItemSize = itemSize;
            this.ElementSize = elementSize;
            this.count = count;

        }
    }
}
