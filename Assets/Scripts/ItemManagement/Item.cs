using System;
using System.Collections.Generic;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.ItemManagement
{
    /// <summary>
    /// Represents object that can be held by inventory and modify game events
    /// </summary>
    [Serializable]
    public abstract class Item : IInventoryObject
    {
        public static Dictionary<string, Item> ItemDictionary = new Dictionary<string, Item>();

        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract string Description { get; }
        public abstract int Level { get; }

        /// <summary>
        /// Called when battle starts
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public abstract void OnBattleStart(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when battle finishes
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public abstract void OnBattleEnd(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public abstract void OnPlayersTurnBegin(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when player is getting reward for winning a battle
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="reward"></param>
        /// <param name="token"></param>
        public abstract void OnGetReward(Battle battle, Reward reward, CancellationToken token);

        /// <summary>
        /// Called when players turn ends
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public abstract void OnPlayersTurnEnd(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when any character takes damage, before it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        public abstract void OnBeforeCharacterTakesDamage(Character character, Damage damage, CancellationToken token);

        /// <summary>
        /// Called when any character takes damage, after it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        public abstract void OnAfterCharacterTakesDamage(Character character, Damage damage, CancellationToken token);

        /// <summary>
        /// Called when any character takes lethal damage, before he calls dead function
        /// </summary>
        /// <param name="character"></param>
        /// <param name="token"></param>
        public abstract void OnBeforeCharacterDead(Character character, CancellationToken token);

        /// <summary>
        /// Called when any character takes lethal damage, after he dies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="token"></param>
        public abstract void OnAfterCharacterDead(Character character, CancellationToken token);

        /// <summary>
        /// Called when player activates skill and it is about to show possible targets
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="token"></param>
        public abstract void OnSkillActivation(Player player, Skill skill, CancellationToken token);

        /// <summary>
        /// Called when player is casting skill and it effect applies
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="target"></param>
        /// <param name="token"></param>
        public abstract void OnSkillUse(Player player, Skill skill, SkillTarget target, CancellationToken token);
    }
}
