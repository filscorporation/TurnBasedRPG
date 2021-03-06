﻿using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;
using UnityEngine;

namespace Assets.Scripts.ItemManagement.Items
{
    /// <summary>
    /// Test item. Logs all events
    /// </summary>
    public class LoggingItem : Item
    {
        public override string Name => nameof(LoggingItem);
        protected override string iconPath => null;
        public override string Description => "logging_item_description";
        public override int Level => 2;

        public override void OnBattleStart(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBattleStart)}, battle {battle}");
        }

        public override void OnBattleEnd(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBattleEnd)}, battle {battle}");
        }

        public override void OnBeforePlayersTurnBegin(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBeforePlayersTurnBegin)}, battle {battle}");
        }

        public override void OnAfterPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnAfterPlayersTurnBegin)}, battle {battle}");
        }

        public override void OnGetReward(Battle battle, Reward reward, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnGetReward)}, battle {battle}, reward {reward}");
        }

        public override void OnPlayersTurnEnd(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnPlayersTurnEnd)}, battle {battle}");
        }

        public override void OnBeforeCharacterTakesDamage(Character character, Damage damage, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBeforeCharacterTakesDamage)}, character {character}, damage {damage}");
        }

        public override void OnAfterCharacterTakesDamage(Character character, Damage damage, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnAfterCharacterTakesDamage)}, character {character}, damage {damage}");
        }

        public override void OnBeforeCharacterDead(Character character, Character killer, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBeforeCharacterDead)}, character {character}, killer {killer}");
        }

        public override void OnAfterCharacterDead(Character character, Character killer, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnAfterCharacterDead)}, character {character}, killer {killer}");
        }

        public override void OnSkillActivation(Player player, Skill skill, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnSkillActivation)}, character {player}, skill {skill}");
        }

        public override void OnSkillUse(Player player, Skill skill, SkillTarget target, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnSkillUse)}, character {player}, target {target}, skill {skill}");
        }
    }
}
