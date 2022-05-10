using FFClone.Controls;
using FFClone.Helpers.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
namespace FFClone.Controls
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


    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class MenuList : IComponent
    {
        private KeyboardState _previousState;
        private Orientation _orientation;
        private int _columns;
        public Rectangle Rectangle { get; set; }

        private int _minCellWidth;
        private int _minCellHeight;
        private int _rows;
        private float _opacity = 1f;

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private int Selected { get; set; } = 0;
        public MenuList(List<string> menuItems, Rectangle rectangle, SpriteFont spriteFont, Func<string, EventHandler> onSubmit, Orientation orientation = Orientation.Vertical, int columns = 1, float opacity = 0f)
        {
            _orientation = orientation;
            _columns = columns;
            _opacity = opacity;
            Rectangle = rectangle;
            _minCellWidth = 0;
            _minCellHeight = 0;
            _rows = (int)Math.Ceiling((decimal)menuItems.Count / columns);
            if (_orientation == Orientation.Horizontal)
            {
                foreach (string option in menuItems)
                {
                    int fontWidth = (int)spriteFont.MeasureString(option).X;
                    int fontHeight = (int)spriteFont.MeasureString(option).Y;
                    if (fontWidth > _minCellWidth)
                        _minCellWidth = fontWidth;
                    if (fontHeight > _minCellHeight)
                        _minCellHeight = fontHeight;
                }

            }
            for (int i = 0; i < menuItems.Count; i++)
            {
                MenuItems.Add(new MenuItem(menuItems[i], GeneratePosition(i, menuItems.Count), spriteFont, Color.Gray, onSubmit(menuItems[i])));
            }
            MenuItems[Selected].Selected = true;

        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(Rectangle, 0, Color.Black, _orientation == Orientation.Vertical ? Color.White : Color.Black * _opacity);
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
            int spaceBetweenY, spaceBetweenX, cellHeight, cellWidth, initialYPos, initialXPos, pushDown, pushDown2;
            double cellPositionPercentage;
            switch (_orientation)
            {
                case Orientation.Horizontal:

                    int availableWidth = Rectangle.Width - (_columns * _minCellWidth);
                    spaceBetweenX = availableWidth / (_columns);

                    int availableHeight = Rectangle.Height;
                    int totalHeight = _minCellHeight * _rows;
                    //if (totalHeight > availableHeight)
                    //{
                    //    // help me jesus
                    //} else
                    //{
                    availableHeight -= totalHeight;
                    //}

                    spaceBetweenY = availableHeight / (_rows);

                    initialYPos = (int)Math.Floor((double)(index / _columns)) * _minCellHeight + Rectangle.Y + ((int)Math.Floor((double)(index / _columns)) * spaceBetweenY) + (int)(0.5 * spaceBetweenY);
                    initialXPos = ((index % _columns) * _minCellWidth) + Rectangle.X + ((index % _columns) * spaceBetweenX + (int)(0.5 * spaceBetweenX));
                    //cellPositionPercentage = (double)index / (double)(length - 1);


                    Rectangle rect = new Rectangle(initialXPos, initialYPos, _minCellWidth, _minCellHeight);
                    return rect;
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
                Selected -= (_orientation == Orientation.Vertical ? 1 : _columns);
                changed = true;
            }
            else if (state.IsKeyDown(Keys.Down) && _previousState.IsKeyUp(Keys.Down))
            {
                MenuItems[Selected].Selected = false;
                Selected += (_orientation == Orientation.Vertical ? 1 : _columns);
                changed = true;
            }
            else if (state.IsKeyDown(Keys.Left) && _previousState.IsKeyUp(Keys.Left))
            {
                if (_orientation == Orientation.Horizontal)
                {
                    MenuItems[Selected].Selected = false;
                    Selected -= 1;
                    changed = true;
                }
            }
            else if (state.IsKeyDown(Keys.Right) && _previousState.IsKeyUp(Keys.Right))
            {
                if (_orientation == Orientation.Horizontal)
                {
                    MenuItems[Selected].Selected = false;
                    Selected += 1;
                    changed = true;
                }
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
