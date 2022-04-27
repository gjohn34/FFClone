using FFClone.Controls;
using FFClone.Models;
using FFClone.States.Battle;
using FFClone.States.Battle.BattleViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FFClone.States
{
    public class BattleState : State
    {
        private BattleModel _battleModel;
        private BattleViewManager _battleViewManager;

        // TODO make thickness reactive

        public BattleState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {

            Dictionary<string, int> statBlock = new Dictionary<string, int>()
            {
                {
                    "HP",2
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
                {
                    "EXP", 10
                }
            };

            List<Enemy> enemies = new List<Enemy> { 
                new Enemy("Purple Guy 1", Color.Purple, statBlock),
                new Enemy("Purple Guy 2", Color.Purple, statBlock),
                new Enemy("Purple Guy 3", Color.Purple, statBlock),
                new Enemy("Purple Guy 4", Color.Purple, statBlock)
            };

            _battleModel = new BattleModel(GameInfo.Instance.Party, enemies);

            _battleViewManager = new BattleViewManager(new BattleMain(game, graphicsDevice, content, _battleModel));

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _battleViewManager.Draw(gameTime, spriteBatch);
            //int width = _vW / 2 - (int)(_font.MeasureString("battlemode").X / 2);
            //spriteBatch.DrawString(_font, "battlemode", new Vector2(width, 0), Color.White);
            //_battle.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (_battleModel.BattleOver)
            {
                _battleViewManager.Next(new BattleVictory(_game, _graphicsDevice, _content, _battleModel));
            }
            _battleViewManager.Update(gameTime);
        }

        public override void Resized()
        {
            base.Resized();
            _battleViewManager.Resized();
        }
    }
}