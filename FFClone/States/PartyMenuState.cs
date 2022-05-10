using FFClone.Helpers.Shapes;
using FFClone.Models;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.States
{
    public class PartyMenuState : State
    {
        private GameInfo _gameInfo = GameInfo.Instance;
        private List<Hero> _party;
        private State _sender;
        private List<Texture2D> _portraits = new List<Texture2D>();

        public PartyMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State sender) : base(game, graphicsDevice, content)
        {
            _party = _gameInfo.Party;
            _sender = sender;
            _party.ForEach(hero => _portraits.Add(content.Load<Texture2D>(hero.Portrait)));

        }

        internal void ProgressBar(SpriteBatch spriteBatch, string label, int starting, int ending, Color leftFill, Color rightFill, Rectangle rectangle)
        {

            spriteBatch.DrawString(_font, $"{label} : {starting}/{ending}", new Vector2(rectangle.X, rectangle.Y), Color.Black);
            int barWidth = rectangle.Width;
            int width = (int)(((double)starting / ending) * barWidth);
            int barHeight = rectangle.Height;
            //hp left
            spriteBatch.DrawRectangleWithFill(new Rectangle(rectangle.X, rectangle.Y + _font.LineSpacing, width, barHeight), 0, leftFill, leftFill);
            ////hp missing
            spriteBatch.DrawRectangleWithFill(new Rectangle(rectangle.X + width, rectangle.Y + _font.LineSpacing, barWidth - width, barHeight), 0, rightFill, rightFill);
            //hp bar outline
            spriteBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + _font.LineSpacing, 100, 20), Color.Black);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int x = 100;
            int y = 100;
            spriteBatch.Begin();
            int spaceBetweenY = _vH / _portraits.Count;
            int xMargin = 10;
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));

            _portraits.ForEach(portrait =>
            {
                int index = _portraits.IndexOf(portrait);
                int initialYPos = index * spaceBetweenY;
                double cellPositionPercentage = (double)index / (double)(_portraits.Count - 1);
                int pushDown = (int)(cellPositionPercentage * cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                int yPosition = initialYPos - pushDown + pushDown2;
                Rectangle rect = new Rectangle(0, yPosition, cellHeight, cellHeight);
                spriteBatch.Draw(portrait, rect, Color.White);
                Hero hero = _party[index];
                spriteBatch.DrawString(_font, hero.Name, new Vector2(cellHeight + xMargin, yPosition), Color.Black);
                yPosition += _font.LineSpacing;
                // hp
                ProgressBar(spriteBatch, "HP", hero.HP, hero.MaxHP, Color.Green, Color.Red, new Rectangle(cellHeight + xMargin, yPosition, 100, 20));
                yPosition += _font.LineSpacing + 20 + 5;
                // xp
                ProgressBar(spriteBatch, "XP", hero.Experience, hero.ToNextLevel, Color.Blue, Color.White, new Rectangle(cellHeight + xMargin, yPosition, 100, 20));

                // statblock
                yPosition = initialYPos - pushDown + pushDown2;

                int leftMargin = cellHeight + xMargin + 200;
                int availableSpace = _vW - leftMargin;
                int foo = availableSpace / 3;
                spriteBatch.DrawString(_font, "Stat Block", new Vector2(cellHeight + xMargin + 200, yPosition), Color.Black);
                yPosition += _font.LineSpacing + 5;

                spriteBatch.DrawString(_font, "STR", new Vector2(leftMargin, yPosition), Color.Black);
                spriteBatch.DrawString(_font, "INT", new Vector2(leftMargin + foo, yPosition), Color.Black);
                spriteBatch.DrawString(_font, "DEX", new Vector2(leftMargin + foo + foo, yPosition), Color.Black);
                yPosition += _font.LineSpacing + 5;


                spriteBatch.DrawString(_font, hero.Strength.ToString(), new Vector2(leftMargin, yPosition), Color.Black);
                spriteBatch.DrawString(_font, hero.Intelligence.ToString(), new Vector2(leftMargin + foo, yPosition), Color.Black);
                spriteBatch.DrawString(_font, hero.Dexterity.ToString(), new Vector2(leftMargin + foo + foo, yPosition), Color.Black);

            });
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState k = Keyboard.GetState();
            if (_previousKeyboard.IsKeyDown(Keys.Escape) && k.IsKeyUp(Keys.Escape))
            {
                _stateManager.Next(_sender, Transition.NoTransition);
            } else if (_previousKeyboard.IsKeyDown(Keys.S) && k.IsKeyUp(Keys.S))
            {
                GameInfo.Instance.Shuffle();
            } else if (_previousKeyboard.IsKeyDown(Keys.A) && k.IsKeyUp(Keys.A))
            {
                Debug.WriteLine(_party);
            }
            _previousKeyboard = k;
        }
    }
}