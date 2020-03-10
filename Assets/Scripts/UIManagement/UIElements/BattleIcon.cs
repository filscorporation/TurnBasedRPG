using System;
using UnityEngine;

namespace Assets.Scripts.UIManagement.UIElements
{
    public class BattleIcon : MonoBehaviour, IUIElement
    {
        public string Name { get; } = "BattleIcon";

        public void SetValue(int value)
        {
            if (value == 1)
                gameObject.SetActive(true);
            if (value == 0)
                gameObject.SetActive(false);
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
