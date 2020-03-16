using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.EnemyManagement
{
    /// <summary>
    /// Represents enemy in the game with his stats and interactions
    /// </summary>
    public class Enemy : Character
    {

        public int LineOfSight = 3;

        private Action callWhenFinishedActing;

        protected Skill CurrentSkill;
        protected Battle CurrentBattle;

        /// <summary>
        /// Checks if other character in sight of this
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool InSight(Character c)
        {
            return Mathf.Max(
                       Mathf.Abs(c.OnTile.X - OnTile.X),
                       Mathf.Abs(c.OnTile.Y - OnTile.Y))
                   <= LineOfSight;
        }

        protected override void Die(Character killer)
        {
            base.Die(killer);

            Destroy(this);
            Destroy(gameObject, 2F);
        }

        /// <summary>
        /// Estimates enemy priority in battle: lower - earlier he gonna act
        /// </summary>
        /// <param name="battle"></param>
        /// <returns></returns>
        public virtual int Priority(Battle battle)
        {
            // Straight up distance to a player
            return Mathf.Max(
                    Mathf.Abs(battle.Player.OnTile.X - OnTile.X),
                    Mathf.Abs(battle.Player.OnTile.Y - OnTile.Y));
        }

        /// <summary>
        /// Makes enemy do his turn - using skills, moving
        /// </summary>
        /// <param name="battle"></param>
        /// <param name="onFinishedActing">Action to call when enemy finished it turn</param>
        public virtual void Act(Battle battle, Action onFinishedActing)
        {
            Debug.Log($"Enemy {(Enemy)this} acts");

            CurrentBattle = battle;
            callWhenFinishedActing = onFinishedActing;
            
            ContinueActing();
        }

        private void ContinueActing()
        {
            if (ActionPoints == 0)
            {
                Debug.Log($"Out of action points");
                callWhenFinishedActing.Invoke();
                return;
            }

            // Order skills by effectivness and try to use them
            List<Skill> sortedSkills = SortSkills(CurrentBattle).ToList();
            foreach (Skill skill in sortedSkills)
            {
                if (skill.InRange(this, CurrentBattle.Player))
                {
                    if (skill.Cost > ActionPoints)
                    {
                        Debug.Log($"Waiting for action points to restore");
                        // Target is in range but we dont have enough AP, we can wait
                        callWhenFinishedActing.Invoke();
                        return;
                    }

                    Debug.Log($"Using skill {skill.Name}");
                    ActionPoints -= skill.Cost;
                    CurrentSkill = skill;
                    CharacterController.UseSkill(skill, new SkillTarget(CurrentBattle.Player), ContinueActing, UseSkillEffect);
                    return;
                }
            }

            // Cant cast any skill, try to move

            // Builging path straight to the player, maybe should act smarter
            Debug.Log($"Moving to target");
            List<Tile> path = MapManager.Instance.GetPath(OnTile, CurrentBattle.Player.OnTile);
            if (path == null || !path.Any())
            {
                // Cant get to the player
                Debug.LogWarning("Cant reach player");
                callWhenFinishedActing.Invoke();
                return;
            }

            // Move to target and try to cast any skill on the path
            CharacterController.Move(path, ReachedNextTile, ContinueActing);
        }

        private void ReachedNextTile(int tileIndex)
        {
            // If not fist tile of the path - take action point
            if (tileIndex != 0)
            {
                ActionPoints--;
            }

            if (ActionPoints == 0)
            {
                CharacterController.Cancel();
                Debug.LogWarning("Used all action points");
                callWhenFinishedActing.Invoke();
                return;
            }

            // TODO: Here need to try to use skills only, not move
        }

        private void UseSkillEffect()
        {
            Debug.Log($"Using skill effect");
            CurrentSkill.Use(this, new SkillTarget(CurrentBattle.Player));
        }

        /// <summary>
        /// Returns best skill to use in context of battle situation
        /// </summary>
        /// <param name="battle"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Skill> SortSkills(Battle battle)
        {
            // Get the most expensive skill
            // TODO: better logic - if we can kill player right now - do that and etc

            return Skills.OrderByDescending(s => s.Cost);
        }
    }
}
