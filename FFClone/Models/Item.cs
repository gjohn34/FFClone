using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public interface IHealItem
    {
        public int HealBy { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Item(int id, string name, int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }
        public int Potency { get; set; } = 5;
    }
}
