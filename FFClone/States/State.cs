using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FFClone
{
    public abstract class State
    {
        #region Fields
        private Game _game;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        #endregion
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {

        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }
}
