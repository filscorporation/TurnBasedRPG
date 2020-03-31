using Assets.Scripts.GameDataManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.RoomsManagement
{
    /// <summary>
    /// Controlls room network generation and transitions
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        private static RoomManager instance;
        public static RoomManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<RoomManager>();
                return instance;
            }
        }

        private int roomsCount = 1;

        /// <summary>
        /// Index of current loaded room
        /// </summary>
        public int CurrentRoomIndex = 0;

        /// <summary>
        /// Sets how many rooms already been generated
        /// </summary>
        /// <param name="count"></param>
        public void SetRoomsCount(int count)
        {
            roomsCount = count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomIndex">Entrance we are using</param>
        public void MoveToNextRoom(Entrance entrance)
        {
            if (entrance.RoomToIndex == -1)
            {
                // Next is new room

                // Set this entrance index where it leads to the next integer,
                // as new room with such index will be generated
                entrance.RoomToIndex = roomsCount;
                roomsCount++;

                GameParams.GameMode = GameMode.NextNewRoom;
            }
            else
            {
                // Next room already exists

                GameParams.GameMode = GameMode.NextExistingRoom;
            }

            // Save current room
            GameDataManager.Instance.Save(MainMenuManager.DefaultGameFileName);

            GameParams.GameFileToLoadName = MainMenuManager.DefaultGameFileName;
            GameParams.CurrentRoomIndex = entrance.RoomToIndex;
            GameParams.LeavedRoomIndex = CurrentRoomIndex;
            GameParams.SpawnDirection = Opposite(entrance.Direction);

            // Load next room
            SceneManager.LoadScene(MainMenuManager.GameSceneName);
        }

        /// <summary>
        /// Returns the opposite direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Direction Opposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Top:
                    return Direction.Bottom;
                case Direction.Bottom:
                    return Direction.Top;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }
    }
}
