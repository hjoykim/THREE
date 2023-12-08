namespace THREE
{
    public class DirectionalLightShadow : LightShadow
    {
        public DirectionalLightShadow() : base(new OrthographicCamera(-5,5,5,-5,0.5f,500))
        {

        }
    }
}
