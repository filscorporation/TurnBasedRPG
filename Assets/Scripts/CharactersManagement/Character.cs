using System;
using System.Collections.Generic;
using Assets.Scripts.EventManagement;
using Assets.Scripts.ItemManagement;
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

            LoadSkills();
            InitializeHealthbar();
            CharacterController = GetComponent<CharacterActionsController>();
            animator = GetComponent<Animator>();
        }

        private void LoadSkills()
        {
            foreach (string skillName in SkillsNames)
            {
                Skills.Add(Skill.SkillDictionary[skillName]);
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

        /// <summary>
        /// Makes character take damage
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Damage damage)
        {
            Debug.Log($"Character {this} took {damage.Value} damage from {damage.Source}");
            // Pre take damage events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnBeforeCharacterTakesDamage(this, damage, token);
            if (token.ShouldBeCancelled)
                return;

            // Logic
            State = CharacterState.ReceivingDamage;
            Health = Mathf.Max(0, Health - damage.Value);
            Healthbar.Set(Health, HealthMax);

            // Post take damage events
            OnCharacterTakeDamage?.Invoke(this, new DamageEventData(damage));
            EventManager.Instance.OnAfterCharacterTakesDamage(this, damage, token);
            if (token.ShouldBeCancelled)
                return;

            if (Health < Mathf.Epsilon)
            {
                // Pre dead events
                EventManager.Instance.OnBeforeCharacterDead(this, token);
                if (token.ShouldBeCancelled)
                    return;

                // Logic
                Die(damage.Source);

                // Post dead events
                EventManager.Instance.OnAfterCharacterDead(this, token);
                if (token.ShouldBeCancelled)
                    return;
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
