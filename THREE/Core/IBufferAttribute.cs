using System;

namespace THREE
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
