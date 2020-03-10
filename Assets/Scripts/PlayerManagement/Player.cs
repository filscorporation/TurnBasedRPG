using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    public enum PlayerState
    {
        FreeControl,
        Waiting,
        InBattle,
    }

    /// <summary>
    /// Represents player in the game with his stats and interactions
    /// </summary>
    public class Player : Character
    {
        private PlayerState state = PlayerState.FreeControl;
        public PlayerState State
        {
            get => state;
            set
            {
                if (value == PlayerState.InBattle)
                    Healthbar.Show();
                if (value == PlayerState.FreeControl)
                    Healthbar.Hide();
                state = value;
            }
        }

        public int ActionPoints = 3;
        public int ActionPointsMax = 3;

        public List<Skill> Skills = new List<Skill>
        {
            new Skill
            {
                Name = "Attack",
                Range = 1,
                Cost = 2,
                TargetType = SkillTargetType.Enemy,
                Damage = 5F,
            },
        };

        protected override void Die(Character killer)
        {
            base.Die(killer);
            // Your dead(
            Debug.Log("Player dead");
        }
    }
}
