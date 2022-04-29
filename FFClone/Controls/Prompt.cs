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
using Action = FFClone.Models.Action;

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
        public List<IBattleable> Options;
        private IBattleable _promptOn;
        private Triangle _selector;
        private BattleMain _battle;
        private Action _action;
        public EventHandler OnConfirm { get; set; }
        public Rectangle Rectangle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Prompt(List<IBattleable> options, BattleMain battle, Action action)
        {
            _previous = Keyboard.GetState();
            //Vectors = vectors;
            //Options = options.ConvertAll<Vector2>(x => new Vector2(x.BattleSprite.Position.X, x.BattleSprite.Position.Y + (int)(0.5 * x.BattleSprite.Height));
            Options = options;
            _promptOn = options[0];
            _selector = new Triangle(new Vector2(_promptOn.BattleSprite.Position.X, _promptOn.BattleSprite.Position.Y + (int)(0.5 * _promptOn.BattleSprite.Height)), new Point(35, 12));
            _battle = battle;
            _action = action;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            bool changed = false;

            if (keyboard.Released(_previous, Keys.Enter))
            {
                //OnConfirm.Invoke(_battle, new EventArgs());
                _battle.SetSelected(_promptOn, _action);
            } else if (keyboard.Released(_previous, Keys.Escape))
            {
                _battle.DisablePrompt();
            } else if (keyboard.Released(_previous, Keys.Down))
            {
                changed = true;
                try
                {
                    _promptOn = Options[Options.IndexOf(_promptOn) + 1];
                } catch
                {
                    _promptOn = Options[0];
                }
            }
            else if (keyboard.Released(_previous, Keys.Up))
            {
                changed = true;
                try
                {
                    _promptOn = Options[Options.IndexOf(_promptOn) - 1];
                }
                catch
                {
                    _promptOn = Options[Options.Count - 1];
                }
            }
            if (changed)
            {
                _selector.Update(
                    new Vector2(_promptOn.BattleSprite.Position.X, _promptOn.BattleSprite.Position.Y + (int)(0.5 * _promptOn.BattleSprite.Height)), 
                    new Point(35, 12)
                );
            }

            _previous = keyboard;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _selector.Draw(gameTime, spriteBatch);
        }
        public void Resized()
        {
            _selector = new Triangle(new Vector2(_promptOn.BattleSprite.Position.X, _promptOn.BattleSprite.Position.Y + (int)(0.5 * _promptOn.BattleSprite.Height)), new Point(35, 12));

            //throw new NotImplementedException();
        }
    }
}