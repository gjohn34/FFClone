using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace FFClone.Models
{
    public class Character
    {
        public string Name { get; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public Color Color { get; set; }
        public string Path { get; set; }
        public Character(string name, Color color, string path)
        {
            Name = name;
            Color = color;
            Path = path;
        }
    }
}