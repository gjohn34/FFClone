using FFClone.Controls;
using FFClone.Helpers.Keyboard;
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
    public class Triangle
    {
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }
        public Vector2 Point3 { get; set; }
        public Triangle(Vector2 point, Point size)
        {

            Point1 = point;
            Point2 = new Vector2(point.X - size.X, point.Y - size.Y);
            Point3 = new Vector2(point.X - size.X, point.Y + size.Y);
        }
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Primitives2D.DrawLine(spriteBatch, Point1, Point2, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point1, Point3, Color.Black);
            Primitives2D.DrawLine(spriteBatch, Point2, Point3, Color.Black);
        }
        public void Update(Vector2 point, Point size)
        {
            Point1 = point;
            Point2 = new Vector2(point.X - size.X, point.Y - size.Y);
            Point3 = new Vector2(point.X - size.X, point.Y + size.Y);

        }
    }
    public class Prompt : IComponent
    {
        private KeyboardState _previous;
        public List<Vector2> Vectors;
        private int _promptOn = 0;
        private Triangle _selector;
        private Battle _battle;
        private Ability _ability;
        public Rectangle Rectangle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Prompt(List<Vector2> vectors, Battle battle, Ability ability)
        {
            _previous = Keyboard.GetState();
            Vectors = vectors;
            _selector = new Triangle(vectors[_promptOn], new Point(35, 12));
            _battle = battle;
            _ability = ability;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            bool changed = false;

            if (keyboard.Released(_previous, Keys.Enter))
            {
                _battle.SetSelected(_promptOn, _ability);
            } else if (keyboard.Released(_previous, Keys.Escape))
            {
                _battle.DisablePrompt();
            } else if (keyboard.Released(_previous, Keys.Down))
            {
                changed = true;
                _promptOn += 1;
                if (_promptOn >= Vectors.Count)
                {
                    _promptOn = 0;
                }
            }
            else if (keyboard.Released(_previous, Keys.Up))
            {
                changed = true;
                _promptOn -= 1;
                if (_promptOn < 0)
                {
                    _promptOn = Vectors.Count - 1;
                }
            }
            if (changed)
            {
                _selector.Update(Vectors[_promptOn], new Point(35, 12));
            }

            _previous = keyboard;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _selector.Draw(gameTime, spriteBatch);
        }
        public void Resized()
        {
            _selector = new Triangle(Vectors[_promptOn], new Point(35, 12));
            //throw new NotImplementedException();
        }
    }

    public class BattleState : State
    {
        private List<Hero> _party;
        private List<Enemy> _enemies;
        private Battle _battle;
        private int _thickness = 10;
        private Rectangle _bottomBar;
        private Rectangle _cBar;
        private Stack<IComponent> _menuStack = new Stack<IComponent>();


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
                new Enemy("Purple Guy", Color.Purple, statBlock) };
            _battle = new Battle(_party, _enemies, game, graphicsDevice, content);
            // TODO - This refactor this w/ resize
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);
            _bottomBar = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            _cBar = new Rectangle(_thickness, menuYPos, commandWidth - _thickness, height - 2 * _thickness - 1);
            MenuList commandMenu = new MenuList(new List<string> { "Attack", "Defend", "Spell" }, _cBar, _font);
            foreach (MenuItem item in commandMenu.MenuItems)
            {
                item.Touch += HandleHandlers(item.Text);
            }
            _menuStack.Push(commandMenu);
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
                },
                "Spell" => (object a, EventArgs e) => Debug.WriteLine("Spell"),
                _ => (a, e) => { },
            };
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

            // commands
            int commandWidth = (int)(_vW * 0.35);
            int menuYPos = _vH - height + thickness;
            int availableSpace = _vW - thickness - commandWidth + 3;

            //_command.Draw(gameTime, spriteBatch);
            if (_menuStack.TryPeek(out _))
            {
                foreach (IComponent item in _menuStack)
                {
                    item.Draw(gameTime, spriteBatch);
                }
            }
            //spriteBatch.DrawRectangle(new Rectangle(thickness, menuYPos, commandWidth - thickness, height - 2 * thickness - 1), Color.Yellow);
            int charInfoWidth = (int)(availableSpace * 0.33);

            // offset by 1 for border pixel
            int xOffset = commandWidth + 1;
            double yOffset = 0.1;

            _battle.Draw(gameTime, spriteBatch);

            foreach (Hero hero in _party)
            {
                Vector2 v = new Vector2(xOffset, menuYPos);
                // player sprite
                //spriteBatch.DrawRectangleWithFill(new Rectangle((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset), 50, 50), 1, Color.Black, hero.Color);
                yOffset += 0.2;
                // command box
                spriteBatch.DrawRectangleWithFill(new Rectangle(xOffset, menuYPos, charInfoWidth, height - 2 * thickness - 1), 1, Color.Black, hero.Color);
                spriteBatch.DrawString(_font, hero.Name, v, Color.White);
                spriteBatch.DrawString(_font, $"{hero.HP} / {hero.MaxHP}", new Vector2(xOffset, menuYPos + (int)_font.LineSpacing), Color.White);

                // offset by 2 for box and border pixels
                xOffset += charInfoWidth + 1;
            }

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
                        if (_menuStack.TryPeek(out _))
                        {
                            _menuStack.Peek().Update(gameTime);
                        }
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
            int height = (int)(_vH * 0.3);
            int menuYPos = _vH - height + _thickness;
            int commandWidth = (int)(_vW * 0.35);
            _bottomBar = new Rectangle(_thickness, _vH - height, _vW - _thickness, height - _thickness);
            _cBar = new Rectangle(_thickness, menuYPos, commandWidth - _thickness, height - 2 * _thickness - 1);
            IComponent[] x = _menuStack.ToArray();
            x[0].Rectangle = _cBar;
            foreach (IComponent component in _menuStack)
            {
                component.Resized();
            }
            _battle.Resized();
            //_command.Rectangle = _cBar;
            //_command.Resized();
        }
    }
}