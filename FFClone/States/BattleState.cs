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
            int thickness = 10;
            spriteBatch.DrawRectangleWithFill(new Rectangle(thickness, _vH - height, _vW - thickness, height - thickness), thickness, Color.Black, Color.CadetBlue);
            
            // heroes
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.10), 50, 50), 1, Color.Black, Color.Red);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.20) + 50, 50, 50), 1, Color.Black, Color.Yellow);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.30) + 100, 50, 50), 1, Color.Black, Color.Green);

            // enemies
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.10), 50, 50), 1, Color.Black, Color.Purple);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.20) + 50, 50, 50), 1, Color.Black, Color.Purple);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.30) + 100, 50, 50), 1, Color.Black, Color.Purple);

            // commands
            int commandWidth = (int)(_vW * 0.35);
            int menuYPos = _vH - height + thickness;
            int availableSpace = _vW - thickness - commandWidth;
            spriteBatch.DrawRectangle(new Rectangle(thickness, menuYPos, commandWidth - thickness, height - 2 * thickness - 1), Color.Yellow);
            int charInfoWidth = (int)(availableSpace * 0.33);

            spriteBatch.DrawRectangle(new Rectangle(commandWidth + 1, _vH - height + thickness, charInfoWidth, height - 2 * thickness - 1), Color.Lime);
            spriteBatch.DrawRectangle(new Rectangle(commandWidth + charInfoWidth + 3, _vH - height + thickness, charInfoWidth, height - 2 * thickness - 1), Color.Red);
            spriteBatch.DrawRectangle(new Rectangle(commandWidth + 2 * charInfoWidth + 5, _vH - height + thickness, charInfoWidth, height - 2 * thickness - 1), Color.Blue);



            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}