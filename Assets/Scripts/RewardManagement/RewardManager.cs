using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.InputManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.RewardManagement
{
    /// <summary>
    /// Calculates reward for the player based on battle result
    /// </summary>
    public class RewardManager : MonoBehaviour
    {
        /// <summary>
        /// The disspersion of distribution for loot
        /// </summary>
        private const float randomRange = 1F;

        private RewardTab rewardTab;

        protected Player Player;
        protected Reward Reward;

        public void Start()
        {
            rewardTab = Resources.FindObjectsOfTypeAll<RewardTab>().FirstOrDefault();
            if (rewardTab == null)
                throw new Exception("RewardTab is required");
        }

        /// <summary>
        /// Calculates reward for the player and open menu to choose
        /// </summary>
        /// <param name="battle"></param>
        public void GivePlayerReward(Battle battle)
        {
            float rewardSum = 0;
            Player = battle.Player;
            // Sum of enemy levels
            foreach (Enemy enemy in battle.Enemies)
            {
                rewardSum += enemy.Level;
            }

            // Bonus for enemy count
            rewardSum *= 1 + (battle.Enemies.Count - 1F) / 10F;
            // Reward per enemy level
            float rewardPerLevel = 10F;
            rewardSum *= rewardPerLevel;
            int rewardGold = Mathf.RoundToInt(rewardSum);
            int rewardExp = Mathf.RoundToInt(rewardSum);
            IInventoryObject item = CalculateItem(battle);

            Reward = new Reward(rewardGold, rewardExp, item);
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnGetReward(battle, Reward, token);
            if (token.ShouldBeCancelled)
                return;

            // Show choose reward UI for the player and wait for response
            Debug.Log($"Player is getting reward {Reward}");
            rewardTab.gameObject.SetActive(true);
            rewardTab.Initialize(this, Reward);
            AutoInputInitializer.InputManager.IsNeedToCheckForInput = false;
        }

        /// <summary>
        /// Give player reward he chose
        /// </summary>
        /// <param name="index"></param>
        public void HandleRewardChoosen(int index)
        {
            switch (index)
            {
                // Gold
                case 0:
                    Player.Inventory.Gold += Reward.Gold;
                    break;
                // Experience
                case 1:
                    Player.Experience += Reward.Experience;
                    break;
                // Item
                case 2:
                    Player.Inventory.Add(Reward.Item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }

            Player = null;
            Reward = null;
            rewardTab.Clear();
            rewardTab.gameObject.SetActive(false);
            AutoInputInitializer.InputManager.IsNeedToCheckForInput = true;
        }

        private IInventoryObject CalculateItem(Battle battle)
        {
            int maxRandomLevel = 1;
            // Roll item level for each enemy defeated and take best result
            foreach (Enemy enemy in battle.Enemies)
            {
                int level = RandomGenerator.Instance.RandomNormal(enemy.Level, randomRange);
                if (level > maxRandomLevel)
                    maxRandomLevel = level;
            }

            if (maxRandomLevel > 1)
            {
                // TODO: optimization for items matching
                List<Item> uniqueItems = Item.ItemDictionary.Values
                    .Where(i => battle.Player.Inventory.Items.All(pi => pi.Name != i.Name)).ToList();
                // Remove all items that for sure are of too high level
                uniqueItems.RemoveAll(i => i.Level > maxRandomLevel);

                if (uniqueItems.Any())
                {
                    // Get a random item of level we rolled and unique for the player
                    while (maxRandomLevel > 1)
                    {
                        List<Item> levelMatchingItems = uniqueItems.Where(i => i.Level == maxRandomLevel).ToList();
                        if (levelMatchingItems.Any())
                        {
                            // Found couple of items of level we needed, roll random one from them
                            int iIndex = RandomGenerator.Instance.RandomInt(levelMatchingItems.Count);
                            return levelMatchingItems[iIndex];
                        }

                        // No items of level we needed, roll closest one
                        maxRandomLevel--;
                    }
                }
            }

            // Didn't roll anything better than level 1 - roll consumable
            // TODO: consumables levels should be taken in consideration too
            int cIndex = RandomGenerator.Instance.RandomInt(Consumable.ConsumablesDictionary.Count);
            return Consumable.ConsumablesDictionary.ElementAt(cIndex).Value;
        }
    }
}
