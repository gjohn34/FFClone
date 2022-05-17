using FFClone.Controls;
using FFClone.Models;
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
    public class MainMenuState : State
    {
        private MenuList _menuList;

        public MainMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            List<IMenuOption> options = new List<IMenuOption> {
                new MenuOption("New", (object a, EventArgs e) => {
                    SaveFile.New();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    }),
                new MenuOption("Load", (a, e) => {
                    SaveFile.Load();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                }),
                new MenuOption("Help", (a, e) => { }),
                new MenuOption("Credits", (a,e) => { })
            };

            int width = (int)Math.Ceiling(_game.Window.ClientBounds.Width * 0.2);
            _menuList = new MenuList(
                options,
                new Rectangle(_game.Window.ClientBounds.Width - width, 0, width, _game.Window.ClientBounds.Height),
                _font
                //HandleHandlers
            );
            //foreach (MenuItem item in _menuList.MenuItems)
            //{
            //    item.Touch += HandleHandlers(item.Text); 
            //}
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _menuList.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public EventHandler HandleHandlers(string option)
        {
            return option switch
            {
                "New" => (object a, EventArgs e) => {
                    SaveFile.New();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    },
                "Load" => (a, e) => {
                    SaveFile.Load();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                }
                ,
                "Help" => (a, e) => { }
                ,
                "Credit" => (a, e) => { }
                ,
                //StateManager.Instance.Next(new NewGameState());
                _ => (a, e) => { }
                ,
            };
        }

        public override void Update(GameTime gameTime)
        {
            _menuList.Update(gameTime);
        }

        public override void Resized()
        {
            int width = (int)Math.Ceiling(_vW * 0.2);
            _menuList.Rectangle = new Rectangle(_vW - width, 0, width, _vH);
            _menuList.Resized();
        }
    }
}
