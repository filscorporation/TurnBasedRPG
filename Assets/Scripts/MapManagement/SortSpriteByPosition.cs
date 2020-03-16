using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Changes sprite sorting order according to position
    /// </summary>
    public class SortSpriteByPosition : MonoBehaviour
    {
        private List<SpriteRenderer> spriteRenderers;

        public void Start()
        {
            spriteRenderers = new List<SpriteRenderer>();
            SpriteRenderer self = GetComponent<SpriteRenderer>();
            if (self != null)
                spriteRenderers.Add(self);
            spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
        }

        public void LateUpdate()
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                int innerOrder = spriteRenderer.sortingOrder % 10;
                spriteRenderer.sortingOrder = - Mathf.RoundToInt(transform.position.y)*100 + innerOrder;
            }
        }
    }
}
