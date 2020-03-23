using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Represents list of items and consumables in UI
    /// </summary>
    public class InventoryTab : MonoBehaviour
    {
        public Transform ScrollViewContent;
        public GameObject InventoryLinePrefab;

        private List<InventoryLine> lines;

        public void Initialize(IEnumerable<IInventoryObject> items)
        {
            lines = new List<InventoryLine>();
            int i = 0;
            float contentSize = 0;
            foreach (IInventoryObject item in items)
            {
                GameObject go = Instantiate(InventoryLinePrefab);
                go.transform.SetParent(ScrollViewContent);
                InventoryLine line = go.GetComponent<InventoryLine>();
                go.GetComponent<RectTransform>().localPosition = new Vector2(0, -i * line.Height);
                go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
                line.Initialize(item);
                lines.Add(line);
                contentSize += line.Height;
                i++;
            }

            SetContentSize(contentSize);
        }

        private void SetContentSize(float size)
        {
            RectTransform rt = ScrollViewContent.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, size);
        }

        public void Clear()
        {
            foreach (InventoryLine inventoryLine in lines)
            {
                Destroy(inventoryLine.gameObject);
            }
            lines.Clear();
            SetContentSize(0);
        }
    }
}
