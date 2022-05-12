using FFClone.Helpers.Shapes;
using FFClone.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Controls
{
    public class PartyRow : IComponent
    {
        private Texture2D _portrait;
        private SpriteFont _font;
        private Hero _hero;
        public Hero Hero { get { return _hero; } }
        public Rectangle Rectangle { get; set; }

        public PartyRow(Texture2D portrait, SpriteFont font, Hero hero, Rectangle rectangle)
        {
            _hero = hero;
            _portrait = portrait;
            _font = font;
            Rectangle = rectangle;
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int yPosition = Rectangle.Y;
            // Drawing portrait - new rectangle set to match height of row
            spriteBatch.Draw(_portrait, new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Height, Rectangle.Height), Color.White);
            spriteBatch.DrawString(_font, _hero.Name, new Vector2(Rectangle.Height, yPosition), Color.Black);
            yPosition += _font.LineSpacing;
            spriteBatch.ProgressBar(_font, "XP", _hero.Experience, _hero.ToNextLevel, Color.Blue, Color.White, new Rectangle(Rectangle.Height, yPosition, 100, 20));
            //spriteBatch.DrawRectangleWithFill(Rectangle, 1, Color.Black, _hero.Color);
        }

        public void Resized()
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
