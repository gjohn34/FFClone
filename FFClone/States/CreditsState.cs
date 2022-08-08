using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FFClone.States
{
    public class CreditsState : State
    {
        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            // https://opengameart.org/content/grid-background
            // https://opengameart.org/content/animated-character
            // https://opengameart.org/content/dorver-monster
            // https://opengameart.org/content/country-field
            // https://www.spriters-resource.com/psp/finalfantasy4thecompletecollectiontheafteryears/sheet/58218/
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(_font, "credits", new Vector2(_vW / 2 - _font.MeasureString("credits").X / 2, _vH / 2), Color.Black);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}