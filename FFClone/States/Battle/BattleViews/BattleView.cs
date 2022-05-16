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
    public abstract class BattleView
    {
        #region Fields
        protected Game1 _game;
        protected GraphicsDevice _graphicsDevice;
        protected ContentManager _content;
        protected int _vH;
        protected int _vW;
        protected SpriteFont _font;
        protected BattleModel _battleModel;
        protected BattleViewManager _stateManager;
        protected KeyboardState _previousKeyboard;
        protected List<Hero> _party;
        protected List<Enemy> _enemies;
        #endregion
        public BattleView(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, BattleModel battleModel)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _vH = game.Window.ClientBounds.Height;
            _vW = game.Window.ClientBounds.Width;
            _font = content.Load<SpriteFont>("Font/font");
            _battleModel = battleModel;
            _party = battleModel.Party;
            _enemies = battleModel.Enemies;
            _stateManager = BattleViewManager.Instance;
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public virtual void Resized()
        {
            _vH = _game.Window.ClientBounds.Height;
            _vW = _game.Window.ClientBounds.Width;
        }
    }
}
