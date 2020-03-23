using Assets.Scripts.ItemManagement;
using Assets.Scripts.SkillManagement;
using System;

namespace Assets.Scripts.EventManagement
{
    public enum InventoryAction
    {
        Added,
        Removed,
    }

    /// <summary>
    /// Info about changes in inventory
    /// </summary>
    public class InventoryEventData : EventArgs
    {
        InventoryAction InventoryAction;

        public Skill Skill;

        public Item Item;

        public Consumable Consumable;

        public InventoryEventData(Consumable consumable, InventoryAction action)
        {
            Consumable = consumable;
            InventoryAction = action;
        }

        public InventoryEventData(Item item, InventoryAction action)
        {
            Item = item;
            InventoryAction = action;
        }

        public InventoryEventData(Skill skill, InventoryAction action)
        {
            Skill = skill;
            InventoryAction = action;
        }
    }
}
