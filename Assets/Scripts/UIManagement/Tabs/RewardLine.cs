using Assets.Scripts.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Reward line in UI inventory tab
    /// </summary>
    public class RewardLine : MonoBehaviour
    {
        public float Height = 140F;
        public int RewardIndex;
        public Image Icon;
        public Text Description;

        public RewardTab tab;

        public void Initialize(int value, string text, int index, RewardTab rewardTab)
        {
            Description.text = $"{value} {LocalizationManager.GetLocalizedValue(text)}";
            tab = rewardTab;
            RewardIndex = index;
        }

        public void Initialize(IInventoryObject item, int index, RewardTab rewardTab)
        {
            Description.text = LocalizationManager.GetLocalizedValue(item.Description);
            Icon.sprite = item.Icon;
            tab = rewardTab;
            RewardIndex = index;
        }

        public void HandleButtonClicked()
        {
            tab.HandleRewardClick(RewardIndex);
        }
    }
}
