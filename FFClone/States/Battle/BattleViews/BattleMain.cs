using FFClone.Controls;
using FFClone.Helpers.Shapes;
using FFClone.Models;
using FFClone.Sprites;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.States.Battle.BattleViews
{
    public class BattleAction
    {
        public IBattleable Executor { get; set; }
        public IBattleable Target { get; set; }
        public Action Action { get; set; }
        public bool Done { get; set; } = false;

        public BattleAction(IBattleable executor, IBattleable target, Action action)
        {
            Executor = executor;
            Target = target;
            Action = action;
        }
    }
    public enum BattleScene
    {
        Idle,
        AnimatingStart,
        Animating,
        DamageCalculation,
        AnimatingEnd,
    }
    public class BattleMain : BattleView
    {

        public BattleScene BattleScene { get; set; } = BattleScene.Idle;
        // currentlyAnimating
        #region CurrentlyAnimating
        private List<BattleSprite> _battleSprites = new List<BattleSprite>();
        public BattleSprite CurrentlyAnimating { get; set; }
        //private int _currentlyAnimating { get; set; } = 0;
        private BattleAction _currentAction { get; set; }
        #endregion
        // TODO - Change from Hero/Enemy to Character, rename to Current/Selected
        public IBattleable Current { get; set; }
        public IBattleable Target { get; set; }
        public List<BattleAction> RoundActions { get; set; } = new List<BattleAction>();
        public List<IBattleable> ValidEnemies
        {
            get
            {
                List<IBattleable> options = new List<IBattleable>();
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy.HP > 0)
                    {
                        options.Add(enemy);
                    }
                }
                return options;
            }
        }

        public Prompt Prompt { get; private set; }
        public bool HasPrompt { get { return _hasPrompt; } }
        private bool _hasPrompt = false;
        private int _turn = 0;
        public bool NextRound;

        private int _thickness = 10;
        private BattleBars _battleBar;
        private Texture2D _battleTexture;


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
            _battleTexture = content.Load<Texture2D>("Sprites/Backgrounds/country_field");

            SetHeroSprites();
            SetEnemySprite();
        }
        public override void Update(GameTime gameTime)
        {
            if (NextRound)
            {
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy.HP > 0)
                    {
                       RoundActions.Add(new BattleAction(enemy, _party[0], new Ability("Attack")));
                    }
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
                    _currentAction = RoundActions.Find(x => x.Done == false);

                    if (_currentAction == null)
                    {
                        BattleScene = BattleScene.Idle;
                        break;
                    }

                    Current = _currentAction.Executor;
                    Target = _currentAction.Target;
                    BattleScene = BattleScene.Animating;
                    CurrentlyAnimating = Current.BattleSprite;
                    Vector2 endDestination = new Vector2(
                        Target.BattleSprite.Position.X + Target.BattleSprite.Width,
                        Target.BattleSprite.Position.Y
                        );

                    Current.HomePosition = CurrentlyAnimating.Position;
                    CurrentlyAnimating.NextPosition = endDestination;

                    int ticks = 24;
                    double xDistance = Current.HomePosition.X - endDestination.X;
                    double yDistance = Current.HomePosition.Y - endDestination.Y;
                    Current.MoveByTick = new Vector2(
                        (float)(xDistance / ticks),
                        (float)(yDistance / ticks)
                    );
                    Current.EndRectangle = new Rectangle((int)endDestination.X, (int)endDestination.Y, 1, 1);

                    break;
                case BattleScene.Animating:
                    CurrentlyAnimating.Position = new Vector2(
                        CurrentlyAnimating.Position.X - Current.MoveByTick.X,
                        CurrentlyAnimating.Position.Y - Current.MoveByTick.Y
                    );

                    if (Current.EndRectangle.Intersects(
                        new Rectangle(
                            new Point((int)CurrentlyAnimating.Position.X, (int)CurrentlyAnimating.Position.Y), 
                            new Point(CurrentlyAnimating.Width, CurrentlyAnimating.Height)
                        ))) {
                        BattleScene = BattleScene.DamageCalculation;
                        CurrentlyAnimating.Position = Current.HomePosition;
                    }
                    break;
                case BattleScene.DamageCalculation:
                    switch (_currentAction.Action.Name)
                    {
                        case "Defend":
                            break;
                        default:
                            Target.HP -= Current.CalculateBattleDamage();
                            Debug.WriteLine($"{_currentAction.Executor.Name} {_currentAction.Action.Name}s {_currentAction.Target.Name} for 1 damage.");
                            break;
                    }
                    _currentAction.Done = true;
                    if (Target.HP <= 0)
                    {
                        Target.BattleSprite = new BattleSprite(
                            _content.Load<Texture2D>("Sprites/grave_markers-shadow"), 1, 1)
                        { Position = new Vector2(
                            Target.BattleSprite.Position.X +(int)(0.5 * Target.BattleSprite.Width),
                            Target.BattleSprite.Position.Y +(int)(0.5 * Target.BattleSprite.Height)
                            )};
                        RoundActions.ForEach(x =>
                        {
                            if (x.Executor == Target || x.Target == Target)
                            {
                                x.Done = true;
                            }
                        });
                    }
                    BattleScene = BattleScene.AnimatingEnd;
                    break;
                case BattleScene.AnimatingEnd:
                    CurrentlyAnimating.Position = Current.HomePosition;
                    if (_battleModel.BattleOver)
                    {
                        if (_party.TrueForAll(x => x.HP <= 0)) 
                        {
                            StateManager.Instance.Next(
                                new MainMenuState(_game, _graphicsDevice, _content), new FadeOut(48, new Rectangle(0, 0, _vW, _vH))
                            );
                                break;

                        }
                        _stateManager.Next(new BattleVictory(_game, _graphicsDevice, _content, _battleModel));
                    }
                    if (RoundActions.TrueForAll(x => x.Done))
                    {
                        RoundReset();
                    } else
                    {
                        BattleScene = BattleScene.AnimatingStart;
                    }

                    break;
                default:
                    break;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_battleTexture, new Rectangle(0, 0, _vW, (int)(_vH * 0.7)), Color.White);
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
            int yOffset = 0;
            // enemies
            _enemies.ForEach(enemy => {
                enemy.BattleSprite.Draw(spriteBatch);
                spriteBatch.ProgressBar(_font, enemy.Name, enemy.HP, enemy.MaxHP, Color.Green, Color.Red, new Rectangle(0, yOffset, 100, 20), false);
                yOffset += _font.LineSpacing;
                spriteBatch.DrawString(_font, $"{enemy.Name} {enemy.HP}/{enemy.MaxHP}", new Vector2(0, yOffset), Color.Black);
            });

            if (_hasPrompt)
            {
                Prompt.Draw(gameTime, spriteBatch);
            }
            _battleBar.Draw(gameTime, spriteBatch);


        }
        public void EnablePrompt(List<IBattleable> options, Action action)
        {
            _hasPrompt = true;
            Prompt = new Prompt(options, this, action);
        }
        public void DisablePrompt()
        {
            _hasPrompt = false;
            Prompt = null;
        }
        public void RoundReset()
        {
            RoundActions = new List<BattleAction>();
            _party.ForEach(hero => hero.Defending = false);
            BattleScene = BattleScene.Idle;
            Current = _party.Find(x => x.HP > 0);
        }
        public void SetSelected(IBattleable promptOn, Action action)
        {
            Target = promptOn;
            RoundActions.Add(new BattleAction(Current, Target, action));
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

            _battleBar.NewMenu(Current);
            Current.BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }
        public void Defend()
        {
            Current.Defending = true;
            RoundActions.Add(new BattleAction(Current, Current, new Ability("Defend")));
            NextHero();
        }
        public void SetHeroSprites()
        {
            double yOffset = 0.2;
            _party.ForEach(hero => {
                hero.BattleSprite.Position = new Vector2((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset));
                yOffset += 0.15;
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

            int yOffset = (int)(0.2 * _vH);
            int spritesPerColumn = (int)(0.7 * _vH - yOffset) / (largestSpriteHeight);
            int index = 0;
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
                    yOffset = (int)(0.2 * _vH);
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


            if (_hasPrompt)
            {
                List<IBattleable> options = new List<IBattleable>();

                _enemies.ForEach(enemy =>
                {
                    if (enemy.HP > 0)
                    {
                        options.Add(enemy);
                    }
                });
                Prompt.Options = options;
                Prompt.Resized();
            }
            _battleBar.Resized();

        }
    }
}
