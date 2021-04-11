using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
// ReSharper disable CA2211

namespace NedoVator.Json
{
    public class Task
    {
        public static List<Task> Tasks = null!;
        public readonly string Description;
        public readonly string DueTo;

        public readonly string Title;

        public Task(string title, string description, string dueTo)
        {
            Title = title;
            Description = description;
            DueTo = dueTo;
        }

        public static void LoadTasks()
        {
            Console.WriteLine("[INFO/IO] Loading Resources/data.json ..");
            Tasks = new List<Task>();
            Tasks.AddRange(JsonConvert.DeserializeObject<Task[]>(File.ReadAllText("Resources/data.json")) ??
                           throw new InvalidOperationException());
        }

        public static void WriteTasks()
        {
            Console.WriteLine("[INFO/IO] Saving tasks to Resources/data.json ..");
            File.WriteAllText("Resources/data.json", JsonConvert.SerializeObject(Tasks, Formatting.Indented));
        }
    }
}