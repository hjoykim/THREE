using System.Collections;
using System.Collections.Generic;


namespace THREE
{
    public class OctahedronGeometry : Geometry
    {
        public Hashtable parameters;
        public OctahedronGeometry(float? radius = null, float? detail = null) : base() 
        {
            parameters = new Hashtable()
            {
                {"radius",radius },
                {"detail",detail }
            };

            this.FromBufferGeometry(new OctahedronBufferGeometry(radius, detail));
            this.MergeVertices();
        }
    }

    public class OctahedronBufferGeometry : PolyhedronBufferGeometry
    {
        static List<float> vertices = new List<float>()
            {
                1, 0, 0, -1, 0, 0,  0, 1, 0,
                0, -1, 0, 0, 0, 1,  0, 0, - 1
            };

        static List<int> indices = new List<int>()
            {
                0, 2, 4,    0, 4, 3,    0, 3, 5,
                0, 5, 2,    1, 2, 5,    1, 5, 3,
                1, 3, 4,    1, 4, 2
            };
        public OctahedronBufferGeometry(float? radius=null,float? detail=null) : base(vertices,indices,radius,detail) 
        {
        }
    }

}
