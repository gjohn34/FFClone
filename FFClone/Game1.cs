﻿using FFClone.States;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FFClone
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private StateManager _stateManager;
        private GameInfo _gameInfo;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 24);
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            //_graphics.IsFullScreen = false;
            //_graphics.PreferredBackBufferWidth = 1000;
            //_graphics.PreferredBackBufferHeight = 1000;
            //_graphics.ApplyChanges();
            base.Initialize();
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            _stateManager.Resized();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _stateManager = StateManager.Instance;
            _gameInfo = GameInfo.Instance;
            _stateManager.Next(new MainMenuState(this, GraphicsDevice, Content), Transition.NoTransition);
            //_stateManager.Next(new BattleState(this, GraphicsDevice, Content), Transition.NoTransition);
            //_stateManager.Next(new MainMenuState(this, GraphicsDevice, Content), new FadeIn(60, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height)));

        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            _stateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _stateManager.Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }
    }
}
