using System;
using System.Linq;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement;

namespace Assets.Scripts.SkillManagement
{
    /// <summary>
    /// Controls players skills usage
    /// </summary>
    public class SkillController
    {
        protected PlayerController PlayerController;
        protected Player Player;

        private Skill activeSkill;
        private SkillTarget currentTarget;
        private bool isUsing = false;

        public SkillController(PlayerController playerController, Player player)
        {
            PlayerController = playerController;
            Player = player;
        }

        public void Initialize()
        {

        }

        /// <summary>
        /// Handles player click on skill button in skillbar
        /// </summary>
        /// <param name="skill"></param>
        /// <returns>Is skill activated</returns>
        public bool HandleSkillClick(Skill skill)
        {
            return TrySetSkillActive(skill);
        }

        private bool TrySetSkillActive(Skill skill)
        {
            if (Player.PlayerState != PlayerState.InBattle)
            {
                // Cant use skills outside of battle
                return false;
            }

            if (Player.ActionPoints < skill.Cost)
            {
                // Not enough skill points
                return false;
            }

            // Skill activation events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnSkillActivation(Player, skill, token);
            if (token.ShouldBeCancelled)
                return false;

            PlayerController.ClearPlannedPath();

            activeSkill = skill;
            skill.HighlightTargetTiles(Player.OnTile);

            return true;
        }

        /// <summary>
        /// Use active skill
        /// </summary>
        public void UseSkill(Tile tile)
        {
            if (isUsing)
                return;

            if (activeSkill == null)
                return;

            if (Player.ActionPoints < activeSkill.Cost)
                throw new Exception("Activated skill with too high cost");

            SkillTarget target;

            switch (activeSkill.TargetType)
            {
                case SkillTargetType.Player:
                    if (!(tile.Occupier is Player self))
                        return;
                    // Used skill on yourself
                    target = new SkillTarget(self);
                    break;
                case SkillTargetType.Enemy:
                    if (!(tile.Occupier is Character enemy))
                        return;
                    if (enemy == Player)
                        // Cant attack self with damaging skills
                        return;
                    if (!activeSkill.InRange(Player, enemy))
                        // Cant reach target
                        return;
                    // Attacked an enemy
                    target = new SkillTarget(enemy);
                    break;
                case SkillTargetType.Tile:
                    if (!activeSkill.InRange(Player, tile))
                        // Cant reach target
                        return;
                    target = new SkillTarget(tile);
                    break;
                default:
                    throw new IndexOutOfRangeException(activeSkill.TargetType.ToString());
            }

            // Skill using events
            CancellationToken token = new CancellationToken();
            EventManager.Instance.OnSkillUse(Player, activeSkill, target, token);
            if (token.ShouldBeCancelled)
                return;

            currentTarget = target;
            isUsing = true;
            Player.State = activeSkill.CharacterTargetState;
            Player.ActionPoints -= activeSkill.Cost;
            UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);
            Player.CharacterController.UseSkill(activeSkill, target, FinishUsingSkill, UseSkillEffect);
        }

        private void FinishUsingSkill()
        {
            Clear();
            isUsing = false;
        }

        private void UseSkillEffect()
        {
            activeSkill.Use(Player, currentTarget);
            // If skill was effect of consumable item - call inventory to decrease its amount
            if (activeSkill.SourceConsumable != null)
                Player.Inventory.Consume(activeSkill.SourceConsumable);
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
            activeSkill?.ClearHighlighted();
            activeSkill = null;
        }
    }
}
