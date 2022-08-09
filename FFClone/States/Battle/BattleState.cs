using FFClone.Controls;
using FFClone.Models;
using FFClone.States.Battle;
using FFClone.States.Battle.BattleViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FFClone.States
{
    public class BattleState : State
    {
        private BattleModel _battleModel;
        private BattleViewManager _battleViewManager;

        // TODO make thickness reactive

        public BattleState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState) : base(game, graphicsDevice, content)
        {
            Dictionary<string, int> statBlock = new Dictionary<string, int>()
            {
                {
                    "HP",1
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
                    "EXP", 2
                }
            };

            int rand = new Random().Next(1, 5);
            List<Enemy> enemies = new List<Enemy>();


            for (int x = 0; x < rand; x++)
            {
                enemies.Add(new Enemy($"Monster {x + 1}", Color.Purple, statBlock));
            }

            _battleModel = new BattleModel(GameInfo.Instance.Party, enemies);
            _battleViewManager = BattleViewManager.Instance;
            //_battleViewManager = new BattleViewManager(new BattleVictory(game, graphicsDevice, content, _battleModel), gameState);
            _battleViewManager.New(new BattleMain(game, graphicsDevice, content, _battleModel), gameState);

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
            //if (_battleModel.BattleOver)
            //{
            //    _battleViewManager.Next(new BattleVictory(_game, _graphicsDevice, _content, _battleModel));
            //}
            _battleViewManager.Update(gameTime);
        }

        public override void Resized()
        {
            base.Resized();
            _battleViewManager.Resized();
        }
    }
}