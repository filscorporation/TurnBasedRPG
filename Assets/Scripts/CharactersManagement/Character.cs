﻿using System;
using System.Collections.Generic;
using Assets.Scripts.EventManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.SkillManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    public enum CharacterState
    {
        Idle,
        Moving,
        Attacking,
        Consuming,
        Casting,
        Throwing,
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
                        case CharacterState.Consuming:
                            animator.SetTrigger(animatorConsumeTrigger);
                            break;
                        case CharacterState.Casting:
                            animator.SetTrigger(animatorCastTrigger);
                            break;
                        case CharacterState.Throwing:
                            animator.SetTrigger(animatorThrowTrigger);
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

        protected bool IsLoaded = false;

        private Animator animator;
        private const string animatorMovingBool = "Moving";
        private const string animatorAttackTrigger = "Attack";
        private const string animatorCastTrigger = "Cast";
        private const string animatorThrowTrigger = "Throw";
        private const string animatorConsumeTrigger = "Consume";
        private const string animatorReceiveDamageTrigger = "ReceiveDamage";
        private const string animatorDeadBool = "Dead";

        public float MovingSpeed = 0.2F;

        public int ActionPoints = 3;
        public int ActionPointsMax = 3;
        public int TilesPassedInCurrentTurn = 0;

        public float HealthMax = 10F;
        public float Health = 10F;
        public Healthbar Healthbar;
        public float HealthbarHeight = 0.2F;
        private const string healthbarSortingGroupName = "HealthbarGroup";

        public int Block = 0;
        public BlockUI BlockUI;

        public List<Effect> Effects = new List<Effect>();

        public List<Skill> Skills = new List<Skill>();
        // Used to initialize skills from inspector
        public List<string> SkillsNames = new List<string>();

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

            if (!IsLoaded)
            {
                LoadSkills();
                SetIsLoaded();
            }
            InitializeHealthbar();
            InitializeBlockUI();
            CharacterController = GetComponent<CharacterActionsController>();
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Sets is loaded to true (when character loaded and doesn't need initialization)
        /// </summary>
        public void SetIsLoaded()
        {
            IsLoaded = true;
        }

        private void LoadSkills()
        {
            foreach (string skillName in SkillsNames)
            {
                Skills.Add(Skill.SkillDictionary[skillName].Clone());
            }
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

        private void InitializeBlockUI()
        {
            Transform healthbarGroup = FindObjectOfType<Canvas>().transform.Find(healthbarSortingGroupName);
            GameObject buiGO = Instantiate(
                Resources.Load(CharacterActionsController.BlockUIPrefabPath),
                Vector3.zero,
                Quaternion.identity,
                healthbarGroup) as GameObject;
            if (buiGO == null)
                throw new Exception("Error initializing block UI");
            BlockUI = buiGO.GetComponent<BlockUI>();
            BlockUI.Initialize(Block);
            BlockUI.Character = this;
            BlockUI.Hide();
        }

        /// <summary>
        /// Makes character take damage
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="animate">If set to false, receive damage animation will not be player</param>
        public void TakeDamage(Damage damage, bool animate = true)
        {
            Debug.Log($"Character {this} took {damage.Value} damage from {damage.Source}");
            // Pre take damage events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnBeforeCharacterTakesDamage(this, damage, token);
            if (token.ShouldBeCancelled)
                return;

            // Logic
            if (animate)
                State = CharacterState.ReceivingDamage;
            float damageValue = damage.Value;
            if (Block > 0)
            {
                if (damageValue > Block)
                {
                    damageValue -= Block;
                    ClearBlock();
                }
                else
                {
                    Block -= Mathf.CeilToInt(damageValue);
                    BlockUI.Set(Block);
                    damageValue = 0;
                }
            }

            if (Mathf.Abs(damageValue) > Mathf.Epsilon)
            {
                Health = Mathf.Max(0, Health - damageValue);
                Healthbar.Set(Health, HealthMax);
            }

            // Post take damage events
            OnCharacterTakeDamage?.Invoke(this, new DamageEventData(damage));
            EventManager.Instance.OnAfterCharacterTakesDamage(this, damage, token);
            if (token.ShouldBeCancelled)
                return;

            if (Health < Mathf.Epsilon)
            {
                // Pre dead events
                EventManager.Instance.OnBeforeCharacterDead(this, damage.Source, token);
                if (token.ShouldBeCancelled)
                    return;

                // Logic
                Die(damage.Source);

                // Post dead events
                EventManager.Instance.OnAfterCharacterDead(this, damage.Source, token);
                if (token.ShouldBeCancelled)
                    return;
            }
        }

        /// <summary>
        /// Add block value to character
        /// </summary>
        /// <param name="block"></param>
        public void GainBlock(int block)
        {
            Block += block;
            BlockUI.Set(Block);
        }

        /// <summary>
        /// Set block value to zero
        /// </summary>
        public void ClearBlock()
        {
            Block = 0;
            BlockUI.Set(Block);
        }

        /// <summary>
        /// Adds effect to the character
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="caster"></param>
        public void AddEffect(Effect effect, Character caster)
        {
            // TODO: merge
            Effects.Add(effect);
            effect.EffectOnApply(this, caster);
        }

        /// <summary>
        /// Removes effect fro
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
        }

        /// <summary>
        /// Makes character to get effects on turn start
        /// </summary>
        public void CallTurnStartEffects()
        {
            foreach (Effect effect in Effects.ToArray())
            {
                if (State == CharacterState.Dead)
                    return;
                effect.EffectOnCharacterTurnStart(this);
            }
        }

        /// <summary>
        /// Removes all applied effects from a character
        /// </summary>
        public void ClearEffects()
        {
            Effects.Clear();
        }

        protected virtual void Die(Character killer)
        {
            CharacterController.Cancel();
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
