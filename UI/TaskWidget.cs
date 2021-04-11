using NedoVator.Core;
using NedoVator.Json;
using SFML.System;
using TGUI;

namespace NedoVator.UI
{
    public class TaskWidget
    {
        public readonly Panel Container;
        private readonly Label _description;
        public readonly Button Done;
        public readonly Label Due;
        public readonly Button Failed;
        private readonly Label _title;

        public TaskWidget(int index, MainWindow context)
        {
            var original = (Panel) context.Gui.Get("TaskPanel");
            Container = new Panel(original)
            {
                Position = new Vector2f(0, 160 + index * 140), Name = $"TaskPanel_{index}"
            };

            _title = (Label) Container.Get("Title");
            _title.Name = $"TaskPanel_{index}";

            _description = (Label) Container.Get("Description");
            _description.Name = $"TaskPanel_{index}";

            Due = (Label) Container.Get("DueTo");
            Due.Name = $"TaskPanel_{index}";

            Done = (Button) Container.Get("Done");
            Failed = (Button) Container.Get("Failed");

            Container.Visible = true;
            context.Gui.Add(Container);
        }

        public void SetCurrentTask(Task? task)
        {
            if (task == null)
            {
                Container.Visible = false;
                return;
            }

            _title.Text = task.Title;
            _description.Text = task.Description;
            Due.Text = task.DueTo;
            Container.Visible = true;
        }
    }
}