using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.ItemManagement.Items
{
    /// <summary>
    /// Test item. Logs all events
    /// </summary>
    public class LoggingItem : Item
    {
        public override string Name => nameof(LoggingItem);
        public override Sprite Icon => null;
        public override string Description => "logging_item_description";

        public override void OnBattleStart(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBattleStart)}, battle {battle}");
        }

        public override void OnBattleEnd(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBattleEnd)}, battle {battle}");
        }

        public override void OnPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnPlayersTurnBegin)}, battle {battle}");
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

        public override void OnBeforeCharacterDead(Character character, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnBeforeCharacterDead)}, character {character}");
        }

        public override void OnAfterCharacterDead(Character character, CancellationToken token)
        {
            Debug.Log($"In event {nameof(OnAfterCharacterDead)}, character {character}");
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
