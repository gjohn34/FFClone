using FFClone.States.Battle.BattleViews;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FFClone.States.Battle
{
    public class BattleViewManager
    {

        private static BattleViewManager instance = null;
        private static readonly object padlock = new object();

        private BattleView _current;
        private BattleView _next;
        private GameState _previous;

        BattleViewManager()
        {
        }

        public static BattleViewManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BattleViewManager();
                    }
                    return instance;
                }
            }
        }

        public void Done()
        {
            StateManager.Instance.Next(_previous, new NoTransition());
            _current = null;
            _next = null;
        }

        public void New(BattleView view, GameState prev)
        {
            _previous = prev;
            _next = view;
        }

        public void Next(BattleView view)
        {
            _next = view;
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
            if (_current != null)
            {
                _current.Draw(gameTime, spriteBatch);
            }
        }
        internal void Resized()
        {
            _current.Resized();
        }
    }
}