using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Changes sprite sorting order according to position
    /// </summary>
    public class SortSpriteByPosition : MonoBehaviour
    {
        public bool Static = false;

        private List<(SpriteRenderer, int)> spriteRenderers;

        public void Start()
        {
            spriteRenderers = new List<(SpriteRenderer, int)>();
            SpriteRenderer self = GetComponent<SpriteRenderer>();
            if (self != null)
                spriteRenderers.Add((self, self.sortingOrder));
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderers.Add((spriteRenderer, spriteRenderer.sortingOrder));
            }

            Sort();
        }

        public void LateUpdate()
        {
            if (!Static)
                Sort();
        }

        private void Sort()
        {
            foreach ((SpriteRenderer, int) spriteRenderer in spriteRenderers)
            {
                spriteRenderer.Item1.sortingOrder = 1000000 - Mathf.RoundToInt(transform.position.y) * 100 + spriteRenderer.Item2;
            }
        }
    }
}
