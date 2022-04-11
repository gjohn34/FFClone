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
    public class BattleState : State
    {
        private List<Hero> _party;
        private int _thickness = 10;
        private Rectangle _bottomBar;
        private Rectangle _cBar;
        private MenuList _command;

        public BattleState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _party = GameInfo.Instance.Party;
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);
            _bottomBar = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            _cBar = new Rectangle(_thickness, menuYPos, commandWidth - _thickness, height - 2 * _thickness - 1);
            _command = new MenuList(new List<string> { "Attack", "Defend", "Spell" }, _cBar, _font);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int width = _vW / 2 - (int)(_font.MeasureString("battlemode").X / 2);
            spriteBatch.DrawString(_font, "battlemode", new Vector2(width, 0), Color.White);
            int height = (int)(_vH * 0.3);
            int thickness = 10;

            // bottom bar
            spriteBatch.DrawRectangleWithFill(_bottomBar, _thickness, Color.Black, Color.CadetBlue);

            // heroes
            {
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.10), 50, 50), 1, Color.Black, Color.Red);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.20) + 50, 50, 50), 1, Color.Black, Color.Blue);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * 0.30) + 100, 50, 50), 1, Color.Black, Color.Green);

            // enemies
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.10), 50, 50), 1, Color.Black, Color.Purple);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.20) + 50, 50, 50), 1, Color.Black, Color.Purple);
            spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW * 0.20), (int)(_vH * 0.30) + 100, 50, 50), 1, Color.Black, Color.Purple);
            }

            // commands
            int commandWidth = (int)(_vW * 0.35);
            int menuYPos = _vH - height + thickness;
            int availableSpace = _vW - thickness - commandWidth + 3;
            spriteBatch.End();

            _command.Draw(gameTime, spriteBatch);
            
            spriteBatch.Begin();

            //spriteBatch.DrawRectangle(new Rectangle(thickness, menuYPos, commandWidth - thickness, height - 2 * thickness - 1), Color.Yellow);
            int charInfoWidth = (int)(availableSpace * 0.33);

            // offset by 1 for border pixel
            int xOffset = commandWidth + 1;
            Stack<Color> colors = new Stack<Color>();
            colors.Push(Color.Red);
            colors.Push(Color.Blue);
            colors.Push(Color.Green);

            foreach (Hero hero in _party)
            {
                Vector2 v = new Vector2(xOffset, menuYPos);
                spriteBatch.DrawRectangleWithFill(new Rectangle(xOffset, menuYPos, charInfoWidth, height - 2 * thickness - 1), 1, Color.Black, colors.Pop());
                spriteBatch.DrawString(_font, hero.Name, v, Color.White);
                spriteBatch.DrawString(_font, $"{hero.HP} / {hero.MaxHP}", new Vector2(xOffset, menuYPos + (int)_font.LineSpacing), Color.White);
                // offset by 2 for box and border pixels
                xOffset += charInfoWidth + 1;
            }




            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            _command.Update(gameTime);
        }

        public override void Resized()
        {
            base.Resized();
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);
            _bottomBar = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            _cBar = new Rectangle(_thickness, menuYPos, commandWidth - _thickness, height - 2 * _thickness - 1);
            _command.Rectangle = _cBar;
            _command.Resized();
        }
    }
}