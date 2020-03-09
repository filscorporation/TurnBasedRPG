using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.InputManagement;
using Assets.Scripts.MapManagement;
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
        protected CharacterActionsController CharacterController;

        public Player Player;

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
            if (Player == null)
                throw new Exception("Player field should not be null");
            if (CharacterController == null)
                throw new Exception("CharacterController should not be null");
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

            Debug.Log($"Handling input to tile {tile.X}:{tile.Y}");

            // Processing clicked tile
            List<Tile> path = MapManager.BuildPath(Player.OnTile, tile);
            if (path == null || path.Count == 0)
                // No path
                throw new Exception("No path");

            Player.State = PlayerState.Moving;
            CharacterController.Move(path, (c, i) => MapManager.ClearPath(i), PlayerReachedPathEnd);
        }

        private void PlayerReachedPathEnd(Character player)
        {
            Debug.Log("Path end reached");
            Player.State = PlayerState.Idle;
            MapManager.ClearPath();
        }
    }
}
