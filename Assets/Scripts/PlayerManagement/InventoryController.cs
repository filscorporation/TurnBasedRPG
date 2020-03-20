using System;
using System.Linq;
using Assets.Scripts.Localization;
using Assets.Scripts.UIManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Controlls players inventory and skills list interactions
    /// </summary>
    public class InventoryController : IUISubscriber
    {
        private enum TabState
        {
            Closed,
            InventoryTab,
            SkillTab,
        }

        private TabState state = TabState.Closed;

        protected Player Player;

        private SkillsTab skillsTab;

        private const string inventoryTabButtonName = "InventoryTabButton";
        private const string skillsTabButtonName = "SkillsTabButton";

        public InventoryController(Player player)
        {
            Player = player;
        }

        public void Initialize()
        {
            UIManager.Instance.Subscribe(skillsTabButtonName, this);
            skillsTab = Resources.FindObjectsOfTypeAll<SkillsTab>().FirstOrDefault();
            if (skillsTab == null)
                throw new Exception("Skills tab object required");
        }

        public void Handle(UIEvent uiEvent)
        {
            if (uiEvent.ButtonName == skillsTabButtonName)
            {
                switch (state)
                {
                    case TabState.Closed:
                        state = TabState.SkillTab;
                        RefreshStats();
                        FillSkillsTab();
                        break;
                    case TabState.InventoryTab:
                        state = TabState.SkillTab;
                        RefreshStats();
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
        }

        private void RefreshStats()
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
        }

        private void ClearInventoryTab()
        {

        }

        private void FillSkillsTab()
        {
            skillsTab.Initialize(Player.Skills);
            skillsTab.gameObject.SetActive(true);
        }

        private void ClearSkillsTab()
        {
            skillsTab.Clear();
            skillsTab.gameObject.SetActive(false);
        }
    }
}
