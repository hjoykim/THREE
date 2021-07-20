using System;
using THREEExample;

namespace THREEExample
{
    public class ExampleInfo
    {
        public readonly Type Demo;
        public readonly ExampleAttribute Attribute;

        public ExampleInfo(Type demo, ExampleAttribute attr)
        {
            Demo = demo;
            Attribute = attr;
        }

        public override string ToString()
        {
            return Attribute.ToString();
        }
    }
}
