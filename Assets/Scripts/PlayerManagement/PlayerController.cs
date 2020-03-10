﻿using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InputManagement;
using Assets.Scripts.MapManagement;
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
        public MapManager MapManager;
        public EnemyController EnemyController;
        protected CharacterActionsController CharacterController;

        public Player Player;

        private Tile confirmTileInBattle;
        private List<Tile> savedPath;
        private Action callWhenPlayersTurnDone;

        public void Start()
        {
            CharacterController = GetComponent<CharacterActionsController>();
            Validate();
            InputManager.Subscribe(this);
        }

        private void Validate()
        {
            if (InputManager == null)
                throw new Exception("InputManager field should not be null");
            if (MapManager == null)
                throw new Exception("MapManager field should not be null");
            if (EnemyController == null)
                throw new Exception("EnemyController field should not be null");
            if (Player == null)
                throw new Exception("Player field should not be null");
            if (CharacterController == null)
                throw new Exception("CharacterController should not be null");
        }

        /// <summary>
        /// Passes control to player
        /// </summary>
        /// <param name="onPlayersTurnDone">After players turn done - pass control to battle manager</param>
        public void PlayersTurn(Action onPlayersTurnDone)
        {
            Player.State = PlayerState.InBattle;
            callWhenPlayersTurnDone = onPlayersTurnDone;

            // Refresh action points
            Player.ActionPoints = Player.ActionPointsMax;
            UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);
        }

        /// <summary>
        /// Command to finish players turn
        /// </summary>
        public void PlayersTurnEnd()
        {
            Player.State = PlayerState.Waiting;
            callWhenPlayersTurnDone();
        }

        /// <summary>
        /// This will get call when InputManager will get input
        /// </summary>
        /// <param name="input">Input information</param>
        public void Handle(InputEvent input)
        {
            Tile tile = input.InputObject.GetComponent<Tile>();
            if (tile == null)
                return;

            if (Player.State == PlayerState.Waiting)
            {
                // Ignore input while waiting for the enemy
                return;
            }

            if (!tile.Free)
            {
                return;
            }

            Debug.Log($"Handling input to tile {tile.X}:{tile.Y}");

            List<Tile> path;
            // Processing clicked tile
            if (confirmTileInBattle != null && confirmTileInBattle.Equals(tile))
            {
                if (Player.ActionPoints == 0)
                {
                    MapManager.ClearPath();
                    savedPath = null;
                    confirmTileInBattle = null;
                    return;
                }
                path = savedPath;
                confirmTileInBattle = null;
            }
            else
            {
                if (CharacterController.IsMoving())
                {
                    if (Player.State == PlayerState.InBattle)
                        // You cant change path when in turn based move
                        return;
                }

                path = MapManager.BuildPath(Player.OnTile, tile);
                if (path == null || path.Count == 0)
                    // No path
                    throw new Exception("No path");
                
                if (Player.State == PlayerState.InBattle)
                {
                    savedPath = path;
                    confirmTileInBattle = tile;
                    return;
                }
            }
            
            CharacterController.Move(path, PlayerReachedNextTile, PlayerReachedPathEnd);
        }

        private void PlayerReachedNextTile(Character player, int tileIndex)
        {
            MapManager.ClearPath(tileIndex);

            if (Player.State == PlayerState.InBattle)
            {
                // If not fist tile of the path - take action point
                if (tileIndex != 0)
                {
                    Player.ActionPoints--;
                    UIManager.Instance.SetVariable(nameof(Player.ActionPoints), Player.ActionPoints);
                }

                if (Player.ActionPoints == 0)
                {
                    CharacterController.Cancel();
                    MapManager.ClearPath();
                    savedPath = null;
                    confirmTileInBattle = null;
                    // Player used all AP, now waiting for EndTurnButton to be pressed
                    return;
                }
            }

            if (Player.State == PlayerState.FreeControl && EnemyController.CheckIfStartBattle(Player))
            {
                // Battle began
                CharacterController.Cancel();
                MapManager.ClearPath();
            }
        }

        private void PlayerReachedPathEnd(Character player)
        {
            Debug.Log("Path end reached");
            MapManager.ClearPath();

            if (Player.State == PlayerState.FreeControl && EnemyController.CheckIfStartBattle(Player))
            {
                // Battle began
            }
        }
    }
}
