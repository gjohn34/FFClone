using FFClone.Controls;
using FFClone.Helpers.Shapes;
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
        private decimal _foo = 0;
        internal MenuList menuList { 
            get
            {
                List<IMenuOption> options = new List<IMenuOption>
                {
                    new MenuItem("Party", _font, (object a, EventArgs e) => _stateManager.Next(new PartyMenuState(_game, _graphicsDevice, _content), Transition.NoTransition)),
                    new MenuItem("Items",  _font,(object a, EventArgs e) => _stateManager.Next(new ItemMenuState(_game, _graphicsDevice, _content), Transition.NoTransition)),
                    new MenuItem("Config", _font,(a, e) => { }),
                    new MenuItem("Main Menu", _font,(a, e) => { _stateManager.Next(new MainMenuState(_game, _graphicsDevice, _content), Transition.NoTransition); }),
                    new MenuItem("Save",  _font,(object a, EventArgs e) => {
                        string label = "Overwrite existing save data?";
                        List<IMenuOption> list = new List<IMenuOption>
                        {
                            new MenuItem("yes", _font, (a,b) => {
                                Debug.WriteLine("Do save stuff");
                                SaveFile.Save();
                                _stack.Pop();
                                }),
                            new MenuItem("no",_font, (a,b) =>
                                {
                                    Debug.WriteLine("close modal stuff");
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
                    })
                };

                int width = (int)Math.Ceiling(_vW * 0.2);
               
                return new MenuList(
                    options,
                    new Rectangle(_vW - width, 0, width, _vH),
                    _font
                );
            } 
        }
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _encounterInfo = GameInfo.Instance.EncounterInfo;
            _background = content.Load<Texture2D>("Sprites/Backgrounds/game-background");
            _mapRectangle = new Rectangle((int)_encounterInfo.MapPosition.X, (int)_encounterInfo.MapPosition.Y, _background.Width, _background.Height);
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
            spriteBatch.DrawString(_font, "Encounter Chance", Vector2.Zero, Color.White);
            spriteBatch.DrawRectangleWithFill(new Rectangle(0, _font.LineSpacing, 30, 20), 0, Color.Black, Color.LightGreen);
            spriteBatch.DrawRectangleWithFill(new Rectangle(30, _font.LineSpacing, 30, 20), 0, Color.Black, Color.YellowGreen);
            spriteBatch.DrawRectangleWithFill(new Rectangle(60, _font.LineSpacing, 25, 20), 0, Color.Black, Color.Orange);
            spriteBatch.DrawRectangleWithFill(new Rectangle(85, _font.LineSpacing, 15, 20), 0, Color.Black, Color.OrangeRed);
            //spriteBatch.ProgressBar(_font, "Encounter Chance", foo, 1, ,,,false);
            
            //spriteBatch.ProgressBar(_font, "Encounter Chance", foo,, , ,,,false);
            spriteBatch.ProgressBar(_font, "Encounter Chance", (int)Math.Floor(_encounterInfo.Ticks), 100, Color.Purple, Color.Transparent, new Rectangle(0, 0, 100, 20), false);


            spriteBatch.End();
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(0, 0, 200, 200), Color.Black);
        }
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (_stack.Count > 0)
            {
                _stack.Peek().Update(gameTime);
                if (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboard.IsKeyUp(Keys.Escape)){
                    _stack.Pop();
                    _previousKeyboard = keyboardState;
                    return;
                }
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
            
            if (velocity != Vector2.Zero)
            {
                _encounterInfo.MapPosition = new Vector2(_mapRectangle.X, _mapRectangle.Y);
            }

            if (generateEnc)
            {
                _encounterInfo.Ticks += 0.3;

                if (_encounterInfo.Ticks > 30 && _encounterInfo.Ticks < 60)
                {
                    _encounterInfo.Chance = _rand.Next(0, 90) + (0.3f * (int)Math.Floor(_encounterInfo.Ticks));
                }
                else if (_encounterInfo.Ticks > 60 && _encounterInfo.Ticks < 85)
                {
                    _encounterInfo.Chance = _rand.Next(0, 85) + (0.5f * (int)Math.Floor(_encounterInfo.Ticks));

                } else if (_encounterInfo.Ticks > 85)
                {
                    _encounterInfo.Chance = _rand.Next(15, 100) + (int)Math.Floor(_encounterInfo.Ticks);

                }
                if (_encounterInfo.Chance > 100)
                {
                    _encounterInfo.Ticks = 0;
                    _encounterInfo.Chance = 0;
                    StateManager.Instance.Next(new BattleState(_game, _graphicsDevice, _content, this), Transition.NoTransition);

                }

            }

            _previousKeyboardState = keyboardState;
            // Stop the player when not pressing any key
            _player.Facing = facing;
            _player.Playing = playing;
            _player.Update(gameTime);
        }
    }
}