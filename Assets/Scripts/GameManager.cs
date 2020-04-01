using Assets.Scripts.GameDataManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RoomsManagement;
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

        /// <summary>
        /// Game UUID to differ it from other saves
        /// </summary>
        public string GameID;

        public void Awake()
        {
            LoadSkillsDictionary();
            LoadItemsDictionary();
            LoadConsumablesDictionary();

            switch (GameParams.GameMode)
            {
                case GameMode.New:
                    GameID = Guid.NewGuid().ToString();
                    RoomManager.Instance.CurrentRoomIndex = 0;
                    RoomManager.Instance.SetRoomsCount(1);
                    RoomGenerator.Instance.GenerateRoom(new RoomParams(100, 100));
                    RoomGenerator.Instance.SpawnPlayer();
                    break;
                case GameMode.Loaded:
                    GameDataManager.Instance.Load(GameParams.GameFileToLoadName);
                    break;
                case GameMode.NextNewRoom:
                    GameDataManager.Instance.LoadPlayer(GameParams.SpawnDirection,
                                                        GameParams.CurrentRoomIndex,
                                                        GameParams.LeavedRoomIndex,
                                                        MainMenuManager.DefaultGameFileName);
                    break;
                case GameMode.NextExistingRoom:
                    GameDataManager.Instance.Load(GameParams.SpawnDirection,
                                                  GameParams.CurrentRoomIndex,
                                                  GameParams.GameFileToLoadName);
                    break;
                default:
                    throw new NotSupportedException(GameParams.GameMode.ToString());
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
