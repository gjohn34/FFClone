using FFClone.Models;
using FFClone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MonoGame;
using FFClone.Helpers.Shapes;

namespace FFClone.States
{
    public class ItemPortraitGroup : IMenuOption
    {
        private int _height;
        private SpriteFont _font;

        public string Label { get; set; }
        public EventHandler OnSubmit { get; set; }
        public bool Selected { get; set; }
        public bool Pressed { get; set; }
        private Rectangle _rectangle;
        
        public Rectangle Rectangle 
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;
                ItemPortrait.Rectangle = value;
            }
        }
        //public Rectangle Rectangle { get {; set; }
        public ItemPortrait ItemPortrait { get; set; }

        public ItemPortraitGroup(Texture2D portrait, int height, Hero hero, /*EventHandler onSubmit,*/ SpriteFont spriteFont)
        {
            Label = hero.Name;
            ItemPortrait = new ItemPortrait(portrait, height, hero, spriteFont);
            //OnSubmit = onSubmit;
            _height = height;
            _font = spriteFont;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(Rectangle, Color.Red);
            ItemPortrait.Draw(gameTime, spriteBatch);
        }

        public void Resized()
        {
        }

        public void Update(GameTime gameTime)
        {

            if (Pressed)
            {
                OnSubmit?.Invoke(this, new EventArgs());
                Pressed = false;
            }
        }
    }
    public class ItemPortrait : IMenuOption, IComponent
    {
        public string Label { get; set; }
        public EventHandler OnSubmit { get; set; }
        public Rectangle Rectangle { get; set; }
        public Texture2D Portrait { get; }
        public Hero Hero { get; }

        private int _height;
        private SpriteFont _font;

        public bool Selected { get; set; }
        public bool Pressed { get; set; }

        public ItemPortrait(Texture2D portrait, int height, Hero hero, SpriteFont spriteFont)
        {
            Label = hero.Name;
            Portrait = portrait;
            Hero = hero;
            _height = height;
            _font = spriteFont;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Portrait, 
                new Rectangle(
                    Rectangle.X - (int)(0.25f * _height),
                    Rectangle.Y - (int)(0.5f * _height),
                    _height, 
                    _height
                    ), 
                Color.White
            );
            spriteBatch.ProgressBar(_font, "HP", Hero.HP, Hero.MaxHP, Color.Green, Color.Red, new Rectangle(Rectangle.X - (int)(0.25f * _height), Rectangle.Y + (int)(0.1f * _height), _height, (int)(0.3f * _height)), false);
        }

        public void Update(GameTime gameTime)
        {
            if (Pressed)
            {
                OnSubmit?.Invoke(this, new EventArgs());
                Pressed = false;
            }

        }

        public void Resized()
        {
        }
    }
    internal class ItemMenuState : State
    {
        private List<Hero> _party = GameInfo.Instance.Party;
        private MenuList _list;
        private List<IMenuOption> _portraits;
        private MenuList _portraitList;
        private Stack<IComponent> _stack;
        private ConfirmationModal _modal;

        public ItemMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _stack = new Stack<IComponent>();

            Rectangle modalR = new Rectangle((int)(0.3f * _vW), (int)(0.3f * _vH), (int)(0.3f * _vW), (int)(0.3f * _vH));

            Rectangle portraitsGroupRectangle = new Rectangle(
                modalR.X,
                modalR.Center.Y,
                modalR.Width,
                (int)(0.5f * modalR.Height)
            );

            _portraits = _party.ConvertAll<IMenuOption>(x => new ItemPortraitGroup(
                _content.Load<Texture2D>(x.Portrait),
                (int)(0.7 * portraitsGroupRectangle.Height),
                x,
                _font
            ));

            _portraitList = new MenuList(_portraits, portraitsGroupRectangle, _font, Orientation.Horizontal, _portraits.Count);


            _list = new MenuList(
                GameInfo.Instance.Inventory.Items.ConvertAll<IMenuOption>(x => new MenuItem(x.Name, _font, (a,b) => Handler(x))),
                new Rectangle((int)(0.1 * _vW), (int)(0.1 * _vH), _vW - (int)(0.2 * _vW), _vH - (int)(0.2 * _vH)),
                _font,
                //Handler,
                Orientation.Horizontal,
                3
            );
        }

        public void Handler(Item item)
        {
            _portraits.ForEach(mo => mo.OnSubmit = (a,b) => { 
                ItemPortraitGroup ipg = (ItemPortraitGroup)mo;
                Debug.WriteLine($"Giving {item.Name} to {ipg.ItemPortrait.Hero.Name}");
                ipg.ItemPortrait.Hero.GiveItem(item);
                _modal.Label = $"{item.Quantity} Left\ngive to who";
            });

            _modal = new ConfirmationModal(
                    _font,
                    $"{item.Quantity} Left\ngive to who",
                    _portraitList,
                    _game.Window.ClientBounds
            );
            
            _stack.Push(_modal);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _list.Draw(gameTime, spriteBatch);

            if (_stack.Count > 0)
            {
                _stack.Peek().Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (_previousKeyboard.IsKeyDown(Keys.Escape) && ks.IsKeyUp(Keys.Escape))
            {
                if (_stack.Count > 0)
                {
                    _stack.Pop();
                }
                else
                {
                    _stateManager.Back();
                }
            }
            if (_stack.Count > 0)
                _stack.Peek().Update(gameTime);
            else
                _list.Update(gameTime);

            _previousKeyboard = ks;
        }
    }
}