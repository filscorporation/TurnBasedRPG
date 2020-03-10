using System;
using System.Linq;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.UIManagement;
using UnityEngine.EventSystems;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Controlls all skills interactions
    /// </summary>
    public class SkillController : IUISubscriber
    {
        protected PlayerController PlayerController;
        protected MapManager MapManager;
        protected Player Player;

        private Skill activeSkill;

        private const string buttonString = "Button";

        public SkillController(PlayerController playerController, MapManager mapManager, Player player)
        {
            PlayerController = playerController;
            MapManager = mapManager;
            Player = player;
        }

        public void Initialize()
        {
            foreach (Skill skill in Player.Skills)
            {
                UIManager.Instance.Subscribe(skill.Name + buttonString, this);
            }
        }

        public void Handle(UIEvent uiEvent)
        {
            if (uiEvent.ButtonName.EndsWith(buttonString))
            {
                Skill skill = Player.Skills.FirstOrDefault(s => s.Name + buttonString == uiEvent.ButtonName);
                if (skill == null)
                    throw new NotSupportedException($"Skill button {uiEvent.ButtonName}");
                
                if (TrySetSkillActive(skill))
                {
                    UIManager.Instance.SetVariable(uiEvent.ButtonName, 2);
                }
            }
        }

        private bool TrySetSkillActive(Skill skill)
        {
            if (Player.ActionPoints < skill.Cost)
            {
                // Not enougth skill points
                return false;
            }

            PlayerController.ClearPlannedPath();

            activeSkill = skill;
            return true;
        }

        /// <summary>
        /// Use active skill
        /// </summary>
        public bool UseSkill(Tile tile)
        {
            if (activeSkill == null)
                return false;

            if (!(tile.Occupier is Character target))
            {
                // Attacked some tile
                return false;
            }

            if (!activeSkill.InRange(Player, target))
            {
                // Cant reach target
                return false;
            }

            if (Player.ActionPoints < activeSkill.Cost)
                throw new Exception("Acivated skill with too high cost");

            Player.ActionPoints -= activeSkill.Cost;
            UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);

            // TODO: temp for test
            target.TakeDamage(new Damage(Player, activeSkill.Damage));

            Clear();
            return true;
        }

        /// <summary>
        /// Return true if player clicked on skill button
        /// </summary>
        /// <returns></returns>
        public bool HasActiveSkill()
        {
            return activeSkill != null;
        }

        /// <summary>
        /// Deactivate active skill
        /// </summary>
        public void Clear()
        {
            if (activeSkill == null)
                return;

            UIManager.Instance.SetVariable(activeSkill.Name + buttonString, 1);
            activeSkill = null;
        }

        /// <summary>
        /// Hides skills UI
        /// </summary>
        public void HideSkills()
        {
            foreach (Skill skill in Player.Skills)
            {
                UIManager.Instance.SetVariable(skill.Name + buttonString, 0);
            }
        }

        /// <summary>
        /// Shows skills UI
        /// </summary>
        public void ShowSkills()
        {
            foreach (Skill skill in Player.Skills)
            {
                UIManager.Instance.SetVariable(skill.Name + buttonString, 1);
            }
        }
    }
}
