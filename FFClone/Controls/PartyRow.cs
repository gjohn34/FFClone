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

        private bool _statBlock;

        public PartyRow(Texture2D portrait, SpriteFont font, Hero hero, Rectangle rectangle, bool statBlock = false)
        {
            _hero = hero;
            _portrait = portrait;
            _font = font;
            Rectangle = rectangle;
            _statBlock = statBlock;
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int yPosition = Rectangle.Y;
            // Drawing portrait - new rectangle set to match height of row
            spriteBatch.DrawRectangleWithFill(new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height), 1, Color.Black, _hero.Color);
            spriteBatch.Draw(_portrait, new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Height, Rectangle.Height), Color.White);
            spriteBatch.DrawString(_font, _hero.Name, new Vector2(Rectangle.Height, yPosition), Color.Black);
            yPosition += _font.LineSpacing;
            
            // hp
            spriteBatch.ProgressBar(_font, "HP", _hero.HP, _hero.MaxHP, Color.Green, Color.Red, new Rectangle(Rectangle.Height + 10, yPosition, 100, 20));
            yPosition += _font.LineSpacing + 20 + 5;
            // xp
            spriteBatch.ProgressBar(_font, "XP", _hero.Experience, _hero.ToNextLevel, Color.Blue, Color.White, new Rectangle(Rectangle.Height + 10, yPosition, 100, 20));
            
            if (_statBlock)
            {
                // statblock
                yPosition = Rectangle.Y;

                int leftMargin = Rectangle.Height + 10 + 200;
                int buffer = (Rectangle.Width - leftMargin) / 3;
                spriteBatch.DrawString(_font, "Stat Block", new Vector2(Rectangle.Height + 10 + 200, yPosition), Color.Black);
                yPosition += _font.LineSpacing + 5;

                spriteBatch.DrawString(_font, "STR", new Vector2(leftMargin, yPosition), Color.Black);
                spriteBatch.DrawString(_font, "INT", new Vector2(leftMargin + buffer, yPosition), Color.Black);
                spriteBatch.DrawString(_font, "DEX", new Vector2(leftMargin + buffer + buffer, yPosition), Color.Black);
                yPosition += _font.LineSpacing + 5;


                spriteBatch.DrawString(_font, _hero.Strength.ToString(), new Vector2(leftMargin, yPosition), Color.Black);
                spriteBatch.DrawString(_font, _hero.Intelligence.ToString(), new Vector2(leftMargin + buffer, yPosition), Color.Black);
                spriteBatch.DrawString(_font, _hero.Dexterity.ToString(), new Vector2(leftMargin + buffer + buffer, yPosition), Color.Black);

            }
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
