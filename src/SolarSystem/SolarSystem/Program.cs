namespace SolarSystemTest
{
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using System.Windows.Forms;

    public static class Program
    {
        private static void Main()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1024, 1024),
                Title = "Solar System",
                Flags = ContextFlags.ForwardCompatible,
                Location = new Vector2i((screenWidth / 2) - 512, 0)
            };

            using (var game = new Game(
                GameWindowSettings.Default,
                nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}