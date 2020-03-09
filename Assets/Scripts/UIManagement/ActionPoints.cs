using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UIManagement
{
    /// <summary>
    /// UI element representing AP bar
    /// </summary>
    public class ActionPoints : MonoBehaviour, IUIElement
    {
        public string Name { get; } = "ActionPoints";

        public GameObject ActionPointUIPrefab;
        public Sprite FullAP;
        public Sprite EmptyAP;

        private List<SpriteRenderer> sprites;
        
        private int ap = -1;
        private int maxap;

        private const float elementOffset = 60F;

        public void SetValue(int value)
        {
            if (value == -1)
            {
                ap = -1;
                foreach (SpriteRenderer sprite in sprites)
                {
                    Destroy(sprite);
                }
                sprites.Clear();
                return;
            }

            if (ap == -1)
            {
                maxap = value;

                sprites = new List<SpriteRenderer>();
                for (int i = 0; i < maxap; i++)
                {
                    Vector2 p = transform.position;
                    p += new Vector2(elementOffset*i, 0);
                    GameObject newAP = Instantiate(ActionPointUIPrefab, p, Quaternion.identity, transform);
                    sprites.Add(newAP.GetComponent<SpriteRenderer>());
                }
                return;
            }

            foreach (SpriteRenderer sprite in sprites.GetRange(0, value))
            {
                sprite.sprite = FullAP;
            }
            foreach (SpriteRenderer sprite in sprites.GetRange(value, maxap - value))
            {
                sprite.sprite = EmptyAP;
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
