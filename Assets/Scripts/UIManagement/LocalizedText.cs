using Assets.Scripts.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// Localizes text value of the component it is attached to
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        private Text text;

        public void Start()
        {
            text = GetComponent<Text>();
            text.text = LocalizationManager.GetLocalizedValue(text.text);
        }
    }
}
