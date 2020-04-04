using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;

namespace Assets.Scripts.EventManagement
{
    /// <summary>
    /// Intrerface for types, that can process events from EventManager
    /// </summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Called when battle starts
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        void OnBattleStart(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when battle finishes
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        void OnBattleEnd(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when player is getting reward for winning a battle
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="reward"></param>
        /// <param name="token"></param>
        void OnGetReward(Battle battle, Reward reward, CancellationToken token);

        /// <summary>
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        void OnPlayersTurnBegin(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when players turn ends
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        void OnPlayersTurnEnd(Battle battle, CancellationToken token);

        /// <summary>
        /// Called when any character takes damage, before it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        void OnBeforeCharacterTakesDamage(Character character, Damage damage, CancellationToken token);

        /// <summary>
        /// Called when any character takes damage, after it applies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="damage"></param>
        /// <param name="token"></param>
        void OnAfterCharacterTakesDamage(Character character, Damage damage, CancellationToken token);

        /// <summary>
        /// Called when any character takes lethal damage, before he calls dead function
        /// </summary>
        /// <param name="character"></param>
        /// <param name="killer"></param>
        /// <param name="token"></param>
        void OnBeforeCharacterDead(Character character, Character killer, CancellationToken token);

        /// <summary>
        /// Called when any character takes lethal damage, after he dies
        /// </summary>
        /// <param name="character"></param>
        /// <param name="killer"></param>
        /// <param name="token"></param>
        void OnAfterCharacterDead(Character character, Character killer, CancellationToken token);

        /// <summary>
        /// Called when player activates skill and it is about to show possible targets
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="token"></param>
        void OnSkillActivation(Player player, Skill skill, CancellationToken token);

        /// <summary>
        /// Called when player is casting skill and it effect applies
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skill"></param>
        /// <param name="target"></param>
        /// <param name="token"></param>
        void OnSkillUse(Player player, Skill skill, SkillTarget target, CancellationToken token);
    }
}
