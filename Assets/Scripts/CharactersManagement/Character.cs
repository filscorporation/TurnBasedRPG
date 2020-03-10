using System;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Player or enemy in the game. Controlled by CharacterController
    /// </summary>
    public abstract class Character : MapObject
    {
        public float MovingSpeed = 0.2F;

        public float HealthMax = 10F;
        public float Health = 10F;

        /// <summary>
        /// Makes character take damage
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Damage damage)
        {
            Debug.Log($"Character {this} took {damage.Value} damage from {damage.Source}");
            Health = Mathf.Max(0, Health - damage.Value);

            if (Health < Mathf.Epsilon)
            {
                Die(damage.Source);
            }
        }

        protected abstract void Die(Character killer);

        public override string ToString()
        {
            return gameObject.name;
        }
    }
}
