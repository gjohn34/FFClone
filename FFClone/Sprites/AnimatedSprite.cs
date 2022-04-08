﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Sprites
{
    public enum Facing
    {
        Left,
        Right,
        Up,
        Down
    }
    public class AnimatedSprite : Sprite
    {
        private int _currentFrame = 0;
        private int _totalFrames;
        private int _tick = 0;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Facing Facing = Facing.Down;
        public bool Playing = false;

        public AnimatedSprite(Texture2D texture, int rows, int columns) : base(texture)
        {
            Rows = rows;
            Columns = columns;
            _totalFrames = rows * columns;
        }

        public override void Update(GameTime gameTime)
        {
            if (Playing)
            {
                _tick++;
                if (_tick >= 6)
                {
                    _tick = 0;
                    _currentFrame++;
                    if (_currentFrame >= _totalFrames)
                    {
                        _currentFrame = 0;
                    }
                }
            } else
            {
                _tick = 0;
                _currentFrame = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int width = _texture.Width / Columns;
            int height = _texture.Height / Rows;
            int row = 0;
            switch (Facing)
            {
                case Facing.Left:
                    row = 2;
                    break;
                case Facing.Right:
                    row = 1;
                    break;
                case Facing.Up:
                    row = 3;
                    break;
                case Facing.Down:
                    row = 0;
                    break;
                default:
                    break;
            }
            int column = _currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)_position.X, (int)_position.Y, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
