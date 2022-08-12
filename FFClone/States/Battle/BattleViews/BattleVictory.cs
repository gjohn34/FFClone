using FFClone.Controls;
using FFClone.Helpers.Keyboard;
using FFClone.Helpers.Shapes;
using FFClone.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.States.Battle.BattleViews
{
    public class LevelUp : IComponent
    {
        private bool _growing = true;
        private SpriteFont _font;
        public Hero Hero
        {
            get
            {
                return _hero;
            }
        }
        private Hero _hero;
        private List<int> _centerPoints;
        private Stopwatch _stopwatch = new Stopwatch();
        private Dictionary<string, string> _stats;
        private Point _position;

        public Rectangle TotalRectangle { get; set; }
        public LevelUp(Hero hero, Dictionary<string, string> oldStats, Rectangle rectangle, SpriteFont spriteFont)
        {
            _font = spriteFont;
            _hero = hero;
            _stats = oldStats;
            TotalRectangle = rectangle;
            _position = new Point(
                TotalRectangle.HorizontalPosition(),
                TotalRectangle.VerticalPosition(5, _font.LineSpacing + 5)
            );
            Rectangle = new Rectangle(
                rectangle.Center.X - (int)(0.1f * rectangle.Width),
                rectangle.Center.Y - (int)(0.1f * rectangle.Height),
                (int)(0.1f * rectangle.Width),
                (int)(0.1f * rectangle.Height)
            );
            _centerPoints = new List<int>
            {
                TotalRectangle.Center.X - (int)(0.5 * _font.MeasureString("Stat Block").X),
            };
        }
        public Rectangle Rectangle { get; set; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(Rectangle, 2, Color.Black, Color.Salmon);
            if (!_growing)
            {
                // Vertical Centering
                // Will refactor after moving to other classes
                int yBuffer = 0;

                int availableSpace = Rectangle.Width;
                int xCenterBuffer = availableSpace / 3;
                spriteBatch.DrawString(_font, "Stat Block", new Vector2(_centerPoints[0], _position.Y + yBuffer), Color.Black);
                yBuffer += _font.LineSpacing;
                List<string> x = new List<string>(){ "STR", "INT", "DEX" };
                x.ForEach(y =>
                {
                    spriteBatch.DrawString(_font, y, new Vector2(_position.X + (x.IndexOf(y) * xCenterBuffer), _position.Y + yBuffer), Color.Black);
                });
                yBuffer += _font.LineSpacing;

                x.ForEach(y =>
                {
                    spriteBatch.DrawString(
                        _font, 
                        _stats[y], 
                        new Vector2(
                            _position.X + (x.IndexOf(y) * xCenterBuffer), 
                            _position.Y + yBuffer
                            ),
                        Color.Black, 
                        0, 
                        Vector2.Zero, 
                        _hero.Stat(y) != _stats[y] ? 1f : 1.5f, SpriteEffects.None, 1);

                });
                // increase by 10 IF bold
                yBuffer += _font.LineSpacing + 10;

                x = new List<string>() { "Level", "HP" };
                x.ForEach(y =>
                {
                    spriteBatch.DrawString(_font, y, new Vector2(_position.X + (x.IndexOf(y) * xCenterBuffer), _position.Y + yBuffer), Color.Black);
                });
                yBuffer += _font.LineSpacing;

                x.ForEach(y =>
                {
                    spriteBatch.DrawString(
                        _font,
                        _stats[y],
                        new Vector2(
                            _position.X + (x.IndexOf(y) * xCenterBuffer),
                            _position.Y + yBuffer
                            ),
                        Color.Black,
                        0,
                        Vector2.Zero,
                        _hero.Stat(y) != _stats[y] ? 1f : 1.5f, SpriteEffects.None, 1);

                });
                // increase by 10 IF bold
                yBuffer += _font.LineSpacing + 10;
            }
        }

        public void Resized()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!_growing && !_stopwatch.IsRunning)
            {
                return;
            }
            if (_stopwatch.IsRunning)
            {
                if (_stopwatch.ElapsedMilliseconds >= 1500)
                {
                    _stats = new Dictionary<string, string>
                    {
                        {"Level",_hero.Level.ToString()},
                        {"STR",_hero.Strength.ToString()},
                        {"INT",_hero.Intelligence.ToString()},
                        {"DEX",_hero.Dexterity.ToString()},
                        {"HP",_hero.MaxHP.ToString()},
                    };
                    _stopwatch.Stop();
                }
                return;
            }
            Rectangle = new Rectangle(
                Rectangle.X - (int)(0.05f * TotalRectangle.Width),
                Rectangle.Y - (int)(0.05f * TotalRectangle.Height),
                Rectangle.Width + (int)(2 * (0.05f * TotalRectangle.Width)),
                Rectangle.Height + (int)(2 * (0.05f * TotalRectangle.Height))
            );
            if (Rectangle.X < TotalRectangle.X)
            {
                Rectangle = TotalRectangle;
                _growing = false;
                _stopwatch.Start();
            }
        }
    }

    internal enum Scene
    {
        Idle,
        AnimatingStart,
        Animating,
        AnimatingEnd
    }

    public class BattleVictory : BattleView
    {
        private enum State
        {
            Gaining,
            Gained,
            Levelled,
            Rewards,
            Done,
        }
        private State _state;
        private Modal _modal;
        private Scene _scene = Scene.Idle;
        private int _remainingExp = 0;
        private int _expGain = 0;
        private List<LevelUp> _levelUps = new List<LevelUp>();
        private List<PartyRow> _partyRows = new List<PartyRow>();
        private List<Dictionary<string, string>> _oldStats = new List<Dictionary<string, string>>();
        private Texture2D _background;
        public BattleVictory(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel) : base(game, graphicsDevice, content, battleModel)
        {
            _background = content.Load<Texture2D>("Sprites/Backgrounds/party-screen");
            _state = State.Gaining;
            int spaceBetweenY = _vH / _party.Count;

            if (spaceBetweenY > (int)(0.25f * _vH))
            {
                spaceBetweenY = (int)(0.33f * _vH);
            }
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));
            _party.ForEach(hero =>
            {
                _oldStats.Add(hero.OldStats);
                int index = _party.IndexOf(hero);
                int initialYPos = index * spaceBetweenY;
                double cellPositionPercentage = (double)index / (double)(_party.Count - 1);
                int pushDown = (int)(cellPositionPercentage * cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                int yPosition = initialYPos - pushDown + pushDown2;
                Texture2D portrait = content.Load<Texture2D>(hero.Portrait);
                _partyRows.Add(new PartyRow(portrait, _font, hero, new Rectangle(0, yPosition, _vW, cellHeight)));
            });
            foreach (Enemy enemy in _enemies)
            {
                _expGain += enemy.ExperienceGain;
            }
            _remainingExp = _expGain;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Rectangle(0, 0, _vW, _vH), Color.White);
            _partyRows.ForEach(x => x.Draw(gameTime, spriteBatch));
            int yOffset = 0;
            spriteBatch.DrawString(_font, "Victory", new Vector2(_vW - _font.MeasureString("Victory").X, yOffset), Color.Black);
            yOffset += _font.LineSpacing;
            if (_state == State.Gained) _levelUps.ForEach(x => x.Draw(gameTime, spriteBatch));
            if (_modal != null)
            {
                _modal.Draw(gameTime, spriteBatch);
            }

        }
        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (_modal != null)
                _modal.Update(gameTime);

            if (ks.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter))
            {
                switch (_state)
                {
                    case State.Gaining:
                        _party.ForEach(hero => hero.IncreaseExperience(_remainingExp));
                        _scene = Scene.AnimatingEnd;
                        _remainingExp = 0;
                        _state = State.Gained;
                        break;
                    case State.Gained:
                        string rewards = "Enemies Dropped:\n";
                        Inventory items = new Inventory();
                        _enemies.ForEach(e =>
                        {
                            rewards += $"{e.Reward.Name}\n";
                            items.Add(e.Reward);
                        });
                        GameInfo.Instance.Inventory.Merge(items);

                        _modal = new Modal(
                            _font,
                            rewards,
                            _game.Window.ClientBounds,
                            new Rectangle(
                                (int)(0.2f * _vW),
                                (int)(0.2f * _vH),
                                (int)(0.6f * _vW),
                                (int)(0.6f * _vH)
                            ),
                            (a, b) => {
                                _modal = null;
                                _state = State.Done;
                            }
                        );
                        _state = State.Rewards;
                        break;
                    case State.Rewards:
                        _modal.Call();
                        break;
                    case State.Done:
                        GameInfo.Instance.EncounterInfo.Reset();
                        _stateManager.Done();
                        break;
                    default:
                        break;
                }
            }
            _previousKeyboard = ks;
            switch (_scene)
            {
                case Scene.Idle:
                    if (_state == State.Gaining)
                    {
                        _scene = Scene.AnimatingStart;
                    } else if (_state == State.Gained)
                    {
                        _levelUps.ForEach(x => x.Update(gameTime));
                    }
                    break;
                case Scene.AnimatingStart:
                    if (_remainingExp <= 0)
                    {
                        _scene = Scene.Idle;
                        _state = State.Gained;
                    } else
                    {
                        _scene = Scene.Animating;
                    }
                    break;
                case Scene.Animating:
                    _party.ForEach(hero => {
                    if (hero.IncreaseExperience(1))
                    {
                        Rectangle x = _partyRows.Find(x => x.Hero == hero).Rectangle;
                        Rectangle r = new Rectangle(
                                    (int)(0.4f * x.Width),
                                    x.Y,
                                    (int)(0.4f * _vW),
                                    x.Height
                                );

                        int index = _levelUps.FindIndex(x => x.Hero == hero);
                            if (index < 0)
                            {
                                _levelUps.Add(
                                    new LevelUp(
                                        hero,
                                        _oldStats[_party.IndexOf(hero)],
                                        r,
                                        _font
                                    )
                                );
                            } else
                            {
                                _levelUps[index] =
                                new LevelUp(
                                    hero,
                                    _oldStats[_party.IndexOf(hero)],
                                    r,
                                    _font
                                );
                            }
                        };
                    });
                    _remainingExp -= 1;
                    _scene = Scene.AnimatingEnd;
                    break;
                case Scene.AnimatingEnd:
                    if (_state == State.Gaining)
                    {
                        _scene = Scene.AnimatingStart;
                    } else
                    {
                        _scene = Scene.Idle;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
