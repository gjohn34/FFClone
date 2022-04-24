﻿using FFClone.Controls;
using FFClone.Models;
using FFClone.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using FFClone.States.Battle.BattleViews;

namespace FFClone.States.Battle.BattleViews
{
    public class Action
    {
        public IBattleable Executor { get; set; }
        public IBattleable Target { get; set; }
        public Ability Move { get; set; }
        public Action(IBattleable executor, IBattleable target, Ability ability)
        {
            Executor = executor;
            Target = target;
            Move = ability;
        }
        //public int CalculateNewTotals()
        //{
        //    // fix this for other actions
        //    //return Executor.CalculateBattleDamage();
        //}
    }
    public enum BattleScene
    {
        Idle,
        AnimatingStart,
        Animating,
        AnimatingEnd,
    }
    public class BattleMain : BattleView
    {
        public BattleScene BattleScene { get; set; } = BattleScene.Idle;
        // currentlyAnimating
        #region CurrentlyAnimating
        private List<BattleSprite> _battleSprites = new List<BattleSprite>();
        public BattleSprite CurrentlyAnimating { get; set; }
        private int _currentlyAnimating { get; set; } = 0;
        private Action _currentAction { get; set; }
        #endregion
        // TODO - Change from Hero/Enemy to Character, rename to Current/Selected
        public IBattleable Current { get; set; }
        public IBattleable Target { get; set; }
        public List<Action> RoundActions { get; set; } = new List<Action>();

        public Prompt Prompt { get; private set; }
        public bool HasPrompt { get { return _hasPrompt; } }
        private bool _hasPrompt = false;
        private int _turn = 0;
        public bool NextRound;

        private int _thickness = 10;
        private BattleBars _battleBar;



        public BattleMain(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel) : base(game, graphicsDevice, content, battleModel)
        {
            Current = _party[0];
            _party.ForEach(hero => {
                Texture2D texture = content.Load<Texture2D>(hero.Path);
                hero.BattleSprite = new BattleSprite(texture, 1, 3)
                {
                    Playing = true
                };
                _battleSprites.Add(hero.BattleSprite);
            });
            _enemies.ForEach(enemy =>
            {
                Texture2D texture = content.Load<Texture2D>(enemy.Path);
                enemy.BattleSprite = new BattleSprite(texture, 1, 8);
                _battleSprites.Add(enemy.BattleSprite);

            });


            Rectangle rectangle = new Rectangle(_thickness, _vH - (int)(_vH * 0.3), _vW - _thickness, (int)(_vH * 0.3) - _thickness);
            _battleBar = new BattleBars(game, graphicsDevice, content, _party, _enemies, this, rectangle);

            SetHeroSprites();
            SetEnemySprite();
        }

        public void EnablePrompt(List<Vector2> vectors, Ability ability)
        {
            _hasPrompt = true;
            Prompt = new Prompt(vectors, this, ability);
        }
        public void DisablePrompt()
        {
            _hasPrompt = false;
            Prompt = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // heroes
            _party.ForEach(hero => {
                hero.BattleSprite.Draw(spriteBatch);
                if (hero.Defending)
                {
                    Vector2 center = new Vector2(hero.BattleSprite.Position.X + (int)(hero.BattleSprite.Width / 2), hero.BattleSprite.Position.Y + (int)(hero.BattleSprite.Height / 2));
                    int thickness = 10;
                    float radius = (float)((hero.BattleSprite.Width / 2) + (0.1 * hero.BattleSprite.Width));
                    spriteBatch.DrawCircle(center, radius + thickness, 50, Color.Blue, thickness);
                }
            });


            // enemies
            _enemies.ForEach(enemy => {
                enemy.BattleSprite.Draw(spriteBatch);
            });

            if (_hasPrompt)
            {
                Prompt.Draw(gameTime, spriteBatch);
            }
            _battleBar.Draw(gameTime, spriteBatch);


        }

        public void RoundReset()
        {
            RoundActions = new List<Action>();
            _party.ForEach(hero => hero.Defending = false);
            BattleScene = BattleScene.Idle;
            Current = _party[0];
            _currentlyAnimating = 0;
        }

        public void SetSelected(int promptOn, Ability ability)
        {
            Target = _enemies[promptOn];
            RoundActions.Add(new Action(Current, Target, ability));
            _hasPrompt = false;
            NextHero();
           
        }
        private void NextHero()
        {
            Vector2 pos = Current.BattleSprite.Position;
            // something funky going on here
            Current.BattleSprite.Position = new Vector2(pos.X + 50, pos.Y);
            _turn += 1;
            if (_turn >= _party.Count)
            {
                _turn = 0;
                NextRound = true;
            }
            Current = _party[_turn];
            pos = Current.BattleSprite.Position;
            Current.BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }

        public override void Update(GameTime gameTime)
        {

            if (NextRound)
            {
                foreach (Enemy enemy in _enemies)
                {
                    RoundActions.Add(new Action(enemy, _party[0], new Ability("attack")));
                }
                BattleScene = BattleScene.AnimatingStart;
            }
            switch (BattleScene)
            {
                case BattleScene.Idle:
                    _party.ForEach(hero => hero.BattleSprite.Update(gameTime));
                    _enemies.ForEach(enemy=> enemy.BattleSprite.Update(gameTime));

                    if (HasPrompt)
                    {
                        Prompt.Update(gameTime);
                    } else
                    {
                        _battleBar.Update(gameTime);
                    }
                    break;
                case BattleScene.AnimatingStart:
                    NextRound = false;
                    BattleScene = BattleScene.Animating;

                    _currentAction = RoundActions[_currentlyAnimating];
                    Current = _currentAction.Executor;
                    Target = _currentAction.Target; 
                    CurrentlyAnimating = _battleSprites[_currentlyAnimating];
                    IBattleable target = _currentAction.Target;
                    Vector2 endDestination = new Vector2(
                        target.BattleSprite.Position.X + target.BattleSprite.Width,
                        target.BattleSprite.Position.Y
                        );

                    Current.HomePosition = CurrentlyAnimating.Position;
                    CurrentlyAnimating.NextPosition = endDestination;

                    int ticks = 32;
                    double xDistance = Current.HomePosition.X - endDestination.X;
                    double yDistance = Current.HomePosition.Y - endDestination.Y;
                    Current.MoveByTick = new Vector2(
                        (float)(xDistance / ticks),
                        (float)(yDistance / ticks)
                    );
                    Current.EndRectangle = new Rectangle((int)endDestination.X, (int)endDestination.Y, 1, 1);
                    Debug.WriteLine($"{_currentAction.Executor.Name} {_currentAction.Move.Name}s {_currentAction.Target.Name} for 1 damage.");

                    break;
                case BattleScene.Animating:
                    CurrentlyAnimating.Position = new Vector2(
                        CurrentlyAnimating.Position.X - Current.MoveByTick.X,
                        CurrentlyAnimating.Position.Y - Current.MoveByTick.Y
                    );

                    if (Current.EndRectangle.Intersects(CurrentlyAnimating.Rectangle)) {
                        BattleScene = BattleScene.AnimatingEnd;
                        CurrentlyAnimating.Position = Current.HomePosition;
                    }
                    break;
                case BattleScene.AnimatingEnd:
                    CurrentlyAnimating.Position = Current.HomePosition;
                    if (CurrentlyAnimating == _battleSprites[_battleSprites.Count - 1])
                    {
                        RoundReset();
                    } else
                    {
                        _currentlyAnimating += 1;
                        BattleScene = BattleScene.AnimatingStart;
                    }
                    Target.HP -= Current.CalculateBattleDamage();

                    break;
                default:
                    break;
            }
        }
        public void Defend()
        {
            Current.Defending = true;
            RoundActions.Add(new Action(Current, Current, new Ability("Defend")));
            NextHero();
        }
        public void SetHeroSprites()
        {
            double yOffset = 0.1;
            _party.ForEach(hero => {
                hero.BattleSprite.Position = new Vector2((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset));
                yOffset += 0.2;
            });

            int current = _party.IndexOf((Hero)Current);

            Vector2 pos = Current.BattleSprite.Position;
            _party[current].BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }
        public void SetEnemySprite()
        {
            int largestSpriteHeight = 0;

            _enemies.ForEach(enemy =>
            {
                if (enemy.BattleSprite.Height > largestSpriteHeight)
                {
                    largestSpriteHeight = enemy.BattleSprite.Height;
                }
            });

            int spritesPerColumn = (int)(0.7 * _vH) / (largestSpriteHeight);
            int index = 0;
            int yOffset = 0;
            int xOffset = 0;
            int spritesLeft = _enemies.Count;
            _enemies.ForEach(enemy =>
            {
                int yMargin = 0;
                if (spritesLeft <= (_enemies.Count % spritesPerColumn))
                {
                    int availableSpace = (spritesPerColumn * largestSpriteHeight) - (_enemies.Count % spritesPerColumn) * largestSpriteHeight;
                    yMargin = (int)(0.5 * availableSpace);

                }
                enemy.BattleSprite.Position = new Vector2((int)(_vW * 0.3) - xOffset, (int)yOffset + yMargin);
                enemy.BattleSprite.Playing = true;
                index++;
                if (index >= spritesPerColumn)
                {
                    xOffset += enemy.BattleSprite.Width;
                    yOffset = 0;
                    index = 0;
                }
                else
                {
                    yOffset += enemy.BattleSprite.Height;
                }
                spritesLeft--;
            });
        }
        public override void Resized()
        {
            base.Resized();
            SetHeroSprites();
            SetEnemySprite();

            List<Vector2> vectors = new List<Vector2>();

            if (_hasPrompt)
            {
                _enemies.ForEach(enemy =>
                {
                    vectors.Add(new Vector2(enemy.BattleSprite.Position.X, enemy.BattleSprite.Position.Y + (int)(0.5 * enemy.BattleSprite.Height)));
                });
                Prompt.Vectors = vectors;
                Prompt.Resized();
            }
            _battleBar.Resized();

        }
    }
}