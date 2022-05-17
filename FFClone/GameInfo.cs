using FFClone.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone
{
    public sealed class GameInfo
    {
        private static GameInfo instance = null;
        private static readonly object padlock = new object();
        public List<Hero> Party { get; private set; }
        public EncounterInfo EncounterInfo { get; private set; } = new EncounterInfo();
        internal SaveGame ToSave => new SaveGame { Party = Party, EncounterInfo = EncounterInfo };
        public Inventory Inventory { get; set; }

        GameInfo()
        {
        }

        public static GameInfo Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameInfo();
                    }
                    return instance;
                }
            }
        }
        public void Initialize(SaveGame saveData)
        {
            Party = saveData.Party;
            EncounterInfo = saveData.EncounterInfo;
            Inventory = new Inventory(new List<Item>()
            {
                new Item(1, "Potion", 10),
                new Item(2, "Hi-Potion", 3),
                new Item(4, "Phoenix Down", 2)
            });
        }
    }

}
