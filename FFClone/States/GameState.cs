using FFClone.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;

namespace FFClone.States
{
    public class GameState : State
    {
        private PlayerSprite _player;
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _player = new PlayerSprite(content.Load<Texture2D>("Sprites/guy"), 4, 4);
            //{ Rectangle = new Rectangle(100, 100, 50, 50) };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _player.Draw(spriteBatch);
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(0, 0, 200, 200), Color.Black);
        }

        public override void Resized()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }
    }
}