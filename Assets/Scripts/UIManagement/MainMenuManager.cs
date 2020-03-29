using Assets.Scripts.GameDataManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        /// <summary>
        /// Starts new game
        /// </summary>
        public void NewGame()
        {
            GameParams.NewGame = true;
            SceneManager.LoadScene(GameSceneName);
        }

        /// <summary>
        /// Loads default save
        /// </summary>
        public void LoadGame()
        {
            GameParams.NewGame = false;
            GameParams.GameFileToLoadName = DefaultGameFileName;
            SceneManager.LoadScene(GameSceneName);
        }
    }
}
