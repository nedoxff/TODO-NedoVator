using System;
using SFML.Graphics;
using SFML.System;

namespace NedoVator.UI
{
    public class ScrollingBackground : Drawable
    {
        private IntRect _intRect;

        private Vector2f _offset;
        private readonly Vector2f _scrollVelocity;
        private readonly Vector2u _tileSize;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly RenderTexture _renderTexture;
        private readonly Sprite _spriteResult;

        public ScrollingBackground(CircleShape circle, Color background, Vector2u windowSize, Vector2u spacing,
            Vector2f scrollVelocity)
        {
            _scrollVelocity = scrollVelocity;
            _tileSize = new Vector2u(((uint) circle.Radius << 1) + spacing.X, ((uint) circle.Radius << 1) + spacing.Y);
            _renderTexture = new RenderTexture(_tileSize.X, _tileSize.Y) {Repeated = true};
            _renderTexture.Clear(background);
            _renderTexture.Draw(circle);
            _renderTexture.Display();
            _offset = new Vector2f();
            _intRect = new IntRect((int) _offset.X, (int) _offset.Y, (int) windowSize.X + ((int) circle.Radius << 1),
                (int) windowSize.Y + ((int) circle.Radius << 1));
            _spriteResult = new Sprite(_renderTexture.Texture, _intRect);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_spriteResult, states);
        }


        public void Update(Time delta)
        {
            _offset += delta.AsSeconds() * -_scrollVelocity;
            if (_scrollVelocity.X != 0f && _offset.X >= 0f)
                _offset.X -= _tileSize.X;
            if (_scrollVelocity.Y != 0f && _offset.Y >= 0f)
                _offset.Y -= _tileSize.Y;
            _intRect.Left = (int) Math.Round(_offset.X);
            _intRect.Top = (int) Math.Round(_offset.Y);
            _spriteResult.TextureRect = _intRect;
        }
    }
}