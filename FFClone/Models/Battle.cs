using FFClone.Helpers.Keyboard;
using FFClone.Helpers.Shapes;
using FFClone.Sprites;
using FFClone.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.Models
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
    public class Battle
    {
        public BattleScene BattleScene { get; set; } = BattleScene.Idle;
        // currentlyAnimating
        #region CurrentlyAnimating
        private List<BattleSprite> _battleSprites = new List<BattleSprite>();
        public BattleSprite CurrentlyAnimating { get; set; }
        public bool EnemiesDefeated
        {
            get
            {
                return Enemies.TrueForAll(h => h.HP <= 0);
            }
        }
        public bool PartyDefeated
        {
            get
            {
                return Party.TrueForAll(h => h.HP <= 0);
            }
        }
        public bool BattleOver { get
            {
                return EnemiesDefeated || PartyDefeated;
            }
        }
        private int _currentlyAnimating { get; set; } = 0;
        private Action _currentAction { get; set; }
        #endregion
        public List<Hero> Party { get; set; }
        public List<Enemy> Enemies { get; set; }
        // TODO - Change from Hero/Enemy to Character, rename to Current/Selected
        public IBattleable Current { get; set; }
        public IBattleable Target { get; set; }
        public Prompt Prompt { get; private set; }
        public bool NextRound;
        public bool HasPrompt { get { return _hasPrompt; } }
        private bool _hasPrompt = false;
        public List<Action> RoundActions { get; set; } = new List<Action>();
        private int _vW;
        private int _vH;
        private int _turn = 0;
        private Game1 _game;
        private SpriteFont _font;
        private Stopwatch _sw = new Stopwatch();

        public Battle(List<Hero> party, List<Enemy> enemies, Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _vW = game.Window.ClientBounds.Width;
            _vH = game.Window.ClientBounds.Height;
            _game = game;
            _font = content.Load<SpriteFont>("Font/font");
            Current = party[0];
            party.ForEach(hero => {
                Texture2D texture = content.Load<Texture2D>(hero.Path);
                hero.BattleSprite = new BattleSprite(texture, 1, 3)
                {
                    Playing = true
                };
                _battleSprites.Add(hero.BattleSprite);
            });
            Party = party;
            Enemies = enemies;
            enemies.ForEach(enemy =>
            {
                Texture2D texture = content.Load<Texture2D>(enemy.Path);
                enemy.BattleSprite = new BattleSprite(texture, 1, 8);
                _battleSprites.Add(enemy.BattleSprite);

            });

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // heroes
            Party.ForEach(hero => {
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
            Enemies.ForEach(enemy => {
                enemy.BattleSprite.Draw(spriteBatch);
            });

            if (_hasPrompt)
            {
                Prompt.Draw(gameTime, spriteBatch);
            }

        }

        public void RoundReset()
        {
            RoundActions = new List<Action>();
            Party.ForEach(hero => hero.Defending = false);
            BattleScene = BattleScene.Idle;
            Current = Party[0];
            _currentlyAnimating = 0;
        }

        public void SetSelected(int promptOn, Ability ability)
        {
            Target = Enemies[promptOn];
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
            if (_turn >= Party.Count)
            {
                _turn = 0;
                NextRound = true;
            }
            Current = Party[_turn];
            pos = Current.BattleSprite.Position;
            Current.BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }

        public void Update(GameTime gameTime)
        {
            switch (BattleScene)
            {
                case BattleScene.Idle:
                    Party.ForEach(hero => hero.BattleSprite.Update(gameTime));
                    Enemies.ForEach(enemy=> enemy.BattleSprite.Update(gameTime));
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

                    //CurrentlyAnimating.NextPosition = new Vector2(xDistance / 48)


                    //foreach (Models.Action action in RoundActions)
                    //{
                    //    Debug.WriteLine($"{action.Executor.Name} {action.Move.Name}s {action.Target.Name} for 1 damage.");
                    //    action.Target.HP -= action.Executor.Strength;
                    //}
                    break;
                case BattleScene.Animating:
                    //_currentAction.Executor.
                    //if (!_sw.IsRunning)
                    //{
                    //    _sw = new Stopwatch();
                    //    _sw.Start();

                    //}

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
            Party.ForEach(hero => {
                hero.BattleSprite.Position = new Vector2((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset));
                yOffset += 0.2;
            });

            int current = Party.IndexOf((Hero)Current);

            Vector2 pos = Current.BattleSprite.Position;
            Party[current].BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }
        public void SetEnemySprite()
        {
            int largestSpriteHeight = 0;

            Enemies.ForEach(enemy =>
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
            int spritesLeft = Enemies.Count;
            Enemies.ForEach(enemy =>
            {
                int yMargin = 0;
                if (spritesLeft <= (Enemies.Count % spritesPerColumn))
                {
                    int availableSpace = (spritesPerColumn * largestSpriteHeight) - (Enemies.Count % spritesPerColumn) * largestSpriteHeight;
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
        internal void Resized()
        {

            _vW = _game.Window.ClientBounds.Width;
            _vH = _game.Window.ClientBounds.Height;
            SetHeroSprites();
            SetEnemySprite();

            List<Vector2> vectors = new List<Vector2>();

            if (_hasPrompt)
            {
                Enemies.ForEach(enemy =>
                {
                    vectors.Add(new Vector2(enemy.BattleSprite.Position.X, enemy.BattleSprite.Position.Y + (int)(0.5 * enemy.BattleSprite.Height)));
                });
                Prompt.Vectors = vectors;
                Prompt.Resized();
            }
        }
    }
}
