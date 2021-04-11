using System;
using NedoVator.Scenes;
using NedoVator.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

namespace NedoVator.Core
{
    public class MainWindow
    {
        public Scene? CurrentScene = null;

        /// <summary>
        ///     The TGUI UI.
        /// </summary>
        public readonly Gui Gui;

        /// <summary>
        ///     The resources container which has themes, textures and sounds.
        /// </summary>
        public readonly ResourceContainer Resources;

        public ScrollingBackground? ScrollingBackground = null;

        /// <summary>
        ///     The SFML window.
        /// </summary>
        public readonly RenderWindow Window;

        public MainWindow()
        {
            Window = new RenderWindow(new VideoMode(1280, 720), "NedoVator", Styles.Close);
            Window.SetIcon(64, 64, new Image("Resources/icon.jpg").Pixels);
            Window.Closed += (_, _) => Window.Close();
            Window.LostFocus += (_, _) => Resources?.Music["background"].Pause();
            Window.GainedFocus += (_, _) => Resources?.Music["background"].Play();
            Gui = new Gui(Window) {Font = new Font("Resources/mono.ttf")};
            CountingClock = new Clock();
            ElapsedClock = new Clock();
            Resources = new ResourceContainer();
            Console.WriteLine("[INFO] A new window was created!");
        }

        private Clock CountingClock { get; }
        public Clock ElapsedClock { get; }

        public void Update()
        {
            var delta = CountingClock.Restart();
            Window.DispatchEvents();
            Window.Clear();
            ScrollingBackground?.Update(delta);
            ScrollingBackground?.Draw(Window, RenderStates.Default);
        }

        public void LoadResources()
        {
            Resources.Load();
        }
    }
}