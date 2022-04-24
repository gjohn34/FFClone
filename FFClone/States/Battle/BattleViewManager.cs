using FFClone.States.Battle.BattleViews;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FFClone.States.Battle
{
    public class BattleViewManager
    {

        private BattleView _current;
        private BattleView _next;

        public void Next(BattleView view)
        {
            _next = view;
        }
        public BattleViewManager(BattleView next) 
        {
            _next = next;
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
        internal void Resized()
        {
            _current.Resized();
        }
    }
}