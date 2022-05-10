using FFClone.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public interface IBattleable
    {
        public int CalculateBattleDamage();
        public BattleSprite BattleSprite { get; set; }
        public bool Defending { get; set; }
        public string Name { get; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public List<string> Options { get; set; }
        public List<string> Spells { get; set; }

        public Vector2 HomePosition { get; set; }
        public Vector2 MoveByTick { get; set; }
        public Rectangle EndRectangle { get; set; }
        public Vector2 GenerateMoveTo(string movingLeftOrRight, Vector2 position, int width)
        {
            if (movingLeftOrRight == "left")
            {
                return new Vector2(position.X - width);
            } else
            {
                return new Vector2(position.X + width);
            }
        }
    }

    public enum Job
    {
        Warrior,
        Mage,
        Thief
    }
    public class Hero : Character, IBattleable
    {
        public string Portrait { get; }
        public Job Job { get; }
        public List<string> Spells { get; set; } = new List<string>();
        public List<string> Options { get; set; }
        public int Experience { get; set; }
        public int ToNextLevel { get; set; } = 100;

        public Hero(string name, Job job, Color color, string path, string portraitPath) : base(name, color, path)
        {
            Job = job;
            SetUpOptions();
            if (Job == Job.Mage)
            {
                //Spells = new List<string> { "Fireball", "Ice Bolt", "Thunder"};
                Spells = new List<string> { "Fireball", "Ice Bolt", "Thunder", "Aero", "Blizzard", "Darkness", "Holy Lance", "Water Whip", "Mind Blast", "Heal", "Flizz", "Biz", "Bang" };
            }
            GenerateStats();
            Portrait = portraitPath;
        }

        private void SetUpOptions()
        {
            Options = Job switch
            {
                Job.Warrior => new List<string> { "Attack", "Defend", "Ability" },
                Job.Mage => new List<string> { "Attack", "Defend", "Spell", "Ability" },
                Job.Thief => new List<string> { "Attack", "Defend", "Ability" },
                _ => new List<string> { "Attack" },
            };
        }

        private void GenerateStats()
        {
            switch (Job)
            {
                case Job.Warrior:
                    Strength = 4;
                    Dexterity = 2;
                    Intelligence = 1;
                    MaxHP = 20;
                    HP = 20;
                    break;
                case Job.Mage:
                    Strength = 1;
                    Dexterity = 1;
                    Intelligence = 5;
                    MaxHP = 10;
                    HP = 10;
                    break;
                case Job.Thief:
                    Strength = 2;
                    Dexterity = 4;
                    Intelligence = 1;
                    MaxHP = 15;
                    HP = 15;
                    break;
                default:
                    throw new Exception();
            }
        }
        //private int CalculateBattleDamage()
        //{
        //    int baseAttackDamage = 1;
        //    return Class switch
        //    {
        //        Job.Warrior => baseAttackDamage + (3 * Strength) + Dexterity,
        //        Job.Mage => baseAttackDamage + (int)(0.5 * (Strength + Dexterity)),
        //        Job.Thief => baseAttackDamage + Strength + (2 * Dexterity),
        //        _ => baseAttackDamage,
        //    };
        //}
        public BattleSprite BattleSprite { get; set ; }
        public Vector2 HomePosition { get; set; }
        public Vector2 MoveByTick { get; set; }
        public Rectangle EndRectangle { get; set; }

        public int CalculateBattleDamage()
        {
            return 1;
        }
    }

}
