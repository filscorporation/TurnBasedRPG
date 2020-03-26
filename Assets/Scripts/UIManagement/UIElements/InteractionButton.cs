using Assets.Scripts.InteractableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
{
    /// <summary>
    /// Button that pops up when player is close enough to interactable object
    /// </summary>
    public class InteractionButton : MonoBehaviour
    {
        public InteractableObject Parent;
        public Image Icon;

        /// <summary>
        /// Redirects button press to object
        /// </summary>
        public void HandleClick()
        {
            Parent.HandleButtonClick();
        }
    }
}
