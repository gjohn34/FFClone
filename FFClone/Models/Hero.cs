using FFClone.Sprites;
using Microsoft.Xna.Framework;
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
        public Job Job { get; }

        public Hero(string name, Job job, Color color, string path) : base(name, color, path)
        {
            Job = job;
            GenerateStats();
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
