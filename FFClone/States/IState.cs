using Microsoft.Xna.Framework;

namespace FFClone
{
    public interface IState
    {
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}