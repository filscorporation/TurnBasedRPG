using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Touch input
    /// </summary>
    public class TouchInputManager : InputManagerBase
    {
        protected override void CheckForInput()
        {
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
            {
                if (ProcessInput(touch.position))
                    return;
            }
        }
    }
}
