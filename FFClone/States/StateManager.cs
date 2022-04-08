using FFClone.States;
using FFClone.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FFClone
{
    enum Animation
    {
        None,
        Playing,
        Finished
    }
    public sealed class StateManager
    {
        private static StateManager instance = null;
        private static readonly object padlock = new object();
        private static State _current;
        private static State _next;
        private Animation _animation = Animation.None;
        private Transition _transition;
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

        public void Next(State state, Transition transition)
        {
            _next = state;
            _transition = transition;
            _animation = Animation.Playing;
        }

        public void Update(GameTime gameTime)
        {
            if (_transition != null && _transition.IsFinished())
            {
                _animation = Animation.Finished;
            }
            if (_next != null)
            {
                _current = _next;
                _next = null;
            }
            switch (_animation)
            {
                case Animation.None:
                    _current.Update(gameTime);
                    break;
                case Animation.Playing:
                    _transition.Update(gameTime);
                    break;
                case Animation.Finished:
                    _transition = null;
                    _animation = Animation.None;
                    break;
                default:
                    break;
            }
        }

        internal void Resized()
        {
            _current.Resized();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (_animation)
            {
                case Animation.None:
                    _current.Draw(gameTime, spriteBatch);
                    break;
                case Animation.Playing:
                    _current.Draw(gameTime, spriteBatch);
                    _transition.Draw(gameTime, spriteBatch);
                    break;
                case Animation.Finished:
                    break;
                default:
                    break;
            }
        }
    }
}
