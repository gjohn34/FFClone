using FFClone.Helpers.Shapes;
using FFClone.Models;
using FFClone.States;
using FFClone.States.Battle.BattleViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Action = FFClone.Models.Action;

namespace FFClone.Controls
{
    public class BattleBars : IComponent
    {
        private Game1 _game;
        private List<Hero> _party;
        private List<Enemy> _enemies;
        private SpriteFont _font;
        private int _thickness;
        private int _vH;
        private int _vW;
        private Stack<IComponent> _menuStack = new Stack<IComponent>();
        private Rectangle _cBar;
        private BattleMain _battle;

        public Rectangle Rectangle { get; set; }
        public BattleBars(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, List<Hero> party, List<Enemy> enemies, BattleMain battle, Rectangle rectangle)
        {
            _game = game;
            _party = party;
            _enemies = enemies;
            _battle = battle;
            _font = content.Load<SpriteFont>("Font/font");
            _vH = game.Window.ClientBounds.Height;
            _vW = game.Window.ClientBounds.Width;
            Rectangle = rectangle;
            _thickness = rectangle.X;

            // TODO - This refactor this w/ resize
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);

            _cBar = new Rectangle(rectangle.X, menuYPos, commandWidth - rectangle.X, rectangle.Height - rectangle.X - 1);
            MenuList commandMenu = new MenuList(
                _battle.Current.Options,
                _cBar, 
                _font, 
                HandleHandlers
            );
            _menuStack.Push(commandMenu);
        }

        public void Update(GameTime gameTime)
        {

            if (_menuStack.TryPeek(out _))
            {
                _menuStack.Peek().Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // bottom bar
            spriteBatch.DrawRectangleWithFill(Rectangle, _thickness, Color.Black, Color.CadetBlue);

            // commands
            int commandWidth = (int)(_vW * 0.35);
            int menuYPos = _vH - (int)(_vH * 0.3) + Rectangle.X;
            int availableSpace = _vW - Rectangle.X - commandWidth + 3;

            if (_menuStack.TryPeek(out _))
            {
                foreach (IComponent item in _menuStack)
                {
                    item.Draw(gameTime, spriteBatch);
                }
            }
            int charInfoWidth = (int)(availableSpace * 0.33);

            // offset by 1 for border pixel
            int xOffset = commandWidth + 1;
            double yOffset = 0.1;


            foreach (Hero hero in _party)
            {
                Vector2 v = new Vector2(xOffset, menuYPos);
                yOffset += 0.2;
                
                // command box
                spriteBatch.DrawRectangleWithFill(new Rectangle(xOffset, menuYPos, charInfoWidth, Rectangle.Height - Rectangle.X - 1), 1, Color.Black, hero.Color);
                spriteBatch.DrawString(_font, hero.Name, v, Color.White);
                spriteBatch.DrawString(_font, $"{hero.HP} / {hero.MaxHP}", new Vector2(xOffset, menuYPos + (int)_font.LineSpacing), Color.White);

                // offset by 2 for box and border pixels
                xOffset += charInfoWidth + 1;
            }
        }

        public void Resized()
        {
            _vH = _game.Window.ClientBounds.Height;
            _vW = _game.Window.ClientBounds.Width;
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            Rectangle = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            _cBar = new Rectangle(Rectangle.X, menuYPos, (int)(_vW * 0.35) - Rectangle.X, height - Rectangle.X - 1);
            IComponent[] x = _menuStack.ToArray();
            x[0].Rectangle = _cBar;
            foreach (IComponent component in _menuStack)
            {
                component.Resized();
            }
        }

        public EventHandler HandleHandlers(string option)
        {
            return option switch
            {
                "Attack" => (object a, EventArgs e) =>
                {
                    _battle.EnablePrompt(GetVectors(), new Action("Attack"));
                },
                "Defend" => (object a, EventArgs e) =>
                {
                    _battle.Defend();
                },
                "Spell" => (object a, EventArgs e) => {
                    _menuStack.Push(new MenuList(
                        _battle.Current.Spells,
                        new Rectangle(0, 0, 100, 100),
                        _font,
                        (string x) => { return (a, b) => { _battle.EnablePrompt(GetVectors(), new Spell(x)); }; }
                    )
                    );
                },
                _ => (a, e) => { Debug.WriteLine("Unhandled"); },
            };
        }

        internal void NewMenu(IBattleable current)
        {
            _menuStack.Clear();
            MenuList menu = new MenuList(
                _battle.Current.Options,
                _cBar,
                _font,
                HandleHandlers
            );
            _menuStack.Push(menu);
        }

        internal List<IBattleable> GetVectors()
        {

            List<IBattleable> options = new List<IBattleable>();
            foreach (Enemy enemy in _enemies)
            {
                if (enemy.HP > 0)
                {
                    options.Add(enemy);
                }
            }
            return options;
        }
    }
}