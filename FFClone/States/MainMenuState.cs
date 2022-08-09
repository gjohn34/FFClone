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
        private Texture2D _background;

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
                new MenuOption("Dummy Party", (a, e) => {
                    SaveFile.LoadDummy();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    }),
                new MenuOption("Delete Save", (a, e) => { SaveFile.Destroy(); }),
                new MenuOption("Help", (a, e) => { }),
                new MenuOption("Credit To", (a,e) => {
                    _stateManager.Next(new CreditsState(_game, _graphicsDevice, _content), Transition.NoTransition);
                })
            };

            int width = (int)Math.Ceiling(_game.Window.ClientBounds.Width * 0.2);
            List<IMenuOption> list = new List<IMenuOption>();
            for (int i = 0; i < options.Count; i++)
            {
                list.Add(new MenuItem(
                    options[i].Label,
                    _font,
                    options[i].OnSubmit
                ));
            }

            _menuList = new MenuList(
                list,
                new Rectangle(_game.Window.ClientBounds.Width - width, 0, width, _game.Window.ClientBounds.Height),
                _font
                //HandleHandlers
            );

            _background = content.Load<Texture2D>("Sprites/Backgrounds/main-menu");
            //foreach (MenuItem item in _menuList.MenuItems)
            //{
            //    item.Touch += HandleHandlers(item.Text); 
            //}
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Rectangle(0, 0, _vW, _vH), Color.White);
            _menuList.Draw(gameTime, spriteBatch);
            spriteBatch.End();
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
