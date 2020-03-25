using Assets.Scripts.RewardManagement;
using UnityEngine;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Shows player reward to choose from
    /// </summary>
    public class RewardTab : MonoBehaviour
    {
        private RewardManager manager;

        public GameObject RewardLinePrefab;
        public GameObject ItemRewardLinePrefab;
        public Transform LinesParent;

        private const string ofGoldDescription = "n_of_gold";
        private const string ofExperienceDescription = "n_of_experience";

        private RewardLine goldLine;
        private RewardLine experienceLine;
        private RewardLine itemLine;

        public virtual void Initialize(RewardManager rewardManager, Reward reward)
        {
            manager = rewardManager;

            float contentSize = 0;
            
            GameObject go = Instantiate(RewardLinePrefab);
            go.transform.SetParent(LinesParent);
            goldLine = go.GetComponent<RewardLine>();
            go.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, contentSize);
            go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            goldLine.Initialize(reward.Gold, ofGoldDescription, 0, this);
            contentSize += goldLine.Height;

            go = Instantiate(RewardLinePrefab);
            go.transform.SetParent(LinesParent);
            experienceLine = go.GetComponent<RewardLine>();
            go.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -contentSize);
            go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            experienceLine.Initialize(reward.Experience, ofExperienceDescription, 1, this);
            contentSize += experienceLine.Height;

            go = Instantiate(ItemRewardLinePrefab);
            go.transform.SetParent(LinesParent);
            itemLine = go.GetComponent<RewardLine>();
            go.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -contentSize);
            go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            itemLine.Initialize(reward.Item, 2, this);
        }

        public void Clear()
        {
            Destroy(goldLine.gameObject);
            Destroy(experienceLine.gameObject);
            Destroy(itemLine.gameObject);
            goldLine = null;
            experienceLine = null;
            itemLine = null;
        }

        /// <summary>
        /// Redirects reward choosen to RewardManager
        /// </summary>
        /// <param name="rewardIndex"></param>
        public void HandleRewardClick(int rewardIndex)
        {
            manager.HandleRewardChoosen(rewardIndex);
        }
    }
}
