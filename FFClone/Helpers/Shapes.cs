using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using System;
using System.Runtime.InteropServices;

namespace FFClone.Helpers.Shapes
{
    public static class ShapeHelper
    {
        public static int VerticalPosition(this Rectangle rectangle, int rows, int rowHeight)
        {
            int yPosition = rectangle.Y;
            int height = rectangle.Height;
            int spaceTaken = rows * rowHeight;
            int availableHeight = height - spaceTaken;
            int yMargin = availableHeight / rows;
            return yPosition + yMargin;
        }
        public static int HorizontalPosition(this Rectangle rectangle)
        {
            int availableSpace = rectangle.Width;
            int xCenterBuffer = availableSpace / 3;
            int leftMargin = rectangle.X + (int)(0.5 * xCenterBuffer);
            return leftMargin;
        }
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
        public static void ProgressBar(this SpriteBatch spriteBatch, SpriteFont font, string label, int starting, int ending, Color leftFill, Color rightFill, Rectangle rectangle, bool showLabel = true)
        {
            if (showLabel)
            {
                spriteBatch.DrawString(font, $"{label} : {starting}/{ending}", new Vector2(rectangle.X, rectangle.Y), Color.Black);
            }
            int barWidth = rectangle.Width;
            int width = (int)(((double)starting / ending) * barWidth);
            int barHeight = rectangle.Height;
            //hp left
            spriteBatch.DrawRectangleWithFill(new Rectangle(rectangle.X, rectangle.Y + font.LineSpacing, width, barHeight), 0, leftFill, leftFill);
            ////hp missing
            spriteBatch.DrawRectangleWithFill(new Rectangle(rectangle.X + width, rectangle.Y + font.LineSpacing, barWidth - width, barHeight), 0, rightFill, rightFill);
            //hp bar outline
            spriteBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + font.LineSpacing, rectangle.Width, rectangle.Height), Color.Black);
        }
    }
}

