using FFClone.Controls;
using FFClone.Helpers.Shapes;
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

        private EventHandler OnSubmit;

        public MenuItem(string text, Rectangle rectangle, SpriteFont font, Color color, EventHandler onSubmit)
        {
            Text = text;
            Rectangle = rectangle;
            _font = font;
            FillColor = color;
            OnSubmit = onSubmit;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(Rectangle, 1, Color.Black, Selected ? FillColor : Color.MediumVioletRed);
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(Rectangle.X - 1, Rectangle.Y - 1, Rectangle.Width + 2, Rectangle.Height + 2), Color.Black);
            //Primitives2D.FillRectangle(spriteBatch, Rectangle, Selected ? FillColor : Color.MediumVioletRed);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color.Black);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Pressed)
            {
                OnSubmit?.Invoke(this, new EventArgs());
                Pressed = false;
            }
        }
        public void Resized() { }

    }

    public class MenuList : IComponent
    {
        private enum Orientation
        {
            Horizontal,
            Vertical
        }
        private KeyboardState _previousState;
        private Orientation _orientation = Orientation.Vertical;
        private int _columns = 1;
        public Rectangle Rectangle { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private int Selected { get; set; } = 0;
        public MenuList(List<string> menuItems, Rectangle rectangle, SpriteFont spriteFont, Func<string, EventHandler> onSubmit)
        {
            Rectangle = rectangle;
            foreach (string option in menuItems)
            {
                MenuItems.Add(new MenuItem(option, GeneratePosition(menuItems.IndexOf(option), menuItems.Count), spriteFont, Color.Gray, onSubmit(option)));
            }
            MenuItems[Selected].Selected = true;

        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(Rectangle, 0, Color.Black, Color.White);
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
            int spaceBetweenY, spaceBetweenX, cellHeight, initialYPos, initialXPos, pushDown, pushDown2;
            double cellPositionPercentage;
            switch (_orientation)
            {
                case Orientation.Horizontal:
                    // 100
                    spaceBetweenY = Rectangle.Height / (_columns / length);
                    // 33
                    spaceBetweenX = Rectangle.Width / _columns;
                    // 75
                    cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));

                    initialYPos = (int)Math.Floor((double)(index / _columns)) * spaceBetweenY + Rectangle.Y;
                    initialXPos = (int)Math.Floor((double)(index / _columns)) *spaceBetweenX + Rectangle.X;
                    cellPositionPercentage = (double)index / (double)(length - 1);
                    return new Rectangle(Rectangle.X, initialYPos, Rectangle.Width, cellHeight);

                    break;
                case Orientation.Vertical:
                    spaceBetweenY = Rectangle.Height / length;
                    cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));

                    initialYPos = index * spaceBetweenY + Rectangle.Y;
                    cellPositionPercentage = (double)index / (double)(length - 1);
                    pushDown = (int)(cellPositionPercentage * cellHeight);
                    pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                    return new Rectangle(Rectangle.X, initialYPos - pushDown + pushDown2, Rectangle.Width, cellHeight);
                default:

                    spaceBetweenY = Rectangle.Height / length;
                    cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));

                    initialYPos = index * spaceBetweenY + Rectangle.Y;
                    cellPositionPercentage = (double)index / (double)(length - 1);
                    pushDown = (int)(cellPositionPercentage * cellHeight);
                    pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                    return new Rectangle(Rectangle.X, initialYPos - pushDown + pushDown2, Rectangle.Width, cellHeight);
            }
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
