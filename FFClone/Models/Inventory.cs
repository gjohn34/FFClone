using System;
using System.Collections.Generic;
using System.Text;

namespace FFClone.Models
{
    public class Inventory
    {
        public List<Item> Items { get; set; }
        public Inventory(List<Item> items)
        {
            Items = items;
        }
        //public List<IMenuItem> ToMenuList()
        //{
            
        //}
        public void Add(Item item)
        {
            int index = GetIndex(item);
            if (index == -1)
                return;

            Items[GetIndex(item)].Quantity += item.Quantity;
        }
        public void Remove(Item item)
        {
            int index = GetIndex(item);
            if (index == -1)
                return;

            Items[index].Quantity -= item.Quantity;
            if (Items[index].Quantity <= 0)
                Items.RemoveAt(index);

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
