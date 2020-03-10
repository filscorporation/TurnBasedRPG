using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapManagement;
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

        public const string HealthbarPrefabPath = "Prefabs/HealthbarPrefab";

        private Action<Character, int> onNewPathTileReachedAction;
        private Action<Character> onPathEndReachedAction;

        private bool freeze = false;

        public void Update()
        {
            if (!freeze)
                MoveCharacter();
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
                    onNewPathTileReachedAction?.Invoke(Character, currentTargetTileIndex - pathOffset);
                    if (path == null || !path.Any())
                    {
                        // Path was canceled after next tile reached
                        return;
                    }
                }
                if (currentTargetTileIndex + 1 == path.Count)
                {
                    // Reached end of the path
                    onPathEndReachedAction?.Invoke(Character);
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

        /// <summary>
        /// Moves character on new path
        /// </summary>
        /// <param name="newPath">Path</param>
        /// <param name="nextAction">Action to be invoked when next path tile reached</param>
        /// <param name="endAction">Action to be invoked by the end of path</param>
        public void Move(List<Tile> newPath, Action<Character, int> nextAction, Action<Character> endAction)
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
    }
}
