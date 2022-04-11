using FFClone.Models;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.States
{
    public class PartyMenuState : State
    {
        private GameInfo _gameInfo = GameInfo.Instance;
        private List<Hero> _party;
        private State _sender;

        public PartyMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State sender) : base(game, graphicsDevice, content)
        {
            _party = _gameInfo.Party;
            _sender = sender;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int x = 100;
            int y = 100;
            spriteBatch.Begin();
            foreach (Hero hero in _party)
            {
                spriteBatch.DrawString(_font, hero.Name, new Vector2(x, y), Color.Black);
                y += 100;
            }
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