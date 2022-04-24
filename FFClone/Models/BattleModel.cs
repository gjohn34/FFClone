using FFClone.States.Battle.BattleViews;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public class BattleModel
    {
        public List<Hero> Party { get; set; }
        public List<Enemy> Enemies { get; set; }

        public bool EnemiesDefeated
        {
            get
            {
                return Enemies.TrueForAll(h => h.HP <= 0);
            }
        }
        public bool PartyDefeated
        {
            get
            {
                return Party.TrueForAll(h => h.HP <= 0);
            }
        }

        public bool BattleOver
        {
            get
            {
                return EnemiesDefeated || PartyDefeated;
            }
        }
        public BattleModel(List<Hero> party, List<Enemy> enemies)
        {
            Party = party;
            Enemies = enemies;
        }
    }
}
