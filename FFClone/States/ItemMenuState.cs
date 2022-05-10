using FFClone.Models;
using FFClone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace FFClone.States
{
    internal class ItemMenuState : State
    {
        private List<Hero> _party = GameInfo.Instance.Party;
        private MenuList _list;

        public ItemMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _list = new MenuList(
                new List<string> { "Potion", "Hi-Potion", "Wa'er" },
                new Rectangle((int)(0.1 * _vW), (int)(0.1 * _vH), _vW - (int)(0.2 * _vW), _vH - (int)(0.2 * _vH)),
                _font,
                Handler,
                Orientation.Horizontal,
                3
            );
        }

        public EventHandler Handler(string x)
        {
            return (a,b) => { };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _list.Draw(gameTime, spriteBatch);
            spriteBatch.End();

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
            {
                _stateManager.Back();
            }
            _list.Update(gameTime);

            _previousKeyboard = ks;
        }
    }
}