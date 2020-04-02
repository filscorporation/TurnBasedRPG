using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Healthbar controller
    /// </summary>
    public class Healthbar : MonoBehaviour
    {
        public Gradient Gradient;
        public Character Character;

        private Slider slider;
        private Image fill;
        private const string fillImageName = "Fill";
        private const float healthbarHeight = 0.2F;

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
            transform.position += new Vector3(0, Screen.height * healthbarHeight);
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
