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

        private float elementOffset = 65F;

        public void Start()
        {
            elementOffset = elementOffset * Screen.width / 1600;
        }

        public void SetValue(int value)
        {
            if (value == -1)
            {
                ap = -1;
                Clear();

                return;
            }

            if (ap == -1)
            {
                maxap = value;
                ap = maxap;
                Build();
                
                return;
            }

            if (value > maxap)
            {
                maxap = value;
                Clear();
                Build();
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

        private void Clear()
        {
            foreach (Image image in images)
            {
                Destroy(image.gameObject);
            }
            images.Clear();
        }

        private void Build()
        {
            images = new List<Image>();
            for (int i = 0; i < maxap; i++)
            {
                Vector2 p = transform.position;
                p += new Vector2(elementOffset * i, 0);
                GameObject newAP = Instantiate(ActionPointUIPrefab, p, Quaternion.identity, transform);
                images.Add(newAP.GetComponent<Image>());
            }
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}
