using System;
using System.Collections.Generic;
using System.IO;
using SFML.Audio;
using SFML.Graphics;

namespace NedoVator.Core
{
    public class ResourceContainer
    {
        public readonly Dictionary<string, Music> Music = new();
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly Dictionary<string, SoundBuffer> Sounds = new();
        public readonly Dictionary<string, Texture> Textures = new();

        public void Load()
        {
            Console.WriteLine("[INFO] Loading resources..");
            LoadTextures();
            LoadSounds();
            LoadMusic();
            Console.WriteLine("[SUCCESS] Done!");
        }

        private void LoadTextures()
        {
            Console.WriteLine("[INFO/RESOURCES] Loading textures..");
            foreach (var file in Directory.EnumerateFiles("Resources/Textures"))
                Textures.Add(Path.GetFileNameWithoutExtension(file), new Texture(file));
        }

        private void LoadMusic()
        {
            Console.WriteLine("[INFO/RESOURCES] Loading music..");
            foreach (var file in Directory.EnumerateFiles("Resources/Music"))
                Music.Add(Path.GetFileNameWithoutExtension(file), new Music(file));
        }

        private void LoadSounds()
        {
            Console.WriteLine("[INFO/RESOURCES] Loading sounds..");
            foreach (var file in Directory.EnumerateFiles("Resources/Sounds"))
                Sounds.Add(Path.GetFileNameWithoutExtension(file), new SoundBuffer(file));
        }

        public Sound SoundFromName(string name)
        {
            return new(Sounds[name]);
        }
    }
}