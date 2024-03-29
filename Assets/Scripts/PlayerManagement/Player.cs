﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement;
using Assets.Scripts.UIManagement.Tabs;
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

        public int level;
        public int Level
        {
            get => level;
            set
            {
                level = value;
                SetExperienceForNextLevel();
            }
        }
        public int SkillPoints = 0;
        public int experience = 0;
        public int Experience
        {
            get => experience;
            set => GainExperience(value);
        }
        public int ExperienceForNextLevel { get; private set; }
        public int ExperienceForFirstLevel = 100;
        private float experienceGrowthFactor = 1.5F;

        /// <summary>
        /// Players collection of skill that he can add to skillbar
        /// </summary>
        public List<Skill> SkillBook;

        /// <summary>
        /// Players collection of items
        /// </summary>
        public Inventory Inventory;

        public new void Start()
        {
            if (!IsLoaded)
            {            
                Inventory = new Inventory();
                Inventory.Initialize();
                Level = 1;
            }
            Inventory.SubscribeToEvents();

            base.Start();

            // TODO: will be changed with skill dictionary implementation
            // and for now skills always contains skill book
            SkillBook = new List<Skill>(Skills);
        }

        public IEnumerable<Skill> SkillsAndConsumables()
        {
            return Skills.Concat(Inventory.Consumables.Select(c => c.UsageEffect));
        }

        public IEnumerable<IInventoryObject> InventoryObjects()
        {
            List<IInventoryObject> objects = new List<IInventoryObject> { new GoldInventoryObject() };
            return objects.Concat(Inventory.Items.Concat(Inventory.Consumables.Cast<IInventoryObject>()));
        }

        protected void GainExperience(int exp)
        {
            if (experience + exp >= ExperienceForNextLevel)
            {
                int overflow = experience + exp - ExperienceForNextLevel;
                experience = 0;
                LevelUp();
                SetExperienceForNextLevel();
                GainExperience(overflow);
            }
            else
            {
                Debug.Log($"Player gains {exp} experience");
                experience += exp;
            }
        }

        /// <summary>
        /// Calculates how much experience needed for specific level
        /// </summary>
        private void SetExperienceForNextLevel()
        {
            ExperienceForNextLevel = Mathf.RoundToInt(
                ExperienceForFirstLevel * Mathf.Pow(experienceGrowthFactor, Level - 1) / 10) * 10;
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
            // For testing - load main menu
            SceneManager.LoadScene(MainMenuManager.MainMenuSceneName);
        }
    }
}
