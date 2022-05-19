using FFClone.Controls;
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
        public List<Action> Options { get; set; }
        public List<Spell> Spells { get; set; }

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
    [Serializable]
    public class Hero : Character, IBattleable
    {
        public string Portrait { get; set; }
        public Job Job { get; }
        public List<Spell> Spells { get; set; } = new List<Spell>();
        public List<Action> Options { get; set; }
        public int Experience { get; set; }
        public int ToNextLevel { get; set; } = 9;
        internal Dictionary<string, string> OldStats
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "Level", Level.ToString() },
                    { "STR", Strength.ToString() },
                    { "INT", Intelligence.ToString() },
                    { "DEX", Dexterity.ToString() },
                    { "HP", MaxHP.ToString() }
                };
            }
        }
        public Hero(string name, Job job, Color color, string path, string portraitPath) : base(name, color, path)
        {
            Job = job;
            SetUpOptions();
            if (Job == Job.Mage)
            {
                //Spells = new List<string> { "Fireball", "Ice Bolt", "Thunder"};
                Spells = new List<Spell> {
                    new Spell("Fireball"),
                    new Spell("Thunder"),
                    new Spell("Aero")
                };
                    //"Fireball", "Ice Bolt", "Thunder", "Aero", "Blizzard", "Darkness", "Holy Lance", "Water Whip", "Mind Blast", "Heal", "Flizz", "Biz", "Bang" };
            }
            GenerateStats();
            Portrait = portraitPath;
        }
        private Hero() { }

        private void SetUpOptions()
        {
            Options = new List<Action> { 
                new Action("Attack"),
                new Action("Defend")
            };
            if (Job == Job.Mage)
            {
                Options.Add(new Action("Spell"));
            }
            //Options = Job switch
            //{
            //    Job.Warrior => new List<string> { "Attack", "Defend", "Ability" },
            //    Job.Mage => new List<string> { "Attack", "Defend", "Spell", "Ability" },
            //    Job.Thief => new List<string> { "Attack", "Defend", "Ability" },
            //    _ => new List<string> { "Attack" },
            //};
        }


        internal bool IncreaseExperience(int experience)
        {
            Experience += experience;
            if (Experience >= ToNextLevel)
            {
                LevelUp();
                Experience -= ToNextLevel;
                ToNextLevel = 9;
                return true;
            }
            return false;
        }

        internal string Stat(string y)
        {
            return y switch
            {
                "STR" => Strength.ToString(),
                "INT" => Intelligence.ToString(),
                "DEX" => Dexterity.ToString(),
                "Level" => Level.ToString(),
                "HP" => MaxHP.ToString(),
                _ => "0",
            };
        }

        internal void LevelUp()
        {
            Level += 1;
            Strength += 1;
            Intelligence += 1;
            Dexterity += 1;
            HP += 5;
            MaxHP += 5;
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
                    HP = 10;
                    break;
                case Job.Mage:
                    Strength = 1;
                    Dexterity = 1;
                    Intelligence = 5;
                    MaxHP = 10;
                    HP = 8;
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
        public BattleSprite BattleSprite { get; set ; }
        public Vector2 HomePosition { get; set; }
        public Vector2 MoveByTick { get; set; }
        public Rectangle EndRectangle { get; set; }

        public int CalculateBattleDamage()
        {
            return 1;
        }

        internal void GiveItem(Item item)
        {
            item.Quantity -= 1;
            HP += item.Potency;
            if (HP > MaxHP)
            {
                HP = MaxHP;
            }
        }
    }

}
