using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.InteractableObjects
{
    /// <summary>
    /// Entance to next level on the map
    /// </summary>
    public class Entrance : InteractableObject
    {
        public Sprite EntranceIcon;
        protected override Sprite ButtonIcon => EntranceIcon;

        protected override void Interact()
        {
            // TODO: temp
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
