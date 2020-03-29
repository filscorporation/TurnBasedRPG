using Assets.Scripts.InputManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Handles ingame menu
    /// </summary>
    public class InGameMenu : MonoBehaviour, IUISubscriber
    {
        private const string ingameMenuButtonName = "MenuButton";

        public GameObject MenuButton;
        public GameObject ContinueButton;
        public GameObject SaveAndLoadButton;

        public void Start()
        {
            UIManager.Instance.Subscribe(ingameMenuButtonName, this);
        }

        public void Handle(UIEvent uiEvent)
        {
            if (uiEvent.ButtonName == ingameMenuButtonName)
            {
                Show();
            }
        }

        private void Hide()
        {
            MenuButton.SetActive(true);

            ContinueButton.SetActive(false);
            SaveAndLoadButton.SetActive(false);
            AutoInputInitializer.InputManager.IsNeedToCheckForInput = true;
        }

        private void Show()
        {
            MenuButton.SetActive(false);

            ContinueButton.SetActive(true);
            SaveAndLoadButton.SetActive(true);
            AutoInputInitializer.InputManager.IsNeedToCheckForInput = false;
        }

        public void HandleContinue()
        {
            Hide();
        }

        public void HandleSaveAndExit()
        {
            GameManager.Instance.SaveAndExit();
            // TODO: show why cant save and exit (battle, movement)
        }
    }
}
