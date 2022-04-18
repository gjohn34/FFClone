using FFClone.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public enum Actions
    {
        Attack,
        Defend,
        Spell
    }
    internal class Action
    {

    }
    public enum Job
    {
        Warrior,
        Mage,
        Thief
    }
    public class Hero : Character
    {
        public Job Job { get; }

        public Hero(string name, Job job, Color color, string path) : base(name, color, path)
        {
            Job = job;
            GenerateStats();
        }
        public bool Defending { get; set; }
        public AnimatedSprite BattleSprite { get; set; }

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
    }

}
