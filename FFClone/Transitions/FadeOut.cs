using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Transitions
{
    public class FadeOut : Transition
    {
        private Color _color;
        public FadeOut(int totalFrames, Rectangle rectangle) : base(totalFrames, rectangle)
        {
            _color = new Color(Color.Black, 255);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            Primitives2D.FillRectangle(spriteBatch, Rectangle, _color);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            Frame += 1;

           // do something to finish
            if (Frame >= TotalFrames)
                return;

            _color = new Color(Color.Black, (float)((float)Frame / (float)TotalFrames) * -1 + 1);
        }
    }
}
