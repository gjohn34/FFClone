using System;

namespace FFClone.Models
{
    [Serializable]
    public class Action
    {
        public string Name { get; set; }
        public int BaseDamage { get; set; } = 1;
        public Action(string name)
        {
            Name = name;
        }
        public Action() { }
    }
    [Serializable]
    public class Spell : Action
    {
        public int Cost { get; set; } = 1;
        public Spell(string name) : base(name)
        {
        }
        public Spell() : base() {}
    }
    [Serializable]
    public class Ability : Action
    {
        public Ability(string name) : base(name)
        {
        }
    }
}