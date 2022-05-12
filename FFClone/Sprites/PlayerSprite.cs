using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Sprites
{
    public class PlayerSprite : AnimatedSprite
    {
        private Vector2 _velocity = Vector2.Zero;
        private float _speed = 4;

        public PlayerSprite(Texture2D texture, int rows, int columns) : base(texture, rows, columns)
        {
        }

        public override void Update(GameTime gameTime)
        {
            //KeyboardState keyboardState = Keyboard.GetState();
            //Playing = true;
            //if (keyboardState.IsKeyDown(Keys.Up))
            //{
            //    _velocity.Y = -_speed;
            //    Facing = Facing.Up;
            //}
            //else if (keyboardState.IsKeyDown(Keys.Down))
            //{
            //    _velocity.Y = _speed;
            //    Facing = Facing.Down;
            //} else if (keyboardState.IsKeyDown(Keys.Left))
            //{
            //    _velocity.X = -_speed;
            //    Facing = Facing.Left;
            //}
            //else if (keyboardState.IsKeyDown(Keys.Right))
            //{
            //    _velocity.X = _speed;
            //    Facing = Facing.Right;
            //} else
            //{
            //    Playing = false;
            //}

            //// Update position
            //_position += _velocity;

            //// Stop the player when not pressing any key
            //_velocity = Vector2.Zero;
            base.Update(gameTime);
            if (Playing)
            {
                GameInfo.Instance.EncounterInfo.Position = _position;
            }
        }
    }
}
