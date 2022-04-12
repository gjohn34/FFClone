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
        public readonly List<Hero> Party;

        GameInfo()
        {
            Party = new List<Hero> { new Hero("John", Job.Warrior, Color.Red), new Hero("Luke", Job.Mage, Color.Blue), new Hero("Paul", Job.Thief, Color.Green) };

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

        internal void Shuffle()
        {
            var temp = Party[0];
            Party[0] = Party[2];
            Party[2] = temp;
        }
    }

}
