using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    [Serializable]
    public class EncounterInfo
    {
        public double Ticks = 0;
        public double Chance = 0;
        public Vector2 Position { get; set; } 
        public Vector2 MapPosition { get; set; } = Vector2.Zero;
        public EncounterInfo() { }

        internal void Reset()
        {
            Ticks = 0;
            Chance = 0;
        }
    }
}
