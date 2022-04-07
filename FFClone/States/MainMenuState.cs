using FFClone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
            int spaceBetween = rectangle.Height / menuItems.Count;
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetween));
            foreach (string option in menuItems)
            {
                int initialYPos = menuItems.IndexOf(option) * spaceBetween;
                double cellPositionPercentage = (double)menuItems.IndexOf(option) / (double)(menuItems.Count - 1);
                int pushDown = (int)(cellPositionPercentage * cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetween);
                Rectangle r = new Rectangle(rectangle.X, initialYPos - pushDown + pushDown2, 200, cellHeight);
                MenuItems.Add(new MenuItem(option, r, spriteFont, Color.Gray));
            }
            MenuItems[Selected].Selected = true;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MenuItem menuItem in MenuItems)
                menuItem.Draw(gameTime, spriteBatch);

            //_selector.Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            bool changed = false;
            if (state.IsKeyDown(Keys.Up) & _previousState.IsKeyUp(Keys.Up))
            {
                MenuItems[Selected].Selected = false;
                Selected -= 1;
                changed = true;
            }
            else if (state.IsKeyDown(Keys.Down) & _previousState.IsKeyUp(Keys.Down))
            {
                MenuItems[Selected].Selected = false;
                Selected += 1;
                changed = true;
            } 
            else if (state.IsKeyDown(Keys.Enter) & _previousState.IsKeyUp(Keys.Enter))
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
    public class Triangle
    {
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }
        public Vector2 Point3 { get; set; }
        public Triangle(Vector2 point, Point size)
        {
            Point1 = point;
            Point2 = new Vector2(point.X - size.X, point.Y - size.Y);
            Point3 = new Vector2(point.X - size.X, point.Y + size.Y);
        }
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
        }
        public void Update(GameTime gameTime)
        {
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Primitives2D.DrawLine(spriteBatch, Point1, Point2, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point1, Point3, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point2, Point3, Color.Black);
        }
    }
    public class MainMenuState : State
    {
        private SpriteFont _font;
        private MenuList _menuList;

        public MainMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _font = content.Load<SpriteFont>("Font/font");
            List<string> options = new List<string> { "New", "Load", "Help", "foo" };
            //_selector = new Triangle(new Vector2(item.X - 50, item.Y + (int)Math.Ceiling((double)0.5 * item.Height)), new Point(100, (int)Math.Ceiling(0.25 * _cellHeight)));
            _menuList = new MenuList(options, new Rectangle(_game.Window.ClientBounds.Width - 200, 0, 200, game.Window.ClientBounds.Height), _font);
            foreach (MenuItem item in _menuList.MenuItems)
            {
                item.Touch += (a, b) => ChangeState(a, b, item.Text); 
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _menuList.Draw(gameTime, spriteBatch);
        }

        public void ChangeState(object sender, EventArgs e, string option)
        {
            switch (option)
            {
                case "New":
                    Debug.WriteLine("New selected");
                    break;
                case "Load":
                    Debug.WriteLine("Load selected");
                    break;
                case "Help":
                    Debug.WriteLine("Help selected");
                    break;
                //StateManager.Instance.Next(new NewGameState());
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _menuList.Update(gameTime);
            //Rectangle item = _menuItems[_selected];
            //_selector = new Triangle(new Vector2(item.X - 50, item.Y + (int)Math.Ceiling((double)0.5 * item.Height)), new Point(100, (int)Math.Ceiling(0.25 * _cellHeight)));
        }
    }
}
