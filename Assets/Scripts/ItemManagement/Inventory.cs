using System;
using System.Collections.Generic;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.ItemManagement.Items;
using Assets.Scripts.PlayerManagement;

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

        public void Initialize()
        {
            Items = new List<Item>();
            // TODO: temporary solution. From testing.
            Add(new LoggingItem());

            EventManager.Instance.Subscribe(this);
        }

        /// <summary>
        /// Adds new item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(Item item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Adds new consumable to the list
        /// </summary>
        /// <param name="consumable"></param>
        public void Add(Consumable consumable)
        {
            Consumables.Add(consumable);
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
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnPlayersTurnBegin(battle, token);
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
        /// <param name="token"></param>
        public void OnBeforeCharacterDead(Character character, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnBeforeCharacterDead(character, token);
            }
        }

        /// <summary>
        /// Called when any character takes lethal damage, after he dies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="token"></param>
        public void OnAfterCharacterDead(Character character, CancellationToken token)
        {
            foreach (Item item in Items)
            {
                item.OnAfterCharacterDead(character, token);
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
