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
        public Healthbar Healthbar;

        public new void Start()
        {
            base.Start();

            InitializeHealthbar();
        }

        private void InitializeHealthbar()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            GameObject hbGO = Instantiate(
                Resources.Load(CharacterActionsController.HealthbarPrefabPath),
                Vector3.zero,
                Quaternion.identity,
                canvas.transform) as GameObject;
            if (hbGO == null)
                throw new Exception("Error initializing healthbar");
            Healthbar = hbGO.GetComponent<Healthbar>();
            Healthbar.Initialize();
            Healthbar.Character = this;
            Healthbar.Set(Health, HealthMax);
            Healthbar.Hide();
        }

        /// <summary>
        /// Makes character take damage
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Damage damage)
        {
            Debug.Log($"Character {this} took {damage.Value} damage from {damage.Source}");
            Health = Mathf.Max(0, Health - damage.Value);
            Healthbar.Set(Health, HealthMax);

            if (Health < Mathf.Epsilon)
            {
                Die(damage.Source);
            }
        }

        protected virtual void Die(Character killer)
        {
            OnTile.Free = true;
            OnTile.Occupier = null;
            Destroy(Healthbar.gameObject);
        }

        public override string ToString()
        {
            return gameObject.name;
        }
    }
}
