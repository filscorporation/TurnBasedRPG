using System.Collections.Generic;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;

namespace Assets.Scripts.EventManagement
{
    /// <summary>
    /// Center for processing ingame events
    /// </summary>
    public class EventManager
    {
        private static EventManager instance;
        public static EventManager Instance => instance ?? (instance = new EventManager());

        private readonly List<IEventSubscriber> subs = new List<IEventSubscriber>();

        public void Subscribe(IEventSubscriber sub)
        {
            subs.Add(sub);
        }

        /// <summary>
        /// Called when battle starts
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnBattleStart(Battle battle, CancellationToken token)
        {
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnBattleStart(battle, token);
            }
        }

        /// <summary>
        /// Called when battle finishes
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnBattleEnd(Battle battle, CancellationToken token)
        {
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnBattleEnd(battle, token);
            }
        }

        /// <summary>
        /// Called when player gets reward for winning a battle
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="reward"></param>
        /// <param name="token"></param>
        public void OnGetReward(Battle battle, Reward reward, CancellationToken token)
        {
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnGetReward(battle, reward, token);
            }
        }

        /// <summary>
        /// Called when players turn begins
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnPlayersTurnBegin(battle, token);
            }
        }

        /// <summary>
        /// Called when players turn ends
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="token"></param>
        public void OnPlayersTurnEnd(Battle battle, CancellationToken token)
        {
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnPlayersTurnEnd(battle, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnBeforeCharacterTakesDamage(character, damage, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnAfterCharacterTakesDamage(character, damage, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnBeforeCharacterDead(character, killer, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnAfterCharacterDead(character, killer, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnSkillActivation(player, skill, token);
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
            foreach (IEventSubscriber sub in subs)
            {
                sub.OnSkillUse(player, skill, target, token);
            }
        }
    }
}
