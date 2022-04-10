using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;

namespace FFClone.Helpers.Shapes
{
    public static class RectangleHelper
    {
        public static void DrawRectangleWithFill(this SpriteBatch spriteBatch, Rectangle rectangle, int thickness, Color outline, Color fill)
        {

            // upper left to right
            spriteBatch.DrawLine(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y, outline, thickness);
            // upeer right to lower right
            spriteBatch.DrawLine(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, outline, thickness);
            // upper left to lower left
            spriteBatch.DrawLine(rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height, outline, thickness);
            // lower left to right
            spriteBatch.DrawLine(rectangle.X - thickness, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, outline, thickness);

            // fill
            spriteBatch.FillRectangle(new Rectangle(rectangle.X, rectangle.Y + thickness, rectangle.Width - thickness, rectangle.Height - thickness), fill);
        }
    }
}
