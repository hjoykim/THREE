using System;

namespace THREE
{
    public interface IBufferAttribute
    {
        string Name { get; set; }
        bool NeedsUpdate { get; set; }

        int Buffer { get; set; }

        int Length { get; }

        int ItemSize { get; }

        Type Type { get; }

        bool Normalized { get; set; }

        int count { get; }
        object Getter(int k, int index);
        void Setter(int k, int index,object value);
    }   
}
