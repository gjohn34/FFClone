using Microsoft.Xna.Framework.Input;

namespace FFClone.Helpers.Keyboard
{
    public static class KeyboardHelper
    {
        public static bool Released(this KeyboardState keyboard, KeyboardState previousKeyboard, Keys key)
        {
            return (previousKeyboard.IsKeyDown(key) && keyboard.IsKeyUp(key));
        }
    }
}

