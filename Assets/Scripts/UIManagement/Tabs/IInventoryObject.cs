using UnityEngine;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Interface for type that can be stored and rendered in inventory
    /// </summary>
    public interface IInventoryObject
    {
        Sprite Icon { get; }

        string Description { get; }
    }
}
