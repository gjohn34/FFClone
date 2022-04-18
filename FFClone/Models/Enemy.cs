using FFClone.Models;
using FFClone.Sprites;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.Models
{
    public class Enemy : Character
    {
        public AnimatedSprite BattleSprite { get; set; }

        public Enemy(string name, Color color, Dictionary<string, int> statBlock) : base(name, color, "Sprites/monster")
        {
            GenerateStats(statBlock);
        }

        private void GenerateStats(Dictionary<string, int> statBlock)
        {
            HP = statBlock["HP"];
            MaxHP = statBlock["HP"];
            Strength = statBlock["STR"];
            Intelligence = statBlock["INT"];
            Dexterity = statBlock["DEX"];
        }
    }
}