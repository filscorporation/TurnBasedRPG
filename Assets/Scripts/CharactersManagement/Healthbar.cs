using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Heathbar controller
    /// </summary>
    public class Healthbar : MonoBehaviour
    {
        public Gradient Gradient;
        public Character Character;

        private Slider slider;
        private Image fill;
        private const string fillImageName = "Fill";
        private const float healthbarHeigth = 0.15F;

        /// <summary>
        /// Should be called before set methods
        /// </summary>
        public void Initialize()
        {
            slider = GetComponent<Slider>();
            fill = GetComponentsInChildren<CanvasRenderer>()
                .First(c => c.gameObject.name == fillImageName)
                .GetComponent<Image>();
        }

        public void LateUpdate()
        {
            transform.position = Camera.main.WorldToScreenPoint(Character.transform.position);
            transform.position += new Vector3(0, Screen.height * healthbarHeigth);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Set(float health, float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = health;

            fill.color = Gradient.Evaluate(slider.normalizedValue);
        }
    }
}
