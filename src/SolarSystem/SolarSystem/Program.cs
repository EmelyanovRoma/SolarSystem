namespace SolarSystem
{
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;

    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1920, 1080),
                Title = "Solar System",
                Flags = ContextFlags.ForwardCompatible,
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