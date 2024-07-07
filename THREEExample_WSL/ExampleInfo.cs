/**
 * This WinForms project  and Example templates were created by referring to Three.cs( (https://github.com/lathoub/three.cs).  
 * */
using System;
using THREEExample;

namespace THREEExample
{
    public class ExampleInfo
    {
        public readonly Type Example;
        public readonly ExampleAttribute Attribute;

        public ExampleInfo(Type example, ExampleAttribute attr)
        {
            Example = example;
            Attribute = attr;
        }

        public override string ToString()
        {
            return Attribute.ToString();
        }
    }
}
