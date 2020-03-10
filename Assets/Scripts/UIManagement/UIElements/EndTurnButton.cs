using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
{
    public class EndTurnButton : MonoBehaviour, IUIElement
    {
        public string Name { get; } = "EndTurnButton";

        private Animator animator;
        private const string waitParameterName = "Wait";

        public void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetValue(int value)
        {
            if (value == 1)
            {
                GetComponent<Button>().interactable = true;
                animator.SetBool(waitParameterName, false);
            }
            if (value == 0)
            {
                GetComponent<Button>().interactable = false;
                animator.SetBool(waitParameterName, false);
            }
            if (value == 2)
            {
                GetComponent<Button>().interactable = true;
                animator.SetBool(waitParameterName, true);
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
