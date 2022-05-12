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
        private Hero _hero;
        private List<int> _centerPoints;
        private Stopwatch _stopwatch = new Stopwatch();
        private Dictionary<string, string> _stats;

        public Rectangle TotalRectangle { get; set; }
        public LevelUp(Hero hero, Dictionary<string, string> oldStats, Rectangle rectangle, SpriteFont spriteFont)
        {
            _font = spriteFont;
            _hero = hero;
            _stats = oldStats;
            TotalRectangle = rectangle;
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
                int yPosition = Rectangle.Y;
                int height = Rectangle.Height;
                int rows = 5;
                int rowHeight = _font.LineSpacing + 5;
                int spaceTaken = rows * rowHeight;
                int availableHeight = height - spaceTaken;
                int yMargin = availableHeight / rows;

                int availableSpace = Rectangle.Width;
                int xCenterBuffer = availableSpace / 3;
                int leftMargin = Rectangle.X + (int)(0.5 * xCenterBuffer);

                spriteBatch.DrawString(_font, "Stat Block", new Vector2(_centerPoints[0], yPosition + yMargin), Color.Black);
                yPosition += _font.LineSpacing + 5;

                spriteBatch.DrawString(_font, "STR", new Vector2(leftMargin, yPosition + yMargin), Color.Black);
                spriteBatch.DrawString(_font, "INT", new Vector2(leftMargin + xCenterBuffer, yPosition + yMargin), Color.Black);
                spriteBatch.DrawString(_font, "DEX", new Vector2(leftMargin + xCenterBuffer + xCenterBuffer, yPosition + yMargin), Color.Black);
                yPosition += _font.LineSpacing + 5;


                spriteBatch.DrawString(_font, _stats["STR"], new Vector2(leftMargin, yPosition + yMargin), Color.Black, 0,Vector2.Zero, _hero.Strength.ToString() != _stats["STR"] ? 1f : 1.5f,SpriteEffects.None,1);
                spriteBatch.DrawString(_font, _stats["INT"], new Vector2(leftMargin + xCenterBuffer, yPosition + yMargin), Color.Black, 0,Vector2.Zero, _hero.Intelligence.ToString() != _stats["INT"] ? 1f : 1.5f,SpriteEffects.None,1);
                spriteBatch.DrawString(_font, _stats["DEX"], new Vector2(leftMargin + xCenterBuffer + xCenterBuffer, yPosition + yMargin), Color.Black, 0,Vector2.Zero, _hero.Dexterity.ToString() != _stats["DEX"] ? 1f : 1.5f,SpriteEffects.None,1);

                yPosition += _font.LineSpacing + 10;
                spriteBatch.DrawString(_font, "Level", new Vector2(leftMargin, yPosition + yMargin), Color.Black);
                spriteBatch.DrawString(_font, "HP", new Vector2(leftMargin + xCenterBuffer, yPosition + yMargin), Color.Black);
                yPosition += _font.LineSpacing + 5;


                spriteBatch.DrawString(_font, _stats["Level"], new Vector2(leftMargin, yPosition + yMargin), Color.Black, 0,Vector2.Zero, _hero.Level.ToString() != _stats["Level"] ? 1f : 1.5f,SpriteEffects.None,1);
                spriteBatch.DrawString(_font, _stats["HP"], new Vector2(leftMargin + xCenterBuffer, yPosition + yMargin), Color.Black, 0,Vector2.Zero, _hero.MaxHP.ToString() != _stats["HP"] ? 1f : 1.5f,SpriteEffects.None,1);
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
                if (_stopwatch.ElapsedMilliseconds >= 2000)
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
        private Scene _scene = Scene.Idle;
        private bool _done = false;
        private int _remainingExp = 0;
        private int _expGain = 0;
        private List<string> _rewards = new List<string>();
        private List<LevelUp> _levelUps = new List<LevelUp>();
        private List<PartyRow> _partyRows = new List<PartyRow>();
        private List<Dictionary<string, string>> _oldStats = new List<Dictionary<string, string>>();
        public BattleVictory(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel) : base(game, graphicsDevice, content, battleModel)
        {
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
                _rewards.Add(enemy.Reward);
            }
            _remainingExp = _expGain;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _partyRows.ForEach(x => x.Draw(gameTime, spriteBatch));
            int yOffset = 0;
            spriteBatch.DrawString(_font, "Victory", new Vector2(_vW - _font.MeasureString("Victory").X, yOffset), Color.Black);
            yOffset += _font.LineSpacing;

            foreach (string reward in _rewards)
            {
                spriteBatch.DrawString(_font, $"Monster dropped: {reward}", new Vector2(_vW - _font.MeasureString($"Monster dropped: {reward}").X, yOffset), Color.Black);
                yOffset += _font.LineSpacing;
            }
            _levelUps.ForEach(x => x.Draw(gameTime, spriteBatch));

        }
        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter))
            {
                if (_done)
                {
                    BattleViewManager.Instance.Done(new Rectangle(0,0,_vW, _vH));
                    return;
                }
                _party.ForEach(hero => hero.IncreaseExperience(_remainingExp));
                _scene = Scene.AnimatingEnd;
                _remainingExp = 0;
                _done = true;
            }
            _previousKeyboard = ks;
            switch (_scene)
            {
                case Scene.Idle:
                    _scene = Scene.AnimatingStart;
                    break;
                case Scene.AnimatingStart:
                    if (_remainingExp <= 0)
                    {
                        _scene = Scene.Idle;
                        _done = true;
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


                            _levelUps.Add(
                                new LevelUp(
                                    hero,
                                    _oldStats[_party.IndexOf(hero)],
                                    r,
                                    _font
                                )
                            );
                        };
                    });
                    _remainingExp -= 1;
                    _scene = Scene.AnimatingEnd;
                    break;
                case Scene.AnimatingEnd:
                    _scene = Scene.AnimatingStart;
                    break;
                default:
                    break;
            }
            _levelUps.ForEach(x => x.Update(gameTime));
        }
    }
}
