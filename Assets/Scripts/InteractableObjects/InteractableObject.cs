using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement.UIElements;
using UnityEngine;

namespace Assets.Scripts.InteractableObjects
{
    /// <summary>
    /// Objects on the map which supports interations with the player
    /// </summary>
    public abstract class InteractableObject : MapObject
    {
        public GameObject interactButtonPrefab;

        protected Player Player;
        private InteractionButton button;
        private bool nowInteracting = false;

        protected abstract Sprite ButtonIcon { get; }

        public void Update()
        {
            // TODO: optimal?
            if (button != null)
            {
                button.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            }
        }

        /// <summary>
        /// Shows interation UI for the player
        /// </summary>
        public void ShowInteraction(Player player)
        {
            if (nowInteracting)
                return;

            nowInteracting = true;

            Transform parent = FindObjectOfType<Canvas>().transform;
            Vector2 position = Camera.main.WorldToScreenPoint(transform.position);
            GameObject go = Instantiate(interactButtonPrefab, position, Quaternion.identity, parent);
            button = go.GetComponent<InteractionButton>();
            button.Parent = this;
            button.Icon.sprite = ButtonIcon;
            this.Player = player;
        }

        /// <summary>
        /// Performs interaction
        /// </summary>
        protected abstract void Interact();

        /// <summary>
        /// Hides interaction UI
        /// </summary>
        public void Clear()
        {
            if (nowInteracting)
            {
                nowInteracting = false;
                Destroy(button.gameObject);
                Player = null;
            }
        }

        /// <summary>
        /// Handles click on interaction button
        /// </summary>
        public void HandleButtonClick()
        {
            Interact();
            Clear();
        }
    }
}
