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
        protected int _vH;
        protected int _vW;
        protected SpriteFont _font;
        protected StateManager _stateManager;
        #endregion
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _vH = game.Window.ClientBounds.Height;
            _vW = game.Window.ClientBounds.Width;
            _font = content.Load<SpriteFont>("Font/font");
            _stateManager = StateManager.Instance;

        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public virtual void Resized() 
        {
            _vH = _game.Window.ClientBounds.Height;
            _vW = _game.Window.ClientBounds.Width;
        }
    }
}
