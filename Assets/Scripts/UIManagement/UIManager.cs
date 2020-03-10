using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.PlayerManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Controlls all UI interation
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<UIManager>();
                return instance;
            }
        }

        private readonly Dictionary<string, IUISubscriber> subs = new Dictionary<string, IUISubscriber>();

        private Dictionary<string, IUIElement> elements;

        public void Start()
        {
            LoadAllElements();
        }

        private void LoadAllElements()
        {
            Type uiElementType = typeof(IUIElement);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => uiElementType.IsAssignableFrom(p) && uiElementType != p);
            elements = new Dictionary<string, IUIElement>();
            foreach (Type type in types)
            {
                foreach (Object o in Resources.FindObjectsOfTypeAll(type))
                {
                    if (o is IUIElement e)
                    {
                        elements[e.Name] = e;
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to button events
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="sub"></param>
        public void Subscribe(string buttonName, IUISubscriber sub)
        {
            subs[buttonName] = sub;
        }

        /// <summary>
        /// Sends button click event to subscribers
        /// </summary>
        /// <param name="button"></param>
        public void HandleButtonClick(string button)
        {
            subs[button].Handle(new UIEvent { ButtonName = button });
        }

        /// <summary>
        /// Finds element attached to the variable and sets it value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetVariable(string variable, int value)
        {
            elements[variable].SetValue(value);
        }

        /// <summary>
        /// Finds element attached to the variable and sets it value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetVariable(string variable, string value)
        {
            elements[variable].SetValue(value);
        }
    }
}
