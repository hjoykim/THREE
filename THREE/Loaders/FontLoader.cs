namespace THREE
{
    public class FontLoader
    {
        public FontLoader()
        {

        }
        public static Font Load(string jsonFile)
        {
            return new Font(jsonFile);

        }
    }
}
