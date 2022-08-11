using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Sprites
{
    public class PlayerSprite : AnimatedSprite
    {
        private Vector2 _velocity = Vector2.Zero;
        public PlayerSprite(Texture2D texture, int rows, int columns) : base(texture, rows, columns)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Playing)
            {
                GameInfo.Instance.EncounterInfo.Position = _position;
            }
        }
    }
}
