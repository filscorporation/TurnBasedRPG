using System.Collections.Generic;
using Assets.Scripts.ItemManagement;
using UnityEngine;

namespace Assets.Scripts.InteractableObjects
{
    /// <summary>
    /// Chest
    /// </summary>
    public class Chest : InteractableObject
    {
        public bool IsLooted = false;
        public List<string> Items;

        public Sprite PickIcon;
        protected override Sprite ButtonIcon => PickIcon;

        protected override void Interact()
        {
            if (IsLooted)
                return;

            IsLooted = true;
            // TODO: temporary, show UI
            foreach (string item in Items)
            {
                if (Consumable.ConsumablesDictionary.TryGetValue(item, out Consumable consumable))
                    Player.Inventory.Add(consumable);
                else
                    Player.Inventory.Add(Item.ItemDictionary[item]);
            }
        }
    }
}
