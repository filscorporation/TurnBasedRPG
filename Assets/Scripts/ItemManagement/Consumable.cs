using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.ItemManagement
{
    public abstract class Consumable : IInventoryObject
    {
        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract string Description { get; }
    }
}
