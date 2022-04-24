using FFClone.Controls;
using FFClone.Helpers.Shapes;
using FFClone.Models;
using FFClone.Sprites;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.States
{
    public class BattleBars
    {
        private Game1 _game;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private List<Hero> _party;
        private List<Enemy> _enemies;
        private SpriteFont _font;
        private int _thickness;
        private int _vH;
        private int _vW;
        private Stack<IComponent> _menuStack = new Stack<IComponent>();
        private Rectangle _cBar;
        private Battle _battle;
        public Rectangle Rectangle;
        public BattleBars(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, List<Hero> party, List<Enemy> enemies, Battle battle, Rectangle rectangle)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
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
            MenuList commandMenu = new MenuList(new List<string> { "Attack", "Defend", "Spell" }, _cBar, _font);
            foreach (MenuItem item in commandMenu.MenuItems)
            {
                item.Touch += HandleHandlers(item.Text);
            }
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

        internal void Resized()
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
                    List<Vector2> vectors = new List<Vector2>();
                    foreach (Enemy enemy in _enemies)
                    {
                        vectors.Add(new Vector2(enemy.BattleSprite.Position.X, enemy.BattleSprite.Position.Y + (int)(0.5 * enemy.BattleSprite.Height)));
                        //vectors.Add(new Vector2((int)(_vW * 0.20) - 25, (int)(_vH * yOffset) + 25));
                        //yOffset += 0.2;
                    }
                    _battle.EnablePrompt(vectors, new Ability("Attack"));
                    // hardcoded 25's to center. need to fix that
                    //_menuStack.Push();
                }
                ,
                "Defend" => (object a, EventArgs e) =>
                {
                    _battle.Defend();
                }
                ,
                "Spell" => (object a, EventArgs e) => Debug.WriteLine("Spell"),
                _ => (a, e) => { }
                ,
            };
        }
    }
    public class BattleState : State
    {
        private List<Hero> _party;
        private List<Enemy> _enemies;
        private Battle _battle;
        private BattleBars _battleBar;

        // TODO make thickness reactive
        private int _thickness = 10;


        public BattleState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _party = GameInfo.Instance.Party;

            Dictionary<string, int> statBlock = new Dictionary<string, int>()
            {
                {
                    "HP",3
                },
                {
                    "MaxHP", 3
                },
                {
                    "STR", 10
                },
                {
                    "INT", 10
                },
                {
                    "DEX", 10
                },
            };
            _enemies = new List<Enemy> { 
                new Enemy("Purple Guy", Color.Purple, statBlock) 
            }
            ;
            _battle = new Battle(_party, _enemies, game, graphicsDevice, content);
            
            Rectangle rectangle = new Rectangle(_thickness, _vH - (int)(_vH * 0.3), _vW - _thickness, (int)(_vH * 0.3) - _thickness);
            _battleBar = new BattleBars(game, graphicsDevice, content, _party, _enemies, _battle, rectangle);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int width = _vW / 2 - (int)(_font.MeasureString("battlemode").X / 2);
            spriteBatch.DrawString(_font, "battlemode", new Vector2(width, 0), Color.White);
            _battle.Draw(gameTime, spriteBatch);
            _battleBar.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (_battle.NextRound)
            {
                foreach (Enemy enemy in _enemies)
                {
                    _battle.RoundActions.Add(new Models.Action(enemy, _party[0], new Ability("attack")));
                }

                // calculate damages;
                // update displays
                _battle.BattleScene = BattleScene.AnimatingStart;
            }
            if (_battle.BattleOver)
            {
                if (_battle.PartyDefeated)
                {

                }
                //_battle.Update(gameTime);
            }
            switch (_battle.BattleScene)
            {
                case BattleScene.Idle:
                    if (_battle.HasPrompt)
                    {
                        _battle.Prompt.Update(gameTime);
                    }
                    else
                    {
                        _battleBar.Update(gameTime);
                    }
                    break;
                case BattleScene.AnimatingStart:
                case BattleScene.Animating:
                    break;
                case BattleScene.AnimatingEnd:
                    _battle.Target.HP -= _battle.Current.CalculateBattleDamage();
                    break;
                default:
                    break;
            }
            _battle.Update(gameTime);
        }

        public override void Resized()
        {
            base.Resized();
            _battleBar.Resized();
            _battle.Resized();
            //_command.Rectangle = _cBar;
            //_command.Resized();
        }
    }
}