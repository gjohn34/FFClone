using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Sprites
{
    public class Sprite
    {
        protected Texture2D _texture;
        protected Vector2 _position = Vector2.Zero;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }
        public virtual void Update(GameTime gameTime) 
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_texture,
                _position,
                Color.White);
            spriteBatch.End();
        }
    }
}
