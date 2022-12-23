namespace THREE
{
    public class AmbientLight : Light
    {
        public AmbientLight(Color color, float? intensity = null)
            : base(color, intensity)
        {
            this.type = "AmbientLight";
        }
        public AmbientLight(int color, float? intensity = null) : this(Color.Hex(color), intensity) { }

    }
}