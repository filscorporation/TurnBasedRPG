using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
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

        private List<Image> images;
        
        private int ap = -1;
        private int maxap;

        private const float elementOffset = 90F;

        public void SetValue(int value)
        {
            if (value == -1)
            {
                ap = -1;
                foreach (Image image in images)
                {
                    Destroy(image);
                }
                images.Clear();
                return;
            }

            if (ap == -1)
            {
                maxap = value;
                ap = maxap;

                images = new List<Image>();
                for (int i = 0; i < maxap; i++)
                {
                    Vector2 p = transform.position;
                    p += new Vector2(elementOffset*i, 0);
                    GameObject newAP = Instantiate(ActionPointUIPrefab, p, Quaternion.identity, transform);
                    images.Add(newAP.GetComponent<Image>());
                }
                return;
            }

            foreach (Image image in images.GetRange(0, value))
            {
                image.sprite = FullAP;
            }
            foreach (Image image in images.GetRange(value, maxap - value))
            {
                image.sprite = EmptyAP;
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
