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
using FFClone.Transitions;
using System.Reflection.Emit;

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
        private Inventory _inventory = GameInfo.Instance.Inventory;
        private MenuList _list;
        private List<IMenuOption> _portraits;
        private MenuList _portraitList;
        private Stack<IComponent> _stack;
        private ConfirmationModal _modal;
        private bool _noItems = false;
        private Texture2D _background;

        public ItemMenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = content.Load<Texture2D>("Sprites/Backgrounds/party-screen");

            _stack = new Stack<IComponent>();

            Rectangle modalR = new Rectangle((int)(0.33f * _vW), (int)(0.33f * _vH), (int)(0.33f * _vW), (int)(0.33f * _vH));

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

            List<IMenuOption> newList = _inventory.Items.ConvertAll<IMenuOption>(x => new MenuItem(x.Name, _font, (a, b) => Handler(x)));

            if (newList.Count > 0)
            {
                _list = new MenuList(
                    newList,
                    //_inventory.Items.ConvertAll<IMenuOption>(x => new MenuItem(x.Name, _font, (a,b) => Handler(x))),
                    new Rectangle((int)(0.1 * _vW), (int)(0.1 * _vH), _vW - (int)(0.2 * _vW), _vH - (int)(0.2 * _vH)),
                    _font,
                    //Handler,
                    Orientation.Horizontal,
                    3
                );
            } else
            {
                NoItems();

                //_stack.Push(new TestComponent(new Rectangle(0, 0, 100, 100), "none", _font));
            }
        }

        private void NoItems()
        {
            _stack.Clear();
            _list = null;
            string label = "No items left\nGo back?\n";
            List<IMenuOption> list = new List<IMenuOption>
                        {
                            new MenuItem("yes", _font, (a,b) => {
                                _stateManager.Back();
                                _stack.Pop();
                                }),
                            new MenuItem("no",_font, (a,b) =>
                                {
                                    _stack.Pop();
                                    _noItems = true;
                                })
                        };

            Rectangle r = new Rectangle((int)(0.33f * _vW), (int)(0.33f * _vH), (int)(0.33f * _vW), (int)(0.33f * _vH));

            Rectangle y = new Rectangle(
                r.X,
                r.Y + (int)(_font.MeasureString(label).Y),
                r.Width,
                (int)(r.Height - _font.MeasureString(label).Y)
            );

            MenuList menulist = new MenuList(list, y, _font, Orientation.Horizontal, list.Count);
            _stack.Push(new ConfirmationModal(
                _font,
                label,
                menulist,
                _game.Window.ClientBounds
            ));
        }
        public void Handler(Item item)
        {
            _portraits.ForEach(mo => mo.OnSubmit = (a,b) => { 
                ItemPortraitGroup ipg = (ItemPortraitGroup)mo;
                Debug.WriteLine($"Giving {item.Name} to {ipg.ItemPortrait.Hero.Name}");
                ipg.ItemPortrait.Hero.GiveItem(item);
                if (!_inventory.Remove(item))
                {
                    _stack.Pop();

                    List<IMenuOption> newList = _inventory.Items.ConvertAll<IMenuOption>(x => new MenuItem(x.Name, _font, (a, b) => Handler(x)));
                    if (newList.Count > 0)
                    {
                        _list.Refresh(newList);
                    } else
                    {
                        NoItems();
                    }
                    //_list = new MenuList(
                    //    _inventory.Items.ConvertAll<IMenuOption>(x => new MenuItem(x.Name, _font, (a, b) => Handler(x))),
                    //    new Rectangle((int)(0.1 * _vW), (int)(0.1 * _vH), _vW - (int)(0.2 * _vW), _vH - (int)(0.2 * _vH)),
                    //    _font,
                    //    //Handler,
                    //    Orientation.Horizontal,
                    //    3
                    //);

                } else
                {
                    _modal.Label = $"{item.Quantity} Left\ngive to who";
                }

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
            spriteBatch.Draw(_background, new Rectangle(0, 0, _vW, _vH), Color.White);

            if (_noItems)
            {
                spriteBatch.DrawRectangleWithFill(new Rectangle(0,0, _vW, _vH), 1, Color.Black, Color.White);
                spriteBatch.DrawString(
                    _font, 
                    "Empty Bag", 
                    new Vector2(
                        (0.5f * _vW) - (0.5f * _font.MeasureString("Empty Bag").X),
                        (0.5f * _vH) - (0.5f * _font.LineSpacing)), 
                    Color.Black);
            }
            if (_list != null)
            {
                _list.Draw(gameTime, spriteBatch);
            }

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
                if (_list != null)
            {
                _list.Update(gameTime);
            }

            _previousKeyboard = ks;
        }
    }
}