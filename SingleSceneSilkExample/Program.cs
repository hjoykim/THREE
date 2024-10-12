using THREE.Silk.Example;
namespace THREE.Silk
{
    class Program
    {
        private static void Main(string[] args)
        {
            var window = new ThreeSilkWindow();
            window.SetCurrentExample(new FirstSceneExample());
            window.Render();
        }
    }
}