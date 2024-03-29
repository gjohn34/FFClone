﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Transitions
{
    public class FadeIn : Transition
    {
        private Color _color;
        private Color _overlay;

        public FadeIn(int totalFrames, Rectangle rectangle) : base(totalFrames, rectangle)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            Frame += 1;

           // do something to finish
            if (Frame >= TotalFrames)
                return;

        }
    }
}
