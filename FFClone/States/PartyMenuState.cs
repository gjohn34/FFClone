using FFClone.Controls;
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
        private List<PartyRow> _partyRows = new List<PartyRow>();
        private Texture2D _background;


        public PartyMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = content.Load<Texture2D>("Sprites/Backgrounds/party-screen");
            _party = _gameInfo.Party;
            int spaceBetweenY = _vH / _party.Count;

            if (spaceBetweenY > (int)(0.25f * _vH))
            {
                spaceBetweenY = (int)(0.33f * _vH);
            }
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));
            _party.ForEach(hero =>
            {
                int index = _party.IndexOf(hero);
                int initialYPos = index * spaceBetweenY;
                double cellPositionPercentage = (double)index / (double)(_party.Count - 1);
                int pushDown = (int)(cellPositionPercentage * cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                int yPosition = initialYPos - pushDown + pushDown2;
                Texture2D portrait = content.Load<Texture2D>(hero.Portrait);
                _partyRows.Add(new PartyRow(portrait, _font, hero, new Rectangle(0, yPosition, _vW, cellHeight), true));
            });
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Rectangle(0, 0, _vW, _vH), Color.White);
            _partyRows.ForEach(x => x.Draw(gameTime, spriteBatch));
            
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState k = Keyboard.GetState();
            if (_previousKeyboard.IsKeyDown(Keys.Escape) && k.IsKeyUp(Keys.Escape))
            {
                _stateManager.Back();
            }
            _previousKeyboard = k;
        }
    }
}