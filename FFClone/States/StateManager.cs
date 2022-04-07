using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FFClone
{
    public sealed class StateManager
    {
        private static StateManager instance = null;
        private static readonly object padlock = new object();
        private static State _current;
        private static State _next;
        StateManager()
        {
        }

        public static StateManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new StateManager();
                    }
                    return instance;
                }
            }
        }

        public void Next(State state)
        {
            _next = state;
        }

        public void Update(GameTime gameTime)
        {
            if (_next != null)
            {
                _current = _next;
                _next = null;
            }
            _current.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _current.Draw(gameTime, spriteBatch);
        }
    }
}
