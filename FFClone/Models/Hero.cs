using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public enum Job
    {
        Warrior,
        Mage,
        Thief
    }
    public class Hero
    {
        public string Name { get; }
        public Job Job { get; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }

        public Hero(string name, Job job)
        {
            Name = name;
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
    }

}
