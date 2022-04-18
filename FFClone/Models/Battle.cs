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
    enum BattleState
    {
        Selecting,
        Animating,
        None
    }
    public class Battle
    {
        
        public List<Hero> Party { get; set; }
        //private List<BattleSprite> _partySprites = new List<BattleSprite>();
        public List<Enemy> Enemies { get; set; }
        public Hero Current { get; set; }
        public Enemy Selected { get; set; }
        public Prompt Prompt { get; private set; }
        private bool _hasPrompt = false;
        public bool HasPrompt { get { return _hasPrompt; } }
        private int _vW;
        private int _vH;
        private int _turn = 0;
        private bool _nextRound;
        private Game1 _game;

        public Battle(List<Hero> party, List<Enemy> enemies, Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _vW = game.Window.ClientBounds.Width;
            _vH = game.Window.ClientBounds.Height;
            _game = game;
            double yOffset = 0.1;

            party.ForEach(hero => {
                Texture2D texture = content.Load<Texture2D>(hero.Path);
                hero.BattleSprite = new AnimatedSprite(texture, 1, 3)
                {
                    Position = new Vector2((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset)),
                    Playing = true
                };
                yOffset += 0.2;
            });
            Party = party;
            Enemies = enemies;
            int requiredHeight = 0;
            int largestSpriteHeight = 0;
            enemies.ForEach(enemy =>
            {
                Texture2D texture = content.Load<Texture2D>(enemy.Path);
                enemy.BattleSprite = new AnimatedSprite(texture, 1, 8);
                requiredHeight += enemy.BattleSprite.Height;
                if (enemy.BattleSprite.Height > largestSpriteHeight)
                {
                    largestSpriteHeight = enemy.BattleSprite.Height;
                }
            });
            // center sprites dynamically, look at menu
            //int availableSpaceForMargin = (int)(0.7 * _vH) - (spritesPerColumn * largestSpriteHeight);
            //int margin = availableSpaceForMargin / spritesPerColumn + (int)Math.Ceiling((0.5 * largestSpriteHeight));
            int spritesPerColumn = (int)(0.7 * _vH) / largestSpriteHeight;
            int index = 0;
            yOffset = 0;
            int xOffset = 0;
            enemies.ForEach(enemy =>
            {
                enemy.BattleSprite.Position = new Vector2((int)(_vW * 0.3) - xOffset,(int)yOffset + 50);
                enemy.BattleSprite.Playing = true;
                index++;
                if (index >= spritesPerColumn)
                {
                    xOffset += enemy.BattleSprite.Width;
                    yOffset = 0;
                    index = 0;
                } else
                {
                    yOffset += enemy.BattleSprite.Height;
                }
            });

            Current = party[0];
            Vector2 pos = Current.BattleSprite.Position;
            Current.BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }

        public void EnablePrompt(List<Vector2> vectors)
        {
            _hasPrompt = true;
            Prompt = new Prompt(vectors, this);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_hasPrompt)
            {
                Prompt.Draw(gameTime, spriteBatch);
            }

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
        }

        public void RoundReset()
        {
            Party.ForEach(hero => hero.Defending = false);
            _nextRound = false;
        }

        public void SetSelected(int promptOn)
        {
            Selected = Enemies[promptOn];
            _hasPrompt = false;
            NextHero();
           
        }

        private void NextHero()
        {
            Vector2 pos = Current.BattleSprite.Position;
            Current.BattleSprite.Position = new Vector2(pos.X + 50, pos.Y);
            _turn += 1;
            if (_turn >= Party.Count)
            {
                _turn = 0;
                _nextRound = true;
            }
            Current = Party[_turn];
            pos = Current.BattleSprite.Position;
            Current.BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
        }

        public void Update(GameTime gameTime)
        {
            if (_nextRound)
            {
                RoundReset();
            }   
            Party.ForEach(hero => hero.BattleSprite.Update(gameTime));
            Enemies.ForEach(enemy=> enemy.BattleSprite.Update(gameTime));
        }
        public bool BattleOver()
        {
            return false;
        }
        public void Defend()
        {
            Current.Defending = true;
            NextHero();
        }

        internal void Resized()
        {
            double yOffset = 0.1;
            _vH = _game.Window.ClientBounds.Height;
            _vW = _game.Window.ClientBounds.Width;
            Party.ForEach(hero => {
                hero.BattleSprite.Position = new Vector2((int)(_vW - (_vW * 0.20)), (int)(_vH * yOffset));
                yOffset += 0.2;
            });

            int current = Party.IndexOf(Current);

            Vector2 pos = Current.BattleSprite.Position;
            Party[current].BattleSprite.Position = new Vector2(pos.X - 50, pos.Y);
            
            int largestSpriteHeight = 0;
            Enemies.ForEach(enemy =>
            {
                if (enemy.BattleSprite.Height > largestSpriteHeight)
                {
                    largestSpriteHeight = enemy.BattleSprite.Height;
                }
            });

            int spritesPerColumn = (int)(0.7 * _vH) / largestSpriteHeight;
            int index = 0;
            yOffset = 0;
            int xOffset = 0;
            Enemies.ForEach(enemy =>
            {
                enemy.BattleSprite.Position = new Vector2((int)(_vW * 0.3) - xOffset, (int)yOffset + 50);
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
            });

            List<Vector2> vectors = new List<Vector2>();

            if (_hasPrompt)
            {
                Enemies.ForEach(enemy =>
                {
                    vectors.Add(enemy.BattleSprite.Position);
                });
                Prompt.Vectors = vectors;
                Prompt.Resized();
            }
        }
    }
}
