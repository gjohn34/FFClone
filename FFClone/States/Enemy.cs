using Microsoft.Xna.Framework;

namespace FFClone.States
{
    public class Enemy
    {
        public Color Color { get; set; }
        public Enemy(Color color)
        {
            Color = color;
        }
    }
}