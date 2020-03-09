using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// PC mouse input
    /// </summary>
    public class MouseInputManager : InputManagerBase
    {
        protected override void CheckForInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInput(Input.mousePosition);
            }
        }
    }
}
