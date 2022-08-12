using FFClone.States;
using FFClone.States.Battle;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 24);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 640;
            // Enable when I figure out the map movement
            //Window.ClientSizeChanged += Window_ClientSizeChanged;

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
            _stateManager.Next(new MainMenuState(this, GraphicsDevice, Content), Transition.NoTransition);

        }

        protected override void Update(GameTime gameTime)
        {
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
