using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items { get; set; }
        public Inventory(List<Item> items)
        {
            Items = items;
        }
        public Inventory()
        {
            Items = new List<Item>();
        }
        public void Add(Item item)
        {
            int index = GetIndex(item);
            if (index == -1)
            {
                Items.Add(item);
                //return;
            } else
            {
                Items[index].Quantity += item.Quantity;
            }
        }
        public void Merge(Inventory subInventory)
        {
            subInventory.Items.ForEach(x => Add(x));
        }

        public bool Remove(Item item)
        {
            int index = GetIndex(item);
            if (index == -1)
                return false;

            Items[index].Quantity -= 1;
            if (Items[index].Quantity <= 0)
            {
                Items.RemoveAt(index);
                return false;
            }
            return true;

        }
        private bool HasItem(Item item)
        {
            return Items.Exists(x => item.Id == x.Id);
        }
        private int GetIndex(Item item)
        {
            return Items.FindIndex(x => item.Id == x.Id);
        }
    }
}
