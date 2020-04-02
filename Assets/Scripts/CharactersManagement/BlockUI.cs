using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Shows block value above character
    /// </summary>
    public class BlockUI : MonoBehaviour
    {
        public GameObject BlockIcon;
        public Text BlockText;
        public Character Character;

        private int currentBlockValue;
        private const float blockUIXOffset = 140F;
        private const float blockUIHeight = 0.2F;

        public void Initialize(int block)
        {
            currentBlockValue = block;
        }

        public void LateUpdate()
        {
            transform.position = Camera.main.WorldToScreenPoint(Character.transform.position);
            transform.position += new Vector3(blockUIXOffset, Screen.height * blockUIHeight);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Set(int block)
        {
            BlockText.text = block.ToString();

            if (block == 0)
            {
                Hide();
            }
            else if (currentBlockValue == 0)
            {
                Show();
            }

            currentBlockValue = block;
        }
    }
}
