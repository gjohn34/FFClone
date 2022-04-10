using FFClone.Controls;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private MenuList _menuList;

        public MainMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            List<string> options = new List<string> { "New", "Load", "Help", "foo" };

            int width = (int)Math.Ceiling(_game.Window.ClientBounds.Width * 0.2);
            _menuList = new MenuList(options, new Rectangle(_game.Window.ClientBounds.Width - width, 0, width, _game.Window.ClientBounds.Height), _font);
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
                    StateManager.Instance.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
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
        }

        public override void Resized()
        {
            int width = (int)Math.Ceiling(_vW * 0.2);
            _menuList.Rectangle = new Rectangle(_vW - width, 0, width, _vH);
            _menuList.Resized(_game.Window.ClientBounds);
        }
    }
}
