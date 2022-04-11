using FFClone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
namespace FFClone.States
{
    public class MenuItem : IComponent
    {
        private SpriteFont _font;

        public string Text { get; set; }
        public Rectangle Rectangle { get; set; }
        private Color FillColor { get; set; }
        public bool Selected { get; internal set; }
        public bool Pressed { get; internal set; }

        public event EventHandler Touch;


        public MenuItem(string text, Rectangle rectangle, SpriteFont font, Color color)
        {
            Text = text;
            Rectangle = rectangle;
            _font = font;
            FillColor = color;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            Primitives2D.DrawRectangle(spriteBatch, new Rectangle(Rectangle.X - 1, Rectangle.Y - 1, Rectangle.Width + 2, Rectangle.Height + 2), Color.Black);
            Primitives2D.FillRectangle(spriteBatch, Rectangle, Selected ? FillColor : Color.MediumVioletRed);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color.Black);
            }
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (Pressed)
            {
                
                Touch?.Invoke(this, new EventArgs());
                Pressed = false;
            }
        }

    }

    public class MenuList : IComponent
    {
        private KeyboardState _previousState;
        public Rectangle Rectangle { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private int Selected { get; set; } = 0;
        public MenuList(List<string> menuItems, Rectangle rectangle, SpriteFont spriteFont)
        {
            Rectangle = rectangle;
            foreach (string option in menuItems)
            {
                MenuItems.Add(new MenuItem(option, GeneratePosition(menuItems.IndexOf(option), menuItems.Count), spriteFont, Color.Gray));
            }
            MenuItems[Selected].Selected = true;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MenuItem menuItem in MenuItems)
                menuItem.Draw(gameTime, spriteBatch);

            //_selector.Draw(gameTime, spriteBatch);
        }
        public void Resized()
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                menuItem.Rectangle = GeneratePosition(MenuItems.IndexOf(menuItem), MenuItems.Count);
                //menuItem.Rectangle = new Rectangle(Rectangle.X - menuItem.Rectangle.Width, menuItem.Rectangle.Y, Rectangle.Width, menuItem.Rectangle.Height);
                //menuItem.Rectangle = new Rectangle(clientBounds.X - menuItem.Rectangle.Width, menuItem.Rectangle.Y, menuItem.Rectangle.Width, menuItem.Rectangle.Height);

            }
        }
        private Rectangle GeneratePosition(int index, int length)
        {

            int spaceBetween = Rectangle.Height / length;
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetween));

            int initialYPos = index * spaceBetween + Rectangle.Y;
            double cellPositionPercentage = (double)index / (double)(length - 1);
            int pushDown = (int)(cellPositionPercentage * cellHeight);
            int pushDown2 = (int)(cellPositionPercentage * spaceBetween);
            return new Rectangle(Rectangle.X, initialYPos - pushDown + pushDown2, Rectangle.Width, cellHeight);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            bool changed = false;
            if (state.IsKeyDown(Keys.Up) && _previousState.IsKeyUp(Keys.Up))
            {
                MenuItems[Selected].Selected = false;
                Selected -= 1;
                changed = true;
            }
            else if (state.IsKeyDown(Keys.Down) && _previousState.IsKeyUp(Keys.Down))
            {
                MenuItems[Selected].Selected = false;
                Selected += 1;
                changed = true;
            }
            else if (state.IsKeyUp(Keys.Enter) && _previousState.IsKeyDown(Keys.Enter))

            //else if (state.IsKeyDown(Keys.Enter) && !_previousState.IsKeyUp(Keys.Enter))
            {
                MenuItems[Selected].Pressed = true;
            }
            if (changed)
            {
                if (Selected >= MenuItems.Count)
                {
                    Selected = 0;
                }
                else if (Selected < 0)
                {
                    Selected = MenuItems.Count - 1;
                }
                MenuItems[Selected].Selected = true;
            }
            _previousState = state;
            foreach (MenuItem menuItem in MenuItems)
                menuItem.Update(gameTime);
        }
    }
}
