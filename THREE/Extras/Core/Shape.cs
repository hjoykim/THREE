using System;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class Shape : Path
    {
        public Guid Uuid = Guid.NewGuid();

        public List<Path> Holes;

        public Shape(List<Vector3> points=null) : base(points)
        {
            Holes = new List<Path>();
        }

        protected Shape(Shape source)
        {
            Holes = new List<Path>();
            for (int i = 0;i< source.Holes.Count;i++)
            {

                var hole = source.Holes[i];

                this.Holes.Add(hole.Clone() as Path);

            }

        }

        public new object Clone()
        {
            return new Shape(this);
        }
        public List<List<Vector3>> GetPointsHoles(float divisions)
        {
            var holePts = new List<List<Vector3>>();

            for(int i = 0; i < this.Holes.Count; i++)
            {
                if (this.Holes[i] == null) continue;
                holePts.Add(this.Holes[i].GetPoints(divisions));
            }

            return holePts;
        }

        public Hashtable ExtractPoints(float divisions)
        {
            return new Hashtable()
            {
                {"shape",this.GetPoints(divisions) },
                {"holes",this.GetPointsHoles(divisions) }
            };
        }
    }
}
