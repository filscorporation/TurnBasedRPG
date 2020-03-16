﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    public enum CharacterState
    {
        Idle,
        Moving,
        Attacking,
        ReceivingDamage,
        Dead,
    }
    /// <summary>
    /// Player or enemy in the game. Controlled by CharacterController
    /// </summary>
    public abstract class Character : MapObject
    {
        public CharacterActionsController CharacterController;

        private CharacterState state = CharacterState.Idle;

        /// <summary>
        /// Animation states
        /// </summary>
        public CharacterState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    switch(value)
                    {
                        case CharacterState.Idle:
                            if (state == CharacterState.Moving)
                                animator.SetBool(animatorMovingBool, false);
                            state = value;
                            break;
                        case CharacterState.Moving:
                            animator.SetBool(animatorMovingBool, true);
                            state = value;
                            break;
                        case CharacterState.Attacking:
                            animator.SetTrigger(animatorAttackTrigger);
                            break;
                        case CharacterState.ReceivingDamage:
                            animator.SetTrigger(animatorReceiveDamageTrigger);
                            break;
                        case CharacterState.Dead:
                            animator.SetBool(animatorDeadBool, true);
                            state = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException(value.ToString());
                    }
                }
            }
        }
        private Animator animator;
        private const string animatorMovingBool = "Moving";
        private const string animatorAttackTrigger = "Attack";
        private const string animatorReceiveDamageTrigger = "ReceiveDamage";
        private const string animatorDeadBool = "Dead";

        public float MovingSpeed = 0.2F;

        public int ActionPoints = 3;
        public int ActionPointsMax = 3;

        public float HealthMax = 10F;
        public float Health = 10F;
        public Healthbar Healthbar;
        private const string healthbarSortingGroupName = "HealthbarGroup";

        public List<Skill> Skills = new List<Skill>();

        /// <summary>
        /// Called when character takes damage
        /// </summary>
        public event EventHandler OnCharacterTakeDamage;

        /// <summary>
        /// Called when character dies
        /// </summary>
        public event EventHandler OnCharacterDied;

        public new void Start()
        {
            base.Start();

            InitializeHealthbar();
            CharacterController = GetComponent<CharacterActionsController>();
            animator = GetComponent<Animator>();
        }

        private void InitializeHealthbar()
        {
            Transform healthbarGroup = FindObjectOfType<Canvas>().transform.Find(healthbarSortingGroupName);
            GameObject hbGO = Instantiate(
                Resources.Load(CharacterActionsController.HealthbarPrefabPath),
                Vector3.zero,
                Quaternion.identity,
                healthbarGroup) as GameObject;
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
            State = CharacterState.ReceivingDamage;
            Health = Mathf.Max(0, Health - damage.Value);
            Healthbar.Set(Health, HealthMax);
            OnCharacterTakeDamage?.Invoke(this, new DamageEventData(damage));

            if (Health < Mathf.Epsilon)
            {
                Die(damage.Source);
            }
        }

        protected virtual void Die(Character killer)
        {
            State = CharacterState.Dead;
            OnTile.Free = true;
            OnTile.Occupier = null;
            Destroy(Healthbar.gameObject);
            OnCharacterDied?.Invoke(this, null);
        }

        public override string ToString()
        {
            return gameObject.name;
        }
    }
}
