using System;
using System.Linq;
using Assets.Scripts.EventManagement;
using Assets.Scripts.Localization;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Controlls players inventory and skills list UI interactions
    /// </summary>
    public class InventoryUIController : IUISubscriber
    {
        private enum TabState
        {
            Closed,
            InventoryTab,
            SkillTab,
        }

        private TabState state = TabState.Closed;

        protected Player Player;

        private InventoryTab inventoryTab;
        private SkillsTab skillsTab;
        private Skillbar skillbar;

        private const string inventoryTabButtonName = "InventoryTabButton";
        private const string skillsTabButtonName = "SkillsTabButton";

        public InventoryUIController(Player player)
        {
            Player = player;
        }

        public void Initialize(SkillController skillController)
        {
            UIManager.Instance.Subscribe(inventoryTabButtonName, this);
            UIManager.Instance.Subscribe(skillsTabButtonName, this);

            inventoryTab = Resources.FindObjectsOfTypeAll<InventoryTab>().FirstOrDefault();
            if (inventoryTab == null)
                throw new Exception("Inventory tab object required");

            skillsTab = Resources.FindObjectsOfTypeAll<SkillsTab>().FirstOrDefault();
            if (skillsTab == null)
                throw new Exception("Skills tab object required");

            skillbar = Resources.FindObjectsOfTypeAll<Skillbar>().FirstOrDefault();
            if (skillbar == null)
                throw new Exception("Skillbar object required");
            skillbar.Initialize(Player.SkillsAndConsumables(), skillController);
            skillbar.Hide();
            Player.Inventory.OnInventoryUpdated += HandleInventoryUpdated;
        }

        private void HandleInventoryUpdated(object sender, EventArgs args)
        {
            if (args is InventoryEventData data)
            {
                //if (data.Consumable != null)
                //    skillbar.Remove(data.Consumable.UsageEffect);
                //if (data.Skill != null)
                //    skillbar.Remove(data.Skill);
                //if (data.Item != null) TODO
                //    inventoryTab.Remove(data.Item);

                skillbar.Rebuild(Player.SkillsAndConsumables());
            }
        }

        public void Handle(UIEvent uiEvent)
        {
            if (uiEvent.ButtonName == skillsTabButtonName)
            {
                switch (state)
                {
                    case TabState.Closed:
                        state = TabState.SkillTab;
                        SkillsTabRefreshStats();
                        FillSkillsTab();
                        break;
                    case TabState.InventoryTab:
                        state = TabState.SkillTab;
                        SkillsTabRefreshStats();
                        ClearInventoryTab();
                        FillSkillsTab();
                        break;
                    case TabState.SkillTab:
                        state = TabState.Closed;
                        ClearSkillsTab();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (uiEvent.ButtonName == inventoryTabButtonName)
            {
                switch (state)
                {
                    case TabState.Closed:
                        state = TabState.InventoryTab;
                        FillInventoryTab();
                        break;
                    case TabState.InventoryTab:
                        state = TabState.Closed;
                        ClearInventoryTab();
                        break;
                    case TabState.SkillTab:
                        state = TabState.InventoryTab;
                        ClearSkillsTab();
                        FillInventoryTab();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Activates all skills on skillbar
        /// </summary>
        public void ShowSkills()
        {
            skillbar.Show();
        }

        /// <summary>
        /// Deactivates all skills on skillbar
        /// </summary>
        public void HideSkills()
        {
            skillbar.Hide();
        }

        private void SkillsTabRefreshStats()
        {
            skillsTab.HealthText.text = LocalizationManager.GetLocalizedValue(nameof(Player.Health).ToLower())
                                        + $" {Player.Health}/{Player.HealthMax}";
            skillsTab.LevelText.text = LocalizationManager.GetLocalizedValue(nameof(Player.Level).ToLower())
                                       + $" {Player.Level}";
            skillsTab.ExperienceText.text = LocalizationManager.GetLocalizedValue(nameof(Player.Experience).ToLower())
                                            + $" {Player.Experience}/{Player.ExperienceForNextLevel}";
            skillsTab.SkillPointsText.text = LocalizationManager.GetLocalizedValue(nameof(Player.SkillPoints).ToLower())
                                             + $" {Player.SkillPoints}";
        }

        private void FillInventoryTab()
        {
            inventoryTab.Initialize(Player.InventoryObjects());
            inventoryTab.gameObject.SetActive(true);
        }

        private void ClearInventoryTab()
        {
            inventoryTab.Clear();
            inventoryTab.gameObject.SetActive(false);
        }

        private void FillSkillsTab()
        {
            skillsTab.Initialize(Player.SkillBook);
            skillsTab.gameObject.SetActive(true);
        }

        private void ClearSkillsTab()
        {
            skillsTab.Clear();
            skillsTab.gameObject.SetActive(false);
        }
    }
}
