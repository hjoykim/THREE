using System.Collections;
using System.Collections.Generic;

namespace THREE
{
   
    public class TetrahedronGeometry : Geometry
    {
        public Hashtable parameters;

        public TetrahedronGeometry(float? radius=null,float? detail=null) : base()
        {
            parameters = new Hashtable()
            {
                {"radius",radius },
                { "detail",detail }
            };

            this.FromBufferGeometry(new TetrahedronBufferGeometry(radius, detail));
            this.MergeVertices();
        }
    }

    public class TetrahedronBufferGeometry : PolyhedronBufferGeometry
    {
        static List<float> vertices = new List<float>()
            {
                1, 1, 1,    - 1, - 1, 1,    - 1, 1, - 1,    1, - 1, - 1
            };

        static List<int> indices = new List<int>()
            {
               2, 1, 0,     0, 3, 2,    1, 3, 0,    2, 3, 1
            };

        public TetrahedronBufferGeometry(float? radius=null,float? detail=null) : base(vertices,indices,radius,detail)
        {         
        }
    }
}
