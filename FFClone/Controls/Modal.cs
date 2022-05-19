using FFClone.Helpers.Shapes;
using FFClone.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Controls
{
    public class Modal : IComponent
    {
        public bool Open { get; set; }
        public Rectangle Rectangle { get; set; }
        private int _vH;
        private int _vW;
        private SpriteFont _font;
        private string _label;
        private MenuList _options;
        private EventHandler _onClose;
        private Vector2 _position;

        public Modal(SpriteFont font, string label, Rectangle gameWindow, Rectangle modalWindow, EventHandler onClose)
        {
            _onClose = onClose;
            Open = true;
            _vW = gameWindow.Width;
            _vH = gameWindow.Height;
            Rectangle = modalWindow;
            _font = font;
            _label = WrapText(label);
            _position = new Vector2(
                Rectangle.X + (((int)(Rectangle.Width - _font.MeasureString(_label).X) - (2 * 5)) / 2),
                Rectangle.Center.Y - (int)(0.5f * _font.MeasureString(_label).Y)
            );
        }
        private string WrapText(string text)
        {
            if (_font.MeasureString(text).X < Rectangle.Width)
                return text;

            string[] words = text.Split(' ');
            StringBuilder wrappedText = new StringBuilder();
            float linewidth = 0f;
            float spaceWidth = _font.MeasureString(" ").X;
            for (int i = 0; i < words.Length; ++i)
            {
                Vector2 size = _font.MeasureString(words[i]);
                if (linewidth + size.X < Rectangle.Width)
                    linewidth += size.X + spaceWidth;
                else
                {
                    wrappedText.Append("\n");
                    linewidth = size.X + spaceWidth;
                }
                wrappedText.Append(words[i]);
                wrappedText.Append(" ");
            }

            return wrappedText.ToString();
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(new Rectangle(0, 0, _vW, _vH), 0, Color.Black, Color.Black * 0.5f);
            spriteBatch.DrawRectangleWithFill(Rectangle, 5, Color.Black, Color.Salmon);
            spriteBatch.DrawString(_font, _label, _position, Color.Black);
        }

        public void Resized()
        {
        }

        public void Call()
        {
            _onClose.Invoke(this, EventArgs.Empty);
        }

        public void Update(GameTime gameTime)
        {
            //KeyboardState ks = Keyboard.GetState();

            //if (ks.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Escape))
            //{
            //    _onClose.Invoke(this, new EventArgs());
            //}
            //_previousKeyboard = ks;

        }
    }
    public class ConfirmationModal : IComponent
    {
        public bool Open { get; set; }
        public Rectangle Rectangle { get; set; }
        private int _vH;
        private int _vW;
        private SpriteFont _font;
        public string Label;
        private int _margin;
        private MenuList _options;

        public ConfirmationModal(SpriteFont font, string label, MenuList list, Rectangle gameWindow)
        {
            Open = true;
            _vW = gameWindow.Width;
            _vH = gameWindow.Height;
            Rectangle = new Rectangle((int)(0.3f * _vW), (int)(0.3f * _vH), (int)(0.3f * _vW), (int)(0.3f * _vH));
            _font = font;
            Label = WrapText(label);
            _margin = ((int)(Rectangle.Width - _font.MeasureString(Label).X) - (2 * 5)) / 2; // left and right border -5
            _options = list;
        }
        private string WrapText(string text)
        {
            if (_font.MeasureString(text).X < Rectangle.Width)
            {
                return text;
            }

            string[] words = text.Split(' ');
            StringBuilder wrappedText = new StringBuilder();
            float linewidth = 0f;
            float spaceWidth = _font.MeasureString(" ").X;
            for (int i = 0; i < words.Length; ++i)
            {
                Vector2 size = _font.MeasureString(words[i]);
                if (linewidth + size.X < Rectangle.Width)
                {
                    linewidth += size.X + spaceWidth;
                }
                else
                {
                    wrappedText.Append("\n");
                    linewidth = size.X + spaceWidth;
                }
                wrappedText.Append(words[i]);
                wrappedText.Append(" ");
            }

            return wrappedText.ToString();
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangleWithFill(new Rectangle(0,0, _vW, _vH), 0, Color.Black, Color.Black * 0.5f);
            spriteBatch.DrawRectangleWithFill(Rectangle, 5, Color.Black, Color.Salmon);
            spriteBatch.DrawString(_font, Label, new Vector2(Rectangle.X + _margin, Rectangle.Center.Y - _font.MeasureString(Label).Y), Color.Black);
            _options.Draw(gameTime, spriteBatch);
        }

        public void Resized()
        {
        }

        public void Update(GameTime gameTime)
        {
            _options.Update(gameTime);
        }
    }
}

