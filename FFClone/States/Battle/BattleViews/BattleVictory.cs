using FFClone.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.States.Battle.BattleViews
{
    public class BattleVictory : BattleView
    {
        private int _expGain = 0;
        private List<string> _rewards = new List<string>();
        public BattleVictory(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel) : base(game, graphicsDevice, content, battleModel)
        {
            foreach (Enemy enemy in _enemies)
            {
                _expGain += enemy.ExperienceGain;
                _rewards.Add(enemy.Reward);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int yOffset = 0;
            spriteBatch.DrawString(_font, "victory", Vector2.Zero, Color.Black);
            yOffset += _font.LineSpacing;
            spriteBatch.DrawString(_font, $"Party earnt: {_expGain} experience", new Vector2(0, yOffset), Color.Black);
            yOffset += _font.LineSpacing;
            foreach (string reward in _rewards)
            {
                spriteBatch.DrawString(_font, $"Monster dropped: {reward}", new Vector2(0, yOffset), Color.Black);
                yOffset += _font.LineSpacing;

            }

        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
