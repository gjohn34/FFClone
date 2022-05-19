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
    public interface IMenuOption : IComponent
    {
        public string Label { get; set; }
        public EventHandler OnSubmit { get; set; }
        public bool Selected { get; set; }
        public bool Pressed { get; set; }

    }
    public class MenuOption : IMenuOption
    {
        public string Label { get; set; }
        public EventHandler OnSubmit { get; set; }
        public Rectangle Rectangle { get; set; }
        public bool Selected { get;  set; }
        public bool Pressed { get; set; }

        public MenuOption(string label, EventHandler onSubmit)
        {
            Label = label;
            OnSubmit = onSubmit;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Resized()
        {
        }
    }
    public class MenuItem : IMenuOption
    {
        private SpriteFont _font;
        public Rectangle Rectangle { get; set; }
        private Color FillColor { get; set; } = Color.Gray;
        public bool Selected { get; set; }
        public bool Pressed { get; set; }
        public string Label { get; set; }
        public EventHandler OnSubmit { get; set; }

        public MenuItem(string text, SpriteFont font, EventHandler onSubmit)
        {
            Label = text;
            _font = font;
            OnSubmit = onSubmit;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(Rectangle, 1, Color.Black, Selected ? FillColor : Color.MediumVioletRed);
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(Rectangle.X - 1, Rectangle.Y - 1, Rectangle.Width + 2, Rectangle.Height + 2), Color.Black);
            //Primitives2D.FillRectangle(spriteBatch, Rectangle, Selected ? FillColor : Color.MediumVioletRed);

            if (!string.IsNullOrEmpty(Label))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Label).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Label).Y / 2);

                spriteBatch.DrawString(_font, Label, new Vector2(x, y), Color.Black);
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

        public List<IMenuOption> MenuItems { get; set; }
        private int Selected { get; set; } = 0;
        public MenuList(
            List<IMenuOption> menuItems,
            Rectangle rectangle,
            SpriteFont spriteFont,
            Orientation orientation = Orientation.Vertical, int columns = 1, float opacity = 0f
        )
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
                foreach (IMenuOption option in menuItems)
                {
                    int fontWidth = (int)spriteFont.MeasureString(option.Label).X;
                    int fontHeight = (int)spriteFont.MeasureString(option.Label).Y;
                    if (fontWidth > _minCellWidth)
                        _minCellWidth = fontWidth;
                    if (fontHeight > _minCellHeight)
                        _minCellHeight = fontHeight;
                }

            }
            MenuItems = menuItems;
            foreach (IMenuOption option in MenuItems)
            {
                option.Rectangle = GeneratePosition(MenuItems.IndexOf(option), MenuItems.Count);
            }
            //{
            //    MenuItems.Add(new MenuItem(
            //        menuItems[i].Label,
            //        GeneratePosition(i, menuItems.Count),
            //        spriteFont,
            //        Color.Gray,
            //        menuItems[i].OnSubmit
            //    ));
            //}
            MenuItems[Selected].Selected = true;

        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(Rectangle, Color.White);
            spriteBatch.DrawRectangleWithFill(Rectangle, 0, Color.Black, _orientation == Orientation.Vertical ? Color.White : Color.Black * _opacity);
            foreach (IMenuOption menuItem in MenuItems)
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

                    if (length == 1)
                        return new Rectangle(Rectangle.X, Rectangle.Center.Y - (int)(0.5f * (Rectangle.Height / 3)), Rectangle.Width, Rectangle.Height / 3);

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
            foreach (IComponent menuItem in MenuItems)
                menuItem.Update(gameTime);
        }
    }
}
