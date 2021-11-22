using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace ColladaRender
{
    class Program
    {
        static void Main(string[] args)
        {
            int width, height;
            string title;
            if (args.Length == 4)
            {
                width = Convert.ToInt32(args[1]);
                height = Convert.ToInt32(args[2]);
                title = args[3];
            }
            else
            {
                width = 600;
                height = 600;
                title = "Testing, testing, 123!";
            }
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = title
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}