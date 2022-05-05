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
        private KeyboardState _previous;
        public Rectangle Rectangle { get; set; }
        public EventHandler OnSubmit { get; set; }
        public TestComponent(Rectangle rectangle, EventHandler onSubmit)
        {
            _previous = Keyboard.GetState();
            Rectangle = rectangle;
            OnSubmit = onSubmit;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.DrawRectangleWithFill(Rectangle, 1, Color.Black, Color.White);
        }
        public void Update(GameTime gameTime) {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.Released(_previous, Keys.Enter)) 
            {
                OnSubmit.Invoke(null, new EventArgs());
            }
            _previous = keyboard;
        }
        public void Resized() { }

    }
}
