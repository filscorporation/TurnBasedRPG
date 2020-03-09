using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement
{
    public class EndTurnButton : MonoBehaviour, IUIElement
    {
        public string Name { get; } = "EndTurnButton";

        public void SetValue(int value)
        {
            if (value == 1)
                GetComponent<Button>().interactable = true;
            if (value == 0)
                GetComponent<Button>().interactable = false;
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
