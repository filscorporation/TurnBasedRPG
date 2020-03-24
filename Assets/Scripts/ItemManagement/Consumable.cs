using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement.Tabs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ItemManagement
{
    /// <summary>
    /// Items with its own effect when used
    /// </summary>
    public abstract class Consumable : IInventoryObject
    {
        public static Dictionary<string, Consumable> ConsumablesDictionary = new Dictionary<string, Consumable>();
        
        public abstract string Name { get; }
        protected abstract string iconPath { get; }
        public Sprite Icon { get; set; }
        public abstract string Description { get; }
        public abstract Skill UsageEffect { get; }

        public Consumable()
        {
            LoadResources();
        }

        /// <summary>
        /// Loads objects icon and prefabs
        /// </summary>
        public virtual void LoadResources()
        {
            if (!string.IsNullOrWhiteSpace(iconPath))
                Icon = Resources.Load<Sprite>(iconPath);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
