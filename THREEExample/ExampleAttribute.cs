using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREEExample
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExampleAttribute : System.Attribute
    {
        public string Title { get; internal set; }

        public readonly ExampleCategory Category;

        public readonly string Subcategory;

        public string Documentation;

        public string CommonControlName = null;
        public ExampleAttribute(string title, ExampleCategory category, string subcategory,string commonUIControlName=null)
        {
            this.Title = title;
            this.Category = category;
            this.Subcategory = subcategory;
            this.CommonControlName = commonUIControlName;
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}", Category, Title);
        }
    }
    public enum ExampleCategory
    {
        OpenTK=0,
        LearnThreeJS,
        Misc
    }
}
