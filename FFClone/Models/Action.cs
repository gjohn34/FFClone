namespace FFClone.Models
{
    public class Action
    {
        public string Name { get; }
        public int BaseDamage { get; set; } = 1;
        public Action(string name)
        {
            Name = name;
        }
    }
    public class Spell : Action
    {
        public int Cost { get; set; } = 1;
        public Spell(string name) : base(name)
        {
        }
    }
    public class Ability : Action
    {
        public Ability(string name) : base(name)
        {
        }
    }
}