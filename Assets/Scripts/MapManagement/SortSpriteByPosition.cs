using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Changes sprite sorting order according to position
    /// </summary>
    public class SortSpriteByPosition : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Update()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 50 - Mathf.RoundToInt(transform.position.y);
            }
        }
    }
}
