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
    public class Player : Character
    {
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
