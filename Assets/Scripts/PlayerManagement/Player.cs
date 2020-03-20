using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Serializable]
    public class Player : Character
    {
        [NonSerialized]
        private PlayerState playerState = PlayerState.FreeControl;
        public PlayerState PlayerState
        {
            get => playerState;
            set
            {
                if (value == PlayerState.InBattle)
                    Healthbar.Show();
                if (value == PlayerState.FreeControl)
                    Healthbar.Hide();
                playerState = value;
            }
        }

        public int Level = 1;
        public int SkillPoints = 0;
        public int Experience = 0;
        public int ExperienceForNextLevel = 100;
        private float experienceGrowthFactor = 1.5F;

        public void GainExperience(int exp)
        {
            if (Experience + exp >= ExperienceForNextLevel)
            {
                int overflow = Experience + exp - ExperienceForNextLevel;
                Experience = 0;
                LevelUp();
                ExperienceForNextLevel = Mathf.RoundToInt(ExperienceForNextLevel*experienceGrowthFactor/10)*10;
                GainExperience(overflow);
            }
            else
            {
                Debug.Log($"Player gains {exp} experience");
                Experience += exp;
            }
        }

        protected void LevelUp()
        {
            Level++;
            SkillPoints++;
            Debug.Log($"Player levels up to {Level}");
        }

        protected override void Die(Character killer)
        {
            base.Die(killer);
            // Your dead(
            Debug.Log("Player dead");
            // For testing - reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
