using Assets.Scripts.GameDataManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement;
using Assets.Scripts.UIManagement.Tabs;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    /// <summary>
    /// Controles all game processes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GameManager>();
                return instance;
            }
        }

        public RoomGenerator RoomGenerator;

        public void Awake()
        {
            LoadSkillsDictionary();
            LoadItemsDictionary();
            LoadConsumablesDictionary();

            if (GameParams.NewGame)
            {
                RoomGenerator.GenerateRoom(new RoomParams(100, 100));
            }
            else
            {
                GameDataManager.Instance.Load(GameParams.GameFileToLoadName);
            }
        }

        /// <summary>
        /// Tries to save the game and exit to main menu
        /// </summary>
        public void SaveAndExit()
        {
            // Saving only outside of battle and movement
            Player player = FindObjectOfType<Player>();
            if (player.PlayerState == PlayerState.FreeControl && player.State == CharactersManagement.CharacterState.Idle)
            {
                GameDataManager.Instance.Save(MainMenuManager.DefaultGameFileName);
                SceneManager.LoadScene(MainMenuManager.MainMenuSceneName);
            }

            Debug.Log("Can't save in battle and movement");
        }

        private void LoadSkillsDictionary()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                Skill skill = (Skill)Activator.CreateInstance(type);
                Skill.SkillDictionary[skill.Name] = skill;
            }
        }

        private void LoadItemsDictionary()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Item)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Item))))
            {
                Item item = (Item)Activator.CreateInstance(type);
                Item.ItemDictionary[item.Name] = item;
            }
        }

        private void LoadConsumablesDictionary()
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
