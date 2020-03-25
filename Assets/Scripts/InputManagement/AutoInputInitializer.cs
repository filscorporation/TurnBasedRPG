using System;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.InputManagement
{
    /// <summary>
    /// Initializing input according to device type and references it to components
    /// </summary>
    public class AutoInputInitializer : MonoBehaviour
    {
        /// <summary>
        /// Used input system
        /// </summary>
        public static InputManagerBase InputManager;

        public void Awake()
        {
            if (InputManager != null)
                throw new Exception("Multiple input manager initialization");

            switch (SystemInfo.deviceType)
            {
                case DeviceType.Handheld:
                    InputManager = gameObject.AddComponent<TouchInputManager>();
                    break;
                case DeviceType.Desktop:
                    InputManager = gameObject.AddComponent<MouseInputManager>();
                    break;
                case DeviceType.Console:
                case DeviceType.Unknown:
                    throw new NotSupportedException(SystemInfo.deviceType.ToString());
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Reference
            FindObjectOfType<PlayerController>().InputManager = InputManager;
        }
    }
}
