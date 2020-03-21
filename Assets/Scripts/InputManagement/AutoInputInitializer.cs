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
        public void Start()
        {
            InputManagerBase im;
            switch (SystemInfo.deviceType)
            {
                case DeviceType.Handheld:
                    im = gameObject.AddComponent<TouchInputManager>();
                    break;
                case DeviceType.Desktop:
                    im = gameObject.AddComponent<MouseInputManager>();
                    break;
                case DeviceType.Console:
                case DeviceType.Unknown:
                    throw new NotSupportedException(SystemInfo.deviceType.ToString());
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Reference
            FindObjectOfType<PlayerController>().InputManager = im;
        }
    }
}
