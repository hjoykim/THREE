using System;

namespace THREE
{
    public class Layers
    {
        public int Mask =1 | 0;

        public Layers()
        {
        }

        public void Set(int channel)
        {
            this.Mask = 1 << channel | 0;
        }

        public void Enable(int channel)
        {
            this.Mask |= 1 << channel | 0;
        }

        public void EnableAll()
        {
            this.Mask = Convert.ToInt32(0xffffffff | 0);
        }

        public void Toggle(int channel)
        {
            this.Mask ^= 1 << channel | 0;
        }

        public void Disable(int channel)
        {
            this.Mask &= ~(1 << channel | 0);
        }

        public void DisableAll()
        {
            this.Mask = 0;
        }
        public bool Test(Layers layers)
        {
            return (this.Mask & layers.Mask) != 0;
        }

    }
}
