using FFClone.Controls;
using FFClone.Models;
using FFClone.Sprites;
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
    public class GameState : State
    {
        private Stack<IComponent> _stack = new Stack<IComponent>();
        private Texture2D _background;
        private Rectangle _mapRectangle;
        private PlayerSprite _player;
        private KeyboardState _previousKeyboardState;
        private EncounterInfo _encounterInfo;
        private Random _rand = new Random();
        internal MenuList menuList { get
            {
                int width = (int)Math.Ceiling(_vW * 0.2);
                return new MenuList(
                    new List<string> { "Party", "Items", "Config", "Save" },
                    new Rectangle(_vW - width, 0, width, _vH),
                    _font,
                    HandleHandlers
                );
            } 
        }
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _encounterInfo = GameInfo.Instance.EncounterInfo;
            _background = content.Load<Texture2D>("Sprites/background");
            _mapRectangle = new Rectangle(0, 0, _background.Width, _background.Height);
            Texture2D pSprite = content.Load<Texture2D>("Sprites/guy");
            // TODO - NEED A BETTER WAY THAN THIS
            if (_encounterInfo.Position.Equals(Vector2.Zero))
            {
                _encounterInfo.Position = new Vector2(
                    (_game.Window.ClientBounds.Width / 2) - (pSprite.Width / 2),
                    (_game.Window.ClientBounds.Height / 2) - (pSprite.Height / 2)
                );
            }
            _player = new PlayerSprite(pSprite, 4, 4) { Position = _encounterInfo.Position };
            _previousKeyboardState = Keyboard.GetState();
            _stateManager.SetMain(this);
            //{ Rectangle = new Rectangle(100, 100, 50, 50) };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, _mapRectangle, Color.White);

            _player.Draw(spriteBatch);
            if (_stack.Count > 0)
            {
                _stack.Peek().Draw(gameTime, spriteBatch);
            }
            spriteBatch.DrawString(_font, _encounterInfo.Chance.ToString(), Vector2.Zero, Color.White);

            spriteBatch.End();
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(0, 0, 200, 200), Color.Black);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (_stack.Count > 0)
            {
                if (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboard.IsKeyUp(Keys.Escape)){
                    _stack.Pop();
                    _previousKeyboard = keyboardState;
                    return;
                }
                _stack.Peek().Update(gameTime);
                _previousKeyboard = keyboardState;
                return;
            }

            bool playing = true;
            Vector2 velocity = Vector2.Zero;
            int speed = 4;
            Facing facing = _player.Facing;
            bool charMove = false;
            bool generateEnc = false;
            if (keyboardState.IsKeyUp(Keys.Enter) && _previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                _stack.Push(menuList);
                // open up menu
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                generateEnc = true;
                if (_mapRectangle.Y >= 0
                    ||
                    _player.Position.Y > (_game.Window.ClientBounds.Height / 2) - (_player.Height / 2))
                {
                    charMove = true;
                    // move char up instead
                }
                velocity.Y = -speed;
                facing = Facing.Up;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                generateEnc = true;

                if (-_mapRectangle.Y >= _mapRectangle.Height - _game.Window.ClientBounds.Height
                    ||
                    _player.Position.Y < (_game.Window.ClientBounds.Height / 2) - (_player.Height / 2))
                {
                    charMove = true;
                    // move char down
                }
                velocity.Y = speed;
                facing = Facing.Down;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                generateEnc = true;

                if (_mapRectangle.X == 0
                    ||
                    _player.Position.X > (_game.Window.ClientBounds.Width / 2) - (_player.Width/ 2))

                {
                    charMove = true;
                    // move char left
                }
                velocity.X = -speed;
                facing = Facing.Left;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                generateEnc = true;

                if (-_mapRectangle.X == _mapRectangle.Width - _game.Window.ClientBounds.Width
                    ||
                    _player.Position.X < (_game.Window.ClientBounds.Width / 2) - (_player.Width/ 2))
                {
                    charMove = true;
                    // move char right
                }
                velocity.X = speed;
                facing = Facing.Right;
            }
            else
            {
                playing = false;
            }

            // Update position
            if (charMove)
            {

                Rectangle newPos = new Rectangle((int)(_player.Position.X + velocity.X + _player.Width), (int)(_player.Position.Y + velocity.Y + _player.Height), -_player.Width, -_player.Height);
                if (_mapRectangle.Intersects(newPos))
                {
                    _player.Position += velocity;
                }
            }
            else
            {
                _mapRectangle = new Rectangle(_mapRectangle.X - (int)velocity.X, _mapRectangle.Y - (int)velocity.Y, _mapRectangle.Width, _mapRectangle.Height);
            }
            if (generateEnc)
            {
                _encounterInfo.Ticks += 0.02;

                _encounterInfo.Chance = _rand.NextDouble() + _encounterInfo.Ticks;
                speed = 1;
                if (_encounterInfo.Chance > speed)
                {
                    _encounterInfo.Ticks = 0;
                    StateManager.Instance.Next(new BattleState(_game, _graphicsDevice, _content, this), Transition.NoTransition);

                }

            }

            _previousKeyboardState = keyboardState;
            // Stop the player when not pressing any key
            _player.Facing = facing;
            _player.Playing = playing;
            _player.Update(gameTime);
        }
        public EventHandler HandleHandlers(string option)
        {
            return option switch
            {
                "Party" => (object a, EventArgs e) => {
                    _stack.Pop();
                    _stateManager.Next(new PartyMenuState(_game, _graphicsDevice, _content, this), Transition.NoTransition);
                },
                "Items" => (object a, EventArgs e) => _stateManager.Next(new ItemMenuState(_game, _graphicsDevice, _content), Transition.NoTransition),
                "Config" => (a, e) => { },
                "Save" => (object a, EventArgs e) => {
                    _stack.Push(new ConfirmationModal(
                        _font,
                        "Overwrite existing save data?",
                        "yes",
                        "no",
                        _game.Window.ClientBounds,
                        (string option) =>
                        {
                            return (a, b) => {
                                if (option == "yes")
                                {
                                    Debug.WriteLine("Do save stuff");
                                    SaveFile.Save();
                                    _stack.Pop();
                                }
                                else
                                {
                                    Debug.WriteLine("close modal stuff");
                                    _stack.Pop();

                                }
                            };
                        }
                    ));
                },
                _ => (a, e) => { }
            };
        }
    }

}