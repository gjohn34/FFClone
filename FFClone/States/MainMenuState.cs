using FFClone.Controls;
using FFClone.Models;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace FFClone.States
{
    public class MainMenuState : State
    {
        private MenuList _menuList;
        private Texture2D _background;
        private Stack<IComponent> _stack = new Stack<IComponent>();

        public MainMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            List<IMenuOption> options = new List<IMenuOption> {
                new MenuOption("New", (object a, EventArgs e) => {
                    SaveFile.New();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    }),
                new MenuOption("Load", (a, e) => {
                    try
                    {
                        SaveFile.Load();
                        _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    } catch {
                        string label = "No save data found. Start new game?";
                        List<IMenuOption> list = new List<IMenuOption>
                        {
                            new MenuItem("yes", _font, (a,b) => {
                                SaveFile.New();
                                _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                                _stack.Pop();
                                }),
                            new MenuItem("no",_font, (a,b) =>
                                {
                                    _stack.Pop();
                                })
                        };

                        Rectangle r = new Rectangle((int)(0.3f * _vW), (int)(0.3f * _vH), (int)(0.3f * _vW), (int)(0.3f * _vH));

                        Rectangle y = new Rectangle(
                            r.X,
                            r.Y + (int)(_font.MeasureString(label).Y),
                            r.Width,
                            (int)(r.Height - _font.MeasureString(label).Y)
                        );

                        MenuList menulist = new MenuList(list, y, _font, Orientation.Horizontal, list.Count);
                        _stack.Push(new ConfirmationModal(
                            _font,
                            label,
                            menulist,
                            _game.Window.ClientBounds
                        ));
                    }
                }),
                new MenuOption("Dummy Party", (a, e) => {
                    SaveFile.LoadDummy();
                    _stateManager.Next(new GameState(_game, _graphicsDevice, _content), Transition.NoTransition);
                    }),
                new MenuOption("Delete Save", (a, e) => { SaveFile.Destroy(); }),
                new MenuOption("Help", (a, e) => {
                    string label= "Hello there:\nControls are arrows keys for selections\nGray is the selected option\nEnter key to confirm selction\nEscape to cancel/close modal\n\nRight-shift opens up in-game menu\n\nDummy party has a full party";
                        _stack.Push(new Modal(
                            _font,
                            label,
                            _game.Window.ClientBounds,
                            new Rectangle(
                                (int)(0.2f * _vW),
                                (int)(0.2f * _vH),
                                (int)(0.6f * _vW),
                                (int)(0.6f * _vH)
                            ),
                            (a, b) => {
                                _stack.Pop();
                            }
                        ));
                }),
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
            if (_stack.Count > 0)
            {
                _stack.Peek().Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (_stack.Count > 0)
            {
                _stack.Peek().Update(gameTime);
                if (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboard.IsKeyUp(Keys.Escape))
                {
                    _stack.Pop();
                    _previousKeyboard = keyboardState;
                    return;
                }
                _previousKeyboard = keyboardState;
                return;
            }
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
