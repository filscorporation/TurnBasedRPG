using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.ItemManagement
{
    public class GoldInventoryObject : IInventoryObject
    {
        private const string iconPath = "Icons/GoldIcon";
        private Sprite icon;
        public Sprite Icon => icon;

        public string Description => "gold_item_description";

        public GoldInventoryObject()
        {
            icon = Resources.Load<Sprite>(iconPath);
        }
    }
}
