using FFClone.Controls;
using FFClone.Models;
using FFClone.Sprites;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFClone.Models
{
    public class Enemy : Character, IBattleable
    {
        public Item Reward;
        public List<Spell> Spells { get; set; }

        public int ExperienceGain {get; set; }
        public Enemy(string name, Color color, Dictionary<string, int> statBlock) : base(name, color, "Sprites/monster")
        {
            GenerateStats(statBlock);
            GenerateReward();
        }

        private void GenerateReward()
        {
            Reward = new Item(1, "A new car!", 1);
        }

        private void GenerateStats(Dictionary<string, int> statBlock)
        {
            HP = statBlock["HP"];
            MaxHP = statBlock["HP"];
            Strength = statBlock["STR"];
            Intelligence = statBlock["INT"];
            Dexterity = statBlock["DEX"];
            ExperienceGain = statBlock["EXP"];
        }
        public BattleSprite BattleSprite { get; set; }

        public int CalculateBattleDamage()
        {
            return 1;
        }

        public Vector2 HomePosition { get; set; }
        public Vector2 MoveByTick { get; set; }
        public Rectangle EndRectangle { get; set; }
        // TODO - Make this character only
        public List<Action> Options { get; set; } = new List<Action> { new Action("Attack") };
    }
}