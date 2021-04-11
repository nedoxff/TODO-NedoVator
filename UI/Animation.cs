using SFML.Graphics;
using SFML.System;

namespace NedoVator.UI
{
    public class Animation : Drawable
    {
        private float _previousElapsed;

        public bool Play = false;

        private readonly RectangleShape _shape;

        public Animation(Texture frames, Vector2i frameSize, Vector2f position, float delay)
        {
            Delay = delay;
            FrameSize = frameSize;
            FrameCount = (int) (frames.Size.X / frameSize.X);

            _shape = new RectangleShape
            {
                Position = position,
                Size = (Vector2f) frameSize,
                FillColor = Color.White,
                Texture = frames
            };

            CurrentFrameIndex = 0;
            _shape.TextureRect = new IntRect(CurrentFrameIndex * FrameSize.X, 0, FrameSize.X, FrameSize.Y);
        }

        private int CurrentFrameIndex { get; set; }
        private int FrameCount { get; }
        private Vector2i FrameSize { get; }

        public int PlayedTimes { get; set; }

        private float Delay { get; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_shape);
        }

        public void Update(float elapsed)
        {
            if (!Play) return;
            if (!(elapsed - _previousElapsed >= Delay)) return;
            _nextFrame();
            _previousElapsed = elapsed;
        }

        private void _nextFrame()
        {
            if (CurrentFrameIndex + 1 > FrameCount - 1)
            {
                CurrentFrameIndex = 0;
                PlayedTimes++;
            }
            else
            {
                CurrentFrameIndex++;
            }

            _shape.TextureRect = new IntRect(CurrentFrameIndex * FrameSize.X, 0, FrameSize.X, FrameSize.Y);
        }
    }
}