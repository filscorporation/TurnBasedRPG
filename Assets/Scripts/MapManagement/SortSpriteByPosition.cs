using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Changes sprite sorting order according to position
    /// </summary>
    public class SortSpriteByPosition : MonoBehaviour
    {
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
        }

        public void LateUpdate()
        {
            foreach ((SpriteRenderer, int) spriteRenderer in spriteRenderers)
            {
                spriteRenderer.Item1.sortingOrder = 100000 - Mathf.RoundToInt(transform.position.y)*100 + spriteRenderer.Item2;
            }
        }
    }
}
