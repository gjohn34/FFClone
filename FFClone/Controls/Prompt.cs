using FFClone.Controls;
using FFClone.Helpers.Keyboard;
using FFClone.Models;
using FFClone.States.Battle.BattleViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;

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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Primitives2D.DrawLine(spriteBatch, Point1, Point2, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point1, Point3, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point2, Point3, Color.Black);
        }
        public void Update(Vector2 point, Point size)
        {
            Point1 = point;
            Point2 = new Vector2(point.X - size.X, point.Y - size.Y);
            Point3 = new Vector2(point.X - size.X, point.Y + size.Y);

        }
    }

    public class Prompt : IComponent
    {
        private KeyboardState _previous;
        public List<Vector2> Vectors;
        private int _promptOn = 0;
        private Triangle _selector;
        private BattleMain _battle;
        private Ability _ability;
        public Rectangle Rectangle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Prompt(List<Vector2> vectors, BattleMain battle, Ability ability)
        {
            _previous = Keyboard.GetState();
            Vectors = vectors;
            _selector = new Triangle(vectors[_promptOn], new Point(35, 12));
            _battle = battle;
            _ability = ability;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            bool changed = false;

            if (keyboard.Released(_previous, Keys.Enter))
            {
                _battle.SetSelected(_promptOn, _ability);
            } else if (keyboard.Released(_previous, Keys.Escape))
            {
                _battle.DisablePrompt();
            } else if (keyboard.Released(_previous, Keys.Down))
            {
                changed = true;
                _promptOn += 1;
                if (_promptOn >= Vectors.Count)
                {
                    _promptOn = 0;
                }
            }
            else if (keyboard.Released(_previous, Keys.Up))
            {
                changed = true;
                _promptOn -= 1;
                if (_promptOn < 0)
                {
                    _promptOn = Vectors.Count - 1;
                }
            }
            if (changed)
            {
                _selector.Update(Vectors[_promptOn], new Point(35, 12));
            }

            _previous = keyboard;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _selector.Draw(gameTime, spriteBatch);
        }
        public void Resized()
        {
            _selector = new Triangle(Vectors[_promptOn], new Point(35, 12));
            //throw new NotImplementedException();
        }
    }
}