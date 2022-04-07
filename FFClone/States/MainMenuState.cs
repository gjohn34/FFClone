using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Text;
namespace FFClone.States
{
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
        private Triangle _selector;
        private int _selected = 0;
        private Game _game;
        private List<Rectangle> _menuItems = new List<Rectangle>();
        private int _cellHeight;

        public MainMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _font = content.Load<SpriteFont>("Font/font");
            _game = game;
            List<string> options = new List<string> { "New", "Load", "Help" };
            int spaceBetween = _game.Window.ClientBounds.Height / options.Count;
            _cellHeight = (int)Math.Ceiling((0.75 * spaceBetween));
            foreach (string option in options)
            {
                int initialYPos = options.IndexOf(option) * spaceBetween;
                double cellPositionPercentage = (double)options.IndexOf(option) / (double)(options.Count - 1);
                int pushDown = (int)(cellPositionPercentage * _cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetween);
                Rectangle r = new Rectangle(_game.Window.ClientBounds.Width - 200, initialYPos - pushDown + pushDown2, 200, _cellHeight);
                _menuItems.Add(r);
            }
            Rectangle item = _menuItems[_selected];
            _selector = new Triangle(new Vector2(item.X - 50, item.Y + (int)Math.Ceiling((double)0.5 * item.Height)), new Point(100, (int)Math.Ceiling(0.25 * _cellHeight)));

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (Rectangle rectangle in _menuItems)
            {
                Primitives2D.DrawRectangle(spriteBatch, rectangle, Color.Black);
            }

            _selector.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
            {
                _selected += 1;
            } else if (state.IsKeyDown(Keys.Down))
            {
                _selected -= 1;
            }
            if (_selected >= _menuItems.Count)
            {
                _selected = 0;
            } else if (_selected < 0)
            {
                _selected = _menuItems.Count - 1;
            }
            Rectangle item = _menuItems[_selected];
            _selector = new Triangle(new Vector2(item.X - 50, item.Y + (int)Math.Ceiling((double)0.5 * item.Height)), new Point(100, (int)Math.Ceiling(0.25 * _cellHeight)));
        }
    }
}
