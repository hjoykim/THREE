
namespace THREE
{
    public class Fog
    {
        public string Name;

        public Color Color;

        public float Near;

        public float Far;

        public Fog()
        {
        }
        public Fog(Color color, float? near=null, float? far=null)
        {
            this.Name = "";

            this.Color = color;

            this.Near = near != null ? near.Value : 1;

            this.Far = far != null ? far.Value : 1000;

        }
        public Fog(int color, float? near = null, float? far = null) : this(Color.Hex(color), near, far) { }
    }
}
