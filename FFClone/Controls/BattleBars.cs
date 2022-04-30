using FFClone.Helpers.Keyboard;
using FFClone.Helpers.Shapes;
using FFClone.Models;
using FFClone.States;
using FFClone.States.Battle.BattleViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private MenuList _commandMenu;
        private BattleMain _battle;
        private KeyboardState _previousKeyboard;


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
            _previousKeyboard = Keyboard.GetState();

            // TODO - This refactor this w/ resize
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);

            _commandMenu = new MenuList(
                _battle.Current.Options,
                new Rectangle(rectangle.X, menuYPos, commandWidth - rectangle.X, rectangle.Height - rectangle.X - 1), 
                _font, 
                HandleHandlers
            );
            //_menuStack.Push(commandMenu);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.Released(_previousKeyboard, Keys.Escape))
                _menuStack.TryPop(out _);
            
            if (_menuStack.TryPeek(out _))
                _menuStack.Peek().Update(gameTime);
            else
                _commandMenu.Update(gameTime);

            _previousKeyboard = ks;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // bottom bar
            spriteBatch.DrawRectangleWithFill(Rectangle, _thickness, Color.Yellow, Color.CadetBlue);
            _commandMenu.Draw(gameTime, spriteBatch);

            // commands
            int menuYPos = _vH - (int)(_vH * 0.3) + Rectangle.X;
            int availableSpace = _vW -  _commandMenu.Rectangle.Width - _thickness;


            int charInfoWidth = (int)(availableSpace / 3);

            // offset by 1 for border pixel
            int xOffset = _commandMenu.Rectangle.Width + 1;
            double yOffset = 0.1;


            foreach (Hero hero in _party)
            {
                Vector2 v = new Vector2(xOffset, menuYPos);
                yOffset += 0.2;
                
                // command box
                spriteBatch.DrawRectangleWithFill(new Rectangle(xOffset, menuYPos, charInfoWidth, Rectangle.Height - Rectangle.X), 1, Color.Black, hero.Color);
                spriteBatch.DrawString(_font, hero.Name, v, Color.White);
                spriteBatch.DrawString(_font, $"{hero.HP} / {hero.MaxHP}", new Vector2(xOffset, menuYPos + (int)_font.LineSpacing), Color.White);

                // offset by 2 for box and border pixels
                xOffset += charInfoWidth + 1;
            }
            IComponent[] x = _menuStack.ToArray();
            for (int i = x.Length - 1; i >= 0; i--)
                x[i].Draw(gameTime, spriteBatch);

        }

        public void Resized()
        {
            _vH = _game.Window.ClientBounds.Height;
            _vW = _game.Window.ClientBounds.Width;
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            // Full Bar
            Rectangle = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            // option bar
            _commandMenu.Rectangle = new Rectangle(Rectangle.X, menuYPos, (int)(_vW * 0.35) - Rectangle.X, height - (2 * Rectangle.X) - 1);
            _commandMenu.Resized();
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
                        new Rectangle(0,0, _vW, _vH),
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
            _commandMenu = new MenuList(
                _battle.Current.Options,
                new Rectangle(Rectangle.X, _vH - (int)(_vH * 0.3) + _thickness, (int)(_vW * 0.35) - Rectangle.X, (int)(_vH * 0.3) - (2 * Rectangle.X) - 1),
                _font,
                HandleHandlers
            );
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