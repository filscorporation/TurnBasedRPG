using System;
using System.Collections.Generic;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.ItemManagement.Consumables;
using Assets.Scripts.ItemManagement.Items;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.ItemManagement
{
    /// <summary>
    /// Players inventory - list of items
    /// </summary>
    [Serializable]
    public class Inventory : IEventSubscriber
    {
        /// <summary>
        /// Items player is carrying. Items are permanent and have some effect that triggers on event
        /// </summary>
        public List<Item> Items;

        /// <summary>
        /// Consumables (potions) player is carrying. Can be used in battle to gain some effect
        /// </summary>
        public List<Consumable> Consumables;

        /// <summary>
        /// Ingame currency. Used in the market to buy items
        /// </summary>
        public int Gold = 0;

        /// <summary>
        /// Called when anything in inventory got changed
        /// </summary>
        public event EventHandler OnInventoryUpdated;

        public void Initialize()
        {
            Items = new List<Item>();
            // TODO: temporary solution. For testing.
            Add(Item.ItemDictionary[nameof(Spurs)]);

            Consumables = new List<Consumable>();
            // TODO: temporary solution. For testing.
            Add(Consumable.ConsumablesDictionary[nameof(FlamePotion)]);
            Add(Consumable.ConsumablesDictionary[nameof(SpeedPotion)]);
            Add(Consumable.ConsumablesDictionary[nameof(ArmourPotion)]);
        }

        /// <summary>
        /// Subscribes to game events for items to work
        /// </summary>
        public void SubscribeToEvents()
        {
            EventManager.Instance.Subscribe(this);
        }
        
        /// <summary>
        /// Adds new item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(Item item)
        {
            Items.Add(item);
            OnInventoryUpdated?.Invoke(this, new InventoryEventData(item, InventoryAction.Added));
        }

        /// <summary>
        /// Adds new consumable to the list
        /// </summary>
        /// <param name="consumable"></param>
        public void Add(Consumable consumable)
        {
            Consumables.Add(consumable);
            OnInventoryUpdated?.Invoke(this, new InventoryEventData(consumable, InventoryAction.Added));
        }

        /// <summary>
        /// Adds new item or consumable to the list
        /// </summary>
        /// <param name="iobj"></param>
        public void Add(IInventoryObject iobj)
        {
            switch (iobj)
            {
                case Item item:
                    Add(item);
                    break;
                case Consumable consumable:
                    Add(consumable);
                    break;
                default:
                    throw new NotSupportedException(iobj.ToString());
            }
        }

        public void Consume(Consumable consumable)
        {
            Debug.Log($"Consuming {consumable}");
            // TODO: quantity --
            Consumables.Remove(consumable);
            OnInventoryUpdated?.Invoke(this, new InventoryEventData(consumable, InventoryAction.Removed));
        }

        #region Events

        /// <summary>
        /// Called when battle starts
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnBattleStart(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBattleStart(battle, token);
            }
        }

        /// <summary>
        /// Called when battle finishes
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnBattleEnd(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBattleEnd(battle, token);
            }
        }

        /// <summary>
        /// Called when player is getting reward for winning a battle
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="reward"></param>
        /// <param name="token"></param>
        public void OnGetReward(Battle battle, Reward reward, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnGetReward(battle, reward, token);
            }
        }

        /// <summary>
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnBeforePlayersTurnBegin(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBeforePlayersTurnBegin(battle, token);
            }
        }

        /// <summary>
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnAfterPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnAfterPlayersTurnBegin(battle, token);
            }
        }

        /// <summary>
        /// Called when players turn ends
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnPlayersTurnEnd(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnPlayersTurnEnd(battle, token);
            }
        }

        /// <summary>
        /// Called when any character takes damage, before it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        public void OnBeforeCharacterTakesDamage(Character character, Damage damage, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBeforeCharacterTakesDamage(character, damage, token);
            }
        }

        /// <summary>
        /// Called when any character takes damage, after it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        public void OnAfterCharacterTakesDamage(Character character, Damage damage, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnAfterCharacterTakesDamage(character, damage, token);
            }
        }

        /// <summary>
        /// Called when any character takes lethal damage, before he calls dead function
        /// </summary>
        /// <param name="character"></param>
        /// <param name="killer"></param>
        /// <param name="token"></param>
        public void OnBeforeCharacterDead(Character character, Character killer, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBeforeCharacterDead(character, killer, token);
            }
        }

        /// <summary>
        /// Called when any character takes lethal damage, after he dies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="killer"></param>
        /// <param name="token"></param>
        public void OnAfterCharacterDead(Character character, Character killer, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnAfterCharacterDead(character, killer, token);
            }
        }

        /// <summary>
        /// Called when player activates skill and it is about to show possible targets
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="token"></param>
        public void OnSkillActivation(Player player, Skill skill, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnSkillActivation(player, skill, token);
            }
        }

        /// <summary>
        /// Called when player is casting skill and it effect applies
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="target"></param>
        /// <param name="token"></param>
        public void OnSkillUse(Player player, Skill skill, SkillTarget target, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnSkillUse(player, skill, target, token);
            }
        }

        #endregion
    }
}
