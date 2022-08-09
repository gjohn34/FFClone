using FFClone.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace FFClone.Models
{
    [Serializable]
    public class Character
    {
        public Job Class { get; set; }
        public string Name { get; set; }
        public int Level { get; set; } = 1;
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public Color Color { get; set; }
        public string Path { get; set; }
        public bool Defending { get; set; }

        public Character(string name, Color color, string path)
        {
            Name = name;
            Color = color;
            Path = path;
        }
        public Character() { }

    }
}