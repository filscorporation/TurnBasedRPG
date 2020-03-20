using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Line in the tab with some info and data source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TabLine<T> : MonoBehaviour where T : IInventoryObject
    {
        public float Height = 140F;
        public Image Icon;
        public Text Description;

        public T Source;

        public virtual void Initialize(T source)
        {
            Source = source;
            Icon.sprite = source.Icon;
            Description.text = source.Description;
        }
    }
}
