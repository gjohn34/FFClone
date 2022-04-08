using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FFClone
{
    public abstract class State
    {
        #region Fields
        protected Game1 _game;
        protected GraphicsDevice _graphicsDevice;
        protected ContentManager _content;
        #endregion
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Resized();
    }
}
