﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Controls
{
    public interface IComponent
    {
        public Rectangle Rectangle { get; set; }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
        public abstract void Resized();
    }
}
