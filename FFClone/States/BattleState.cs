using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using FFClone.Helpers.Shapes;

namespace FFClone.States
{
    public class BattleState : State
    {
        public BattleState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int width = _vW / 2 - (int)(_font.MeasureString("battlemode").X / 2);
            spriteBatch.DrawString(_font, "battlemode", new Vector2(width, 0), Color.White);
            int height = (int)(_vH * 0.3);
            spriteBatch.DrawRectangleWithFill(new Rectangle(10, _vH - height, _vW - 10, height - 10), 10, Color.Black, Color.CadetBlue);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}