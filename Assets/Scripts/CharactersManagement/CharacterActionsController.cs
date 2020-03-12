﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Controlls character animations, movement, states
    /// </summary>
    public class CharacterActionsController : MonoBehaviour
    {
        public Character Character;
        private List<Tile> path;
        private int currentTargetTileIndex;
        private int pathOffset = 0;

        private Skill currentSkill;
        private SkillTarget currentSkillTarget;
        private float skillCastTimer;
        private bool skillEffectPlayed;

        public const string HealthbarPrefabPath = "Prefabs/HealthbarPrefab";

        private Action<int> onNewPathTileReachedAction;
        private Action onPathEndReachedAction;
        private Action onSkillCastEndAction;
        private Action onSkillCastEffectAction;

        private Animator animator;
        private const string skillAnimatorParam = "UsingSkill";
        private bool freeze = false;

        public void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Update()
        {
            if (!freeze)
            {
                MoveCharacter();
                UseCurrentSkill();
            }
        }

        private void MoveCharacter()
        {
            if (path == null || !path.Any())
                return;
            
            if (Vector2.Distance(
                    Character.transform.position,
                    path[currentTargetTileIndex].transform.position) < Mathf.Epsilon)
            {
                // Reached target tile
                if (currentTargetTileIndex - pathOffset >= 0)
                {
                    onNewPathTileReachedAction?.Invoke(currentTargetTileIndex - pathOffset);
                    if (path == null || !path.Any())
                    {
                        // Path was canceled after next tile reached
                        return;
                    }
                }
                if (currentTargetTileIndex + 1 == path.Count)
                {
                    // Reached end of the path
                    onPathEndReachedAction?.Invoke();
                    currentTargetTileIndex = 0;
                    pathOffset = 0;
                    path.Clear();
                }
                else
                {
                    currentTargetTileIndex++;
                    Character.OnTile = path[currentTargetTileIndex];
                }
                return;
            }

            Character.transform.position = Vector2.MoveTowards(
                Character.transform.position,
                path[currentTargetTileIndex].transform.position,
                Character.MovingSpeed*Time.deltaTime*0.4F);
        }

        private void UseCurrentSkill()
        {
            if (currentSkill == null)
                return;

            skillCastTimer += Time.deltaTime;
            if (!skillEffectPlayed && skillCastTimer > currentSkill.CastingEffectTime)
            {
                // Skill effect - when all effects and damage applies
                skillEffectPlayed = true;
                onSkillCastEffectAction.Invoke();
            }

            if (skillCastTimer > currentSkill.CastingTime)
            {
                // Finished using skill
                currentSkill = null;
                currentSkillTarget = null;
                animator?.SetBool(skillAnimatorParam, false);

                // In this invoke enemy can use next skill
                onSkillCastEndAction.Invoke();
            }
        }

        /// <summary>
        /// Moves character on new path
        /// </summary>
        /// <param name="newPath">Path</param>
        /// <param name="nextAction">Action to be invoked when next path tile reached</param>
        /// <param name="endAction">Action to be invoked by the end of path</param>
        public void Move(List<Tile> newPath, Action<int> nextAction, Action endAction)
        {
            onNewPathTileReachedAction = nextAction;
            onPathEndReachedAction = endAction;
            if (path != null && path.Any())
            {
                // Changing path while still moving last one
                Tile currentTargetTile = path[currentTargetTileIndex];
                path = newPath;
                path.Insert(0, currentTargetTile);
                currentTargetTileIndex = 0;
                // Offset because we are adding current target to passed as a parameter path
                pathOffset = 1;
                return;
            }

            path = newPath;
        }

        /// <summary>
        /// Plays skill animations and effects
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="target"></param>
        /// <param name="endAction">Action to call when acting finished</param>
        /// <param name="effectAction">Action to call when to apply skill effects</param>
        public void UseSkill(Skill skill, SkillTarget target, Action endAction, Action effectAction)
        {
            onSkillCastEndAction = endAction;
            onSkillCastEffectAction = effectAction;
            currentSkill = skill;
            currentSkillTarget = target;
            skillCastTimer = 0;
            skillEffectPlayed = false;

            animator?.SetBool(skillAnimatorParam, true);
        }

        /// <summary>
        /// Cancels current character movement
        /// </summary>
        public void Cancel()
        {
            currentTargetTileIndex = 0;
            pathOffset = 0;
            path.Clear();
        }

        /// <summary>
        /// Is character moving
        /// </summary>
        /// <returns></returns>
        public bool IsMoving()
        {
            return path != null && path.Any();
        }

        /// <summary>
        /// Is character using skill
        /// </summary>
        /// <returns></returns>
        public bool IsUsingSkill()
        {
            return currentSkill != null;
        }
    }
}