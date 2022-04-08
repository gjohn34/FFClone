using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Transitions
{
    public class NoTransition : Transition
    {
        public NoTransition(int totalFrames, Rectangle rectangle) : base(totalFrames, rectangle)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
