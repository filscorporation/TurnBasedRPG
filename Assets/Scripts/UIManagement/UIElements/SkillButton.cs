using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
{
    public class SkillButton : MonoBehaviour, IUIElement
    {
        public string SkillName;
        public string Name => $"{SkillName}Button";

        //private Animator animator;
        //private const string selectedParameterName = "Selected";

        public void Start()
        {
            //animator = GetComponent<Animator>();
        }

        public void SetValue(int value)
        {
            if (value == 0)
            {
                GetComponent<Button>().interactable = false;
                //animator.SetBool(selectedParameterName, false);
            }
            if (value == 1)
            {
                GetComponent<Button>().interactable = true;
                //animator.SetBool(selectedParameterName, false);
            }
            if (value == 2)
            {
                GetComponent<Button>().interactable = true;
                //animator.SetBool(selectedParameterName, true);
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
