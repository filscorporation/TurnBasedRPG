using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement.Tabs;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Controles all game processes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public RoomGenerator RoomGenerator;

        public void Awake()
        {
            RoomGenerator.GenerateRoom(new RoomParams(100, 100));
            LoadSkills();
            LoadItems();
            LoadConsumables();
        }

        private void LoadSkills()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                Skill skill = (Skill)Activator.CreateInstance(type);
                Skill.SkillDictionary[skill.Name] = skill;
            }
        }

        private void LoadItems()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Item)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Item))))
            {
                Item item = (Item)Activator.CreateInstance(type);
                Item.ItemDictionary[item.Name] = item;
            }
        }

        private void LoadConsumables()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Consumable)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Consumable))))
            {
                Consumable item = (Consumable)Activator.CreateInstance(type);
                Consumable.ConsumablesDictionary[item.Name] = item;
            }
        }
    }
}
