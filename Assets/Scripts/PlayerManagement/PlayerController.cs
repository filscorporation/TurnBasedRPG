using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InputManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using Assets.Scripts.SkillManagement;
using Assets.Scripts.UIManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Manages player input, movement, combat and etc
    /// </summary>
    public class PlayerController : MonoBehaviour, IInputSubscriber
    {
        public InputManagerBase InputManager;
        public EnemyController EnemyController;
        public SkillController SkillController;
        public InventoryUIController InventoryUIController;
        protected CharacterActionsController CharacterController;

        public Player Player;

        private Tile confirmTileInBattle;
        private List<Tile> savedPath;
        private readonly List<InteractableObject> interactableObjects = new List<InteractableObject>();
        private Action callWhenPlayersTurnDone;

        public void Start()
        {
            CharacterController = Player.CharacterController;
            Validate();
            SkillController = new SkillController(this, Player);
            SkillController.Initialize();
            InventoryUIController = new InventoryUIController(Player);
            InventoryUIController.Initialize(SkillController);
            InputManager.Subscribe(this);
            DamageValueEffectController.Instance.AddToShowEffectList(new[] { Player });
        }

        private void Validate()
        {
            if (InputManager == null)
                throw new Exception("InputManager field should not be null");
            if (EnemyController == null)
                throw new Exception("EnemyController field should not be null");
            if (Player == null)
                throw new Exception("Player field should not be null");
            if (CharacterController == null)
                throw new Exception("CharacterController component should be attached");
        }

        /// <summary>
        /// Passes control to player
        /// </summary>
        /// <param name="onPlayersTurnDone">After players turn done - pass control to battle manager</param>
        /// <returns>Returns false if player leaves the battle</returns>
        public bool PlayersTurn(Action onPlayersTurnDone)
        {
            Player.PlayerState = PlayerState.InBattle;
            callWhenPlayersTurnDone = onPlayersTurnDone;

            InventoryUIController.ShowSkills();

            // Refresh action points
            Player.ActionPoints = Player.ActionPointsMax;
            UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);
            
            // Set block to zero
            Player.ClearBlock();

            return true;
        }

        /// <summary>
        /// Command to finish players turn
        /// </summary>
        public void PlayersTurnEnd()
        {
            if (CharacterController.IsMoving())
                return;

            SkillController.Clear();
            InventoryUIController.HideSkills();
            UIManager.Instance.SetVariable(nameof(Player.ActionPoints), -1);

            Player.PlayerState = PlayerState.Waiting;
            callWhenPlayersTurnDone();
        }

        /// <summary>
        /// Makes battle end finishing actions
        /// </summary>
        public void PlayerEndBattle()
        {
            // Put player into free control state
            Player.PlayerState = PlayerState.FreeControl;
            // Deactivate all skills on the skillbar
            InventoryUIController.HideSkills();
            // Set block to zero
            Player.ClearBlock();
        }

        /// <summary>
        /// This will get called when InputManager will get input
        /// </summary>
        /// <param name="input">Input information</param>
        public void Handle(InputEvent input)
        {
            Tile tile = input.InputObject.GetComponent<Tile>();
            if (tile == null)
                return;

            Debug.Log($"Handling input to tile {tile.X}:{tile.Y}");

            if (Player.PlayerState == PlayerState.Waiting)
            {
                // Ignore input while waiting for the enemy
                return;
            }

            // Try using skill
            if (SkillController.HasActiveSkill())
            {
                SkillController.UseSkill(tile);
                return;
            }

            // Occupied tile processing
            if (!tile.Free)
            {
                if (tile.Occupier is InteractableObject)
                {
                    // Moving to interactable object
                }
                else
                {
                    return;
                }
            }

            // Free tile processing
            List<Tile> path;
            // Player click on tile second time in a row - confirmation for action in battle
            if (confirmTileInBattle != null && confirmTileInBattle.Equals(tile))
            {
                if (Player.ActionPoints == 0)
                {
                    ClearPlannedPath();
                    return;
                }
                path = savedPath;
                confirmTileInBattle = null;
            }
            else
            {
                if (CharacterController.IsMoving())
                {
                    if (Player.PlayerState == PlayerState.InBattle)
                        // You cant change path when in turn based move
                        return;
                }

                path = MapManager.Instance.BuildPath(Player.OnTile, tile);
                if (path == null || path.Count == 0)
                    // No path
                    throw new Exception("No path");
                
                if (Player.PlayerState == PlayerState.InBattle)
                {
                    savedPath = path;
                    confirmTileInBattle = tile;
                    return;
                }
            }

            ClearInteractions();
            CharacterController.Move(path, PlayerReachedNextTile, PlayerReachedPathEnd);
        }

        /// <summary>
        /// Clears planned path for the player
        /// </summary>
        public void ClearPlannedPath()
        {
            MapManager.Instance.ClearPath();
            SkillController.Clear();
            savedPath = null;
            confirmTileInBattle = null;
        }

        private void PlayerReachedNextTile(int tileIndex)
        {
            MapManager.Instance.ClearPath(tileIndex);

            if (Player.PlayerState == PlayerState.InBattle)
            {
                // If not fist tile of the path - take action point
                if (tileIndex != 0)
                {
                    Player.ActionPoints--;
                    UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);
                    if (EnemyController.TryAddEnemyToBattle(Player))
                    {
                        // Got in line of sight of new enemy
                    }
                }

                if (Player.ActionPoints == 0)
                {
                    CharacterController.Cancel();
                    MapManager.Instance.ClearPath();
                    savedPath = null;
                    confirmTileInBattle = null;
                    // Player used all AP, now waiting for EndTurnButton to be pressed
                    return;
                }
            }

            if (Player.PlayerState == PlayerState.FreeControl && EnemyController.TryStartBattle(Player))
            {
                // Battle began
                ClearInteractions();
                CharacterController.Cancel();
                MapManager.Instance.ClearPath();
            }
        }

        private void PlayerReachedPathEnd()
        {
            Debug.Log("Path end reached");
            MapManager.Instance.ClearPath();

            if (Player.PlayerState == PlayerState.FreeControl)
            {
                if (EnemyController.TryStartBattle(Player))
                {
                    // Battle began
                    return;
                }

                CheckForInteractions();
            }
        }

        private void CheckForInteractions()
        {
            foreach (Tile tile in MapManager.Instance.GetNeighbours(Player.OnTile))
            {
                if (!tile.Free && tile.Occupier is InteractableObject interactable)
                {
                    Debug.Log($"Interacting with {interactable.name}");
                    interactable.ShowInteraction(Player);
                    interactableObjects.Add(interactable);
                }
            }
        }

        private void ClearInteractions()
        {
            foreach (InteractableObject interactableObject in interactableObjects)
            {
                interactableObject.Clear();
            }
            interactableObjects.Clear();
        }
    }
}
