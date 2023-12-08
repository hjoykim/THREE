using System.Collections;

namespace THREE
{

    public class LineBasicMaterial : Material
    {
        //public Color Color;

        public string LineCap ="round";

        public string LineJoin ="round";


        public LineBasicMaterial(Hashtable parameters = null)
        {
            this.Color = new Color().SetHex(0xffffff);

            this.type = "LineBasicMaterial";

            this.LineWidth = 1.0f;
            this.LineCap = "round";
            this.LineJoin = "round";

            if(parameters!=null)
                this.SetValues(parameters);
        }
        protected LineBasicMaterial(LineBasicMaterial source) : base(source)
        {
            this.Color = source.Color;

            this.LineWidth = source.LineWidth;

            this.LineCap = source.LineCap;

            this.LineJoin = source.LineJoin;
        }
       
        public new LineBasicMaterial Clone()
        {
            return new LineBasicMaterial(this);
        }
    }
}
