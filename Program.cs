using System.IO;
using NedoVator.Core;
using NedoVator.Scenes;

namespace NedoVator
{
    internal static class Program
    {
        private static void Main()
        {
            var window = new MainWindow();
            window.LoadResources();

            window.Resources.Music["background"].Volume = 1f;
            window.Resources.Music["background"].Loop = true;
            window.Resources.Music["background"].Play();

            if (!File.Exists("Resources/data.json"))
            {
                window.CurrentScene = new SettingsScene();
                window.CurrentScene.Show(window);
            }
            else
            {
                window.CurrentScene = new MainScene();
                window.CurrentScene.Show(window);
            }
        }
    }
}