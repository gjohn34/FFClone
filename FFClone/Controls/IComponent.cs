using FFClone.Helpers.Keyboard;
using FFClone.Helpers.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Controls
{
    public interface IComponent
    {
        public Rectangle Rectangle { get; set; }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
        public abstract void Resized();
    }

    public class TestComponent : IComponent
    {
        public Rectangle Rectangle { get; set; }
        public string Label { get; set; }

        private SpriteFont _font;

        public TestComponent(Rectangle rectangle, string label, SpriteFont spriteFont)
        {
            Rectangle = rectangle;
            Label = label;
            _font = spriteFont;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.DrawRectangleWithFill(Rectangle, 1, Color.Black, Color.White);
            spriteBatch.DrawString(_font, Label, Rectangle.Center.ToVector2(), Color.Black);
        }
        public void Update(GameTime gameTime) {}
        public void Resized() { }

    }
}
