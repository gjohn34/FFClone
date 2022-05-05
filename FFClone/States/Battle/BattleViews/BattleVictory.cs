using FFClone.Helpers.Keyboard;
using FFClone.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.States.Battle.BattleViews
{
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
        private List<Texture2D> _portraits = new List<Texture2D>();
        public BattleVictory(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel) : base(game, graphicsDevice, content, battleModel)
        {
            _party.ForEach(hero => _portraits.Add(content.Load<Texture2D>(hero.Portrait)));
            foreach (Enemy enemy in _enemies)
            {
                _expGain += enemy.ExperienceGain;
                _rewards.Add(enemy.Reward);
            }
            _remainingExp = _expGain;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int spaceBetweenY = _vH / _portraits.Count;
            int cellHeight = (int)Math.Ceiling((0.75 * spaceBetweenY));

            _portraits.ForEach(portrait =>
            {
                int index = _portraits.IndexOf(portrait);
                int initialYPos = index * spaceBetweenY;
                double cellPositionPercentage = (double)index / (double)(_portraits.Count - 1);
                int pushDown = (int)(cellPositionPercentage * cellHeight);
                int pushDown2 = (int)(cellPositionPercentage * spaceBetweenY);
                int yPosition = initialYPos - pushDown + pushDown2;
                Rectangle rect =  new Rectangle(0, yPosition, cellHeight, cellHeight);
                spriteBatch.Draw(portrait, rect, Color.White);
                Hero hero = _party[index];
                spriteBatch.DrawString(_font, hero.Name, new Vector2(cellHeight, yPosition), Color.Black);
                yPosition += _font.LineSpacing;
                spriteBatch.DrawString(_font, hero.Experience.ToString(), new Vector2(cellHeight, yPosition), Color.Black);
            });
            int yOffset = 0;
            int xOffset = _vW;
            spriteBatch.DrawString(_font, "victory", new Vector2(xOffset - _font.MeasureString("victory").X, yOffset), Color.Black);
            yOffset += _font.LineSpacing;
            spriteBatch.DrawString(_font, $"Party earnt: {_expGain} experience", new Vector2(xOffset - _font.MeasureString($"Party earnt: {_expGain} experience").X, yOffset), Color.Black);
            yOffset += _font.LineSpacing;
            foreach (string reward in _rewards)
            {
                spriteBatch.DrawString(_font, $"Monster dropped: {reward}", new Vector2(xOffset - _font.MeasureString($"Monster dropped: {reward}").X, yOffset), Color.Black);
                yOffset += _font.LineSpacing;
            }

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
                _party.ForEach(hero => hero.Experience += _remainingExp);
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
                    _party.ForEach(hero => hero.Experience += 1);
                    _remainingExp -= 1;
                    _scene = Scene.AnimatingEnd;
                    break;
                case Scene.AnimatingEnd:
                    _scene = Scene.AnimatingStart;
                    break;
                default:
                    break;
            }
        }
    }
}
