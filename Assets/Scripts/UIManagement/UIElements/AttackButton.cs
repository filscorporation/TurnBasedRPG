using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
{
    public class AttackButton : MonoBehaviour, IUIElement
    {
        public string Name { get; } = "AttackButton";

        private Animator animator;
        private const string selectedParameterName = "Selected";

        public void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetValue(int value)
        {
            if (value == 0)
            {
                GetComponent<Button>().interactable = false;
                animator.SetBool(selectedParameterName, false);
            }
            if (value == 1)
            {
                GetComponent<Button>().interactable = true;
                animator.SetBool(selectedParameterName, false);
            }
            if (value == 2)
            {
                GetComponent<Button>().interactable = true;
                animator.SetBool(selectedParameterName, true);
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
