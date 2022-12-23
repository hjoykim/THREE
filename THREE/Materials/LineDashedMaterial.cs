
namespace THREE
{
    public class LineDashedMaterial : LineBasicMaterial
    {
        public float Scale = 1;
        
        public float DashSize = 3;
        
        public float GapSize = 1;

        public LineDashedMaterial() : base()
        {
            this.type = "LineDashedMaterial";
        }
    }
}
