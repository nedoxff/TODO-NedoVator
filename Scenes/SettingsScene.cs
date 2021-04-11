using System;
using System.IO;
using NedoVator.Core;
using NedoVator.UI;
using SFML.Graphics;
using SFML.System;
using TGUI;

namespace NedoVator.Scenes
{
    public class SettingsScene : Scene
    {
        public override void Show(MainWindow window)
        {
            window.Gui.LoadWidgetsFromFile("Resources/Scenes/Settings.txt");
            window.ScrollingBackground = new ScrollingBackground(new CircleShape
            {
                Radius = 25f,
                FillColor = new Color(62, 59, 59, 100)
            }, Color.Black, new Vector2u(1280, 720), new Vector2u(20, 20), new Vector2f(20, 20));
            var arrow = (Picture) window.Gui.Get("Arrow");
            arrow.Renderer.Texture = window.Resources.Textures["arrow"];
            var waving = new Animation(window.Resources.Textures["waving"], new Vector2i(820, 585),
                new Vector2f(460, 140),
                500f) {Play = true};
            var buttonPressed = false;
            var begin = (Button) window.Gui.Get("Begin");
            begin.Clicked += (_, _) =>
            {
                Console.WriteLine("[INFO/IO] Creating config..");
                File.WriteAllText("Resources/data.json", "[]");
                buttonPressed = true;
                window.CurrentScene = new MainScene();
                window.CurrentScene.Show(window);
            };
            while (!buttonPressed && window.Window.IsOpen)
            {
                waving.Update(window.ElapsedClock.ElapsedTime.AsMilliseconds());
                window.Update();
                window.Gui.Draw();
                window.Window.Draw(waving);
                window.Window.Display();
            }
        }
    }
}