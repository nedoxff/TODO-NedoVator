using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NedoVator.Core;
using NedoVator.Json;
using NedoVator.UI;
using SFML.Graphics;
using SFML.System;
using TGUI;

namespace NedoVator.Scenes
{
    public class MainScene : Scene
    {
        private Label _clock = null!;
        private Animation _currentAnimation = null!;
        private int _currentScrollAmount;
        private int _displayTaskWidgets;
        private Animation _failed = null!;
        private Animation _idle = null!;

        private bool _notify = true;
        private float _previousElapsed;
        private Scrollbar _scrollbar = null!;
        private Animation _surprised = null!;
        private readonly List<TaskWidget> _taskWidgets = new();

        public override void Show(MainWindow window)
        {
            window.Gui.LoadWidgetsFromFile("Resources/Scenes/MainScene.txt");
            window.ScrollingBackground = new ScrollingBackground(new CircleShape
            {
                FillColor = new Color(140, 27, 27),
                Radius = 50f
            }, new Color(155, 30, 30), new Vector2u(1280, 720), new Vector2u(20, 20), new Vector2f(20, 20));


            var clock = window.Gui.Get("Time") as Label;
            _clock = clock!;
            var createNewTask = window.Gui.Get("CreateNewTask") as Button;
            var createWindow = window.Gui.Get("CreateTaskWindow") as ChildWindow;
            var scrollbar = window.Gui.Get("Scrollbar") as Scrollbar;
            _scrollbar = scrollbar!;

            Task.LoadTasks();
            _placeTaskWidgets(window);

            if (Task.Tasks.Count > 4)
            {
                scrollbar!.Visible = true;
                scrollbar!.Maximum = (uint) (Task.Tasks.Count - 3);
            }

            _idle = new Animation(window.Resources.Textures["calm"], new Vector2i(820, 585), new Vector2f(460, 140),
                1000f);
            _surprised = new Animation(window.Resources.Textures["completed"], new Vector2i(820, 585),
                new Vector2f(460, 140), 1000f);
            _failed = new Animation(window.Resources.Textures["fail"], new Vector2i(820, 585), new Vector2f(460, 140),
                1000f);
            _currentAnimation = _idle;
            _currentAnimation.Play = true;

            UpdateScrollbar();
            createWindow!.Closed += (_, _) => ResetWindow(createWindow);
            (createWindow.Get("Create") as Button)!.Clicked += (_, _) => TryCreate(createWindow, window);
            createNewTask!.Clicked += (_, _) => createWindow!.Visible = true;

            while (window.Window.IsOpen)
                Update(window);
        }

        private void Update(MainWindow window)
        {
            var elapsedSecs = window.ElapsedClock.ElapsedTime.AsSeconds();
            if (elapsedSecs - _previousElapsed >= 60f && _notify)
            {
                var soonTask = Task.Tasks.FirstOrDefault(t =>
                    DateTime.ParseExact(t.DueTo, "G", CultureInfo.InvariantCulture, DateTimeStyles.None) -
                    DateTime.Now <= TimeSpan.FromMinutes(30));
                if (soonTask != null)
                {
                    Console.WriteLine("[INFO/NOTIFY] Found soon tasks, requesting focus..");
                    window.Window.RequestFocus();
                    var messageBox = new MessageBox("Warning",
                        "You have task(s) which will expire in 30 minutes or less! Please check if you finished/failed it.",
                        new[] {"OK"});
                    messageBox.Closed += (_, _) => messageBox.Visible = false;
                    messageBox.ButtonPressed += (_, _) => messageBox.Visible = false;
                    messageBox.PositionLayout = new Layout2d("25%", "50%");
                    window.Gui.Add(messageBox);
                    _notify = false;
                }

                _previousElapsed = elapsedSecs;
            }

            _clock!.Text = DateTime.Now.ToString("F", CultureInfo.InvariantCulture);
            _currentAnimation.Update(window.ElapsedClock.ElapsedTime.AsMilliseconds());
            _currentAnimation.Update(window.ElapsedClock.ElapsedTime.AsMilliseconds());
            window.Update();
            window.Window.Draw(_currentAnimation);
            window.Gui.Draw();
            window.Window.Display();
        }

        private void _placeTaskWidgets(MainWindow window)
        {
            foreach (var taskWidget in _taskWidgets)
                window.Gui.Remove(taskWidget.Container);
            _taskWidgets.Clear();
            if (Task.Tasks.Count == 0) return;
            _displayTaskWidgets = Task.Tasks.Count >= 4 ? 4 : Task.Tasks.Count;
            for (var i = 0; i < _displayTaskWidgets; i++)
            {
                var taskWidget = new TaskWidget(i, window);
                var i1 = i;
                taskWidget.Done.Clicked += (_, _) =>
                {
                    window.Resources.SoundFromName("success").Play();
                    RemoveTask(i1, window);
                    _currentAnimation = _surprised;
                    _currentAnimation.PlayedTimes = 0;
                    _currentAnimation.Play = true;
                    while (_currentAnimation.PlayedTimes != 3)
                        Update(window);
                    _currentAnimation = _idle;
                };
                taskWidget.Failed.Clicked += (_, _) =>
                {
                    window.Resources.SoundFromName("fail").Play();
                    RemoveTask(i1, window);
                    _currentAnimation = _failed;
                    _currentAnimation.PlayedTimes = 0;
                    _currentAnimation.Play = true;
                    while (_currentAnimation.PlayedTimes != 3)
                        Update(window);
                    _currentAnimation = _idle;
                };
                taskWidget.SetCurrentTask(Task.Tasks[i]);
                if (DateTime.ParseExact(taskWidget!.Due.Text, "G", CultureInfo.InvariantCulture, DateTimeStyles.None) <
                    DateTime.Now)
                    taskWidget.Due.Renderer.TextColor = Color.Red;
                _taskWidgets.Add(taskWidget);
            }
        }

        private static void ResetWindow(Container createWindow)
        {
            (createWindow.Get("TitleBox") as EditBox)!.Text = "";
            (createWindow.Get("DueBox") as EditBox)!.Text = "";
            (createWindow.Get("DescriptionBox") as TextBox)!.Text = "";
            createWindow.Visible = false;
        }

        private void RemoveTask(int index, MainWindow window)
        {
            Console.WriteLine("[INFO/TASKS] Removing task requested..");
            if (index + _currentScrollAmount < 0 || index + _currentScrollAmount >= Task.Tasks.Count) return;
            Task.Tasks.RemoveAt(index + _currentScrollAmount);
            Task.WriteTasks();
            _placeTaskWidgets(window);
            UpdateScrollbar();
        }

        private void UpdateScrollbar()
        {
            switch (Task.Tasks.Count)
            {
                case <= 4:
                    _currentScrollAmount = 0;
                    _scrollbar.Visible = false;
                    break;
                case > 4:
                    _scrollbar!.Visible = true;
                    _scrollbar!.Maximum = (uint) (Task.Tasks.Count - 3);
                    _scrollbar!.ValueChanged += (_, i) =>
                    {
                        for (var j = 0; j < 4; j++)
                            _taskWidgets[j].SetCurrentTask(Task.Tasks[(int) i.Value + j]);
                        _currentScrollAmount = (int) i.Value;
                    };
                    break;
            }
        }

        private void TryCreate(Container createWindow, MainWindow window)
        {
            var titleBox = createWindow.Get("TitleBox") as EditBox;
            var dueBox = createWindow.Get("DueBox") as EditBox;
            var descriptionBox = createWindow.Get("DescriptionBox") as TextBox;
            var error = createWindow.Get("Error") as Label;
            if (string.IsNullOrEmpty(titleBox!.Text))
            {
                error!.Text = "Title of the task cannot be empty!";
                return;
            }

            if (string.IsNullOrEmpty(descriptionBox!.Text))
            {
                error!.Text = "Description of the task cannot be empty!";
                return;
            }

            if (string.IsNullOrEmpty(dueBox!.Text))
            {
                error!.Text = "Due Date of the task cannot be empty!";
                return;
            }


            if (!DateTime.TryParseExact(dueBox!.Text, "G", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var date))
            {
                error!.Text = "Invalid Due Date format!";
                return;
            }

            if (DateTime.Now > date)
            {
                error!.Text = "Are you building a time machine? :D";
                return;
            }

            var task = new Task(titleBox!.Text, descriptionBox!.Text, dueBox!.Text);
            Task.Tasks.Add(task);
            Task.WriteTasks();
            _placeTaskWidgets(window);
            UpdateScrollbar();

            window.Resources.SoundFromName("success").Play();
            ResetWindow(createWindow);
            Console.WriteLine("[INFO/TASKS] New task was created..");

            _currentAnimation = _surprised;
            _currentAnimation.PlayedTimes = 0;
            _currentAnimation.Play = true;
            while (_currentAnimation.PlayedTimes != 3)
                Update(window);
            _currentAnimation = _idle;
        }
    }
}