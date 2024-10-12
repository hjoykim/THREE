namespace SilkDemo
{
    class Program
    {
        private static void Main(string[] args)
        {
            var demoMainWindow = new SilkDemoWindow();
            demoMainWindow.Render();
            demoMainWindow.window.Dispose();

        }
    }
}