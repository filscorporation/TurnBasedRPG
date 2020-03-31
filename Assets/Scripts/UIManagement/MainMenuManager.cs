using Assets.Scripts.GameDataManagement;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Controls buttons in main menu: new game, load, etc..
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public const string GameSceneName = "RoomScene";
        public const string MainMenuSceneName = "MainMenu";
        public const string DefaultGameFileName = "GameSave01";

        public Button LoadButton;

        public void Start()
        {
            if (!File.Exists(Path.Combine(Application.persistentDataPath, DefaultGameFileName)))
            {
                // Disable load button if there is no save yet
                LoadButton.interactable = false;
            }
        }

        /// <summary>
        /// Starts new game
        /// </summary>
        public void NewGame()
        {
            GameParams.GameMode = GameMode.New;
            SceneManager.LoadScene(GameSceneName);
        }

        /// <summary>
        /// Loads default save
        /// </summary>
        public void LoadGame()
        {
            GameParams.GameMode = GameMode.Loaded;
            GameParams.GameFileToLoadName = DefaultGameFileName;
            SceneManager.LoadScene(GameSceneName);
        }
    }
}
