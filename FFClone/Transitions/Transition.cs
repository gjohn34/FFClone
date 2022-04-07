using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Transitions
{
    public abstract class Transition 
    {
        protected int Frame { get; set; } = 0;
        protected int TotalFrames { get; set; }
        protected Rectangle Rectangle { get; set; }
        public Transition(int totalFrames, Rectangle rectangle)
        {
            TotalFrames = totalFrames;
            Rectangle = rectangle;
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public bool IsFinished()
        {
            return Frame >= TotalFrames;
        }
    }

}
