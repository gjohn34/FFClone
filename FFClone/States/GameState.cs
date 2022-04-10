using FFClone.Controls;
using FFClone.Sprites;
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
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = content.Load<Texture2D>("Sprites/background");
            _mapRectangle = new Rectangle(0, 0, _background.Width, _background.Height);
            Texture2D pSprite = content.Load<Texture2D>("Sprites/guy");
            int x = (_game.Window.ClientBounds.Width / 2) - (pSprite.Width / 2);
            int y = (_game.Window.ClientBounds.Height / 2) - (pSprite.Height / 2);
            _player = new PlayerSprite(pSprite, 4, 4) { Position = new Vector2(x, y) };
            //{ Rectangle = new Rectangle(100, 100, 50, 50) };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, _mapRectangle, Color.White);
            spriteBatch.End();

            _player.Draw(spriteBatch);

            if (_stack.Count > 0)
            {
                _stack.Peek().Draw(gameTime, spriteBatch);
            }
            //Primitives2D.DrawRectangle(spriteBatch, new Rectangle(0, 0, 200, 200), Color.Black);
        }

        public override void Resized()
        {
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (_stack.Count > 0)
            {

                if (keyboardState.IsKeyDown(Keys.Escape)){
                    _stack.Pop();
                    return;
                }
                _stack.Peek().Update(gameTime);
                return;
            }
            bool playing = true;
            Vector2 velocity = Vector2.Zero;
            int speed = 4;
            Facing facing = _player.Facing;
            bool charMove = false;
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                int width = (int)Math.Ceiling(_vW* 0.2);
                _stack.Push(new MenuList(new List<string> { "Party", "Items", "Config"}, new Rectangle(_vW - width, 0, width, _vH), _font));
                // open up menu
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (_mapRectangle.Y == 0
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
                if (-_mapRectangle.Y == _mapRectangle.Height - _game.Window.ClientBounds.Height
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
            } else
            {
                _mapRectangle = new Rectangle(_mapRectangle.X - (int)velocity.X, _mapRectangle.Y - (int)velocity.Y, _mapRectangle.Width, _mapRectangle.Height);
            }

            // Stop the player when not pressing any key
            _player.Facing = facing;
            _player.Playing = playing;
            _player.Update(gameTime);
        }
    }
}