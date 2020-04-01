using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.RoomsManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Controls game save and load
    /// </summary>
    public class GameDataManager : MonoBehaviour
    {
        public RoomGenerator RoomGenerator;

        private static GameDataManager instance;
        public static GameDataManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GameDataManager>();
                return instance;
            }
        }

        /// <summary>
        /// Save current game progress into a file
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, fileName);

            GameData oldGameData = LoadGameData(fileName);
            GameData newGameData = GetGameData();

            if (oldGameData != null)
            {
                if (!oldGameData.GameID.Equals(newGameData.GameID, StringComparison.OrdinalIgnoreCase))
                {
                    // Existring save is from another game, rewrite it
                }
                else
                {
                    // Game was previously saved - merge room data
                    MergeGameData(oldGameData, newGameData);
                }
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                binaryFormatter.Serialize(fileStream, newGameData);
            }

            sw.Stop();
            UnityEngine.Debug.Log($"Game saved in {sw.Elapsed.TotalMilliseconds} ms");
        }

        /// <summary>
        /// Initialize current game with previously saved progress
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GameData gameData = LoadGameData(fileName);

            if (gameData == null)
                throw new Exception("Save file does not exist");

            GameManager.Instance.GameID = gameData.GameID;
            RandomGenerator.Instance.Seed = gameData.Seed;
            RoomManager.Instance.CurrentRoomIndex = gameData.CurrentRoomIndex;
            RoomManager.Instance.SetRoomsCount(gameData.Rooms.Length);
            RoomGenerator.GenerateRoom(gameData);

            sw.Stop();
            UnityEngine.Debug.Log($"Game loaded in {sw.Elapsed.TotalMilliseconds} ms");
        }

        /// <summary>
        /// Initialize current game into specific room with previously saved progress
        /// </summary>
        /// <param name="entranceDirection"></param>
        /// <param name="currentRoomIndex"></param>
        /// <param name="fileName"></param>
        public void Load(Direction entranceDirection, int currentRoomIndex, string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GameData gameData = LoadGameData(fileName);

            if (gameData == null)
                throw new Exception("Save file does not exist");

            gameData.CurrentRoomIndex = currentRoomIndex;

            GameManager.Instance.GameID = gameData.GameID;
            RandomGenerator.Instance.Seed = gameData.Seed;
            RoomManager.Instance.CurrentRoomIndex = gameData.CurrentRoomIndex;
            RoomManager.Instance.SetRoomsCount(gameData.Rooms.Length);
            RoomGenerator.GenerateRoom(gameData, false);
            RoomGenerator.SpawnPlayer(entranceDirection, gameData.Player);

            sw.Stop();
            UnityEngine.Debug.Log($"Game loaded in {sw.Elapsed.TotalMilliseconds} ms");
        }

        /// <summary>
        /// Load player from file and then generate room
        /// </summary>
        /// <param name="entranceDirection"></param>
        /// <param name="currentRoomIndex"></param>
        /// <param name="leavedRoomIndex"></param>
        /// <param name="fileName"></param>
        public void LoadPlayer(Direction entranceDirection, int currentRoomIndex, int leavedRoomIndex, string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GameData gameData = LoadGameData(fileName);

            if (gameData == null)
                throw new Exception("Save file does not exist");

            GameManager.Instance.GameID = gameData.GameID;
            RandomGenerator.Instance.Seed = gameData.Seed;
            Dictionary<Direction, int> doors = new Dictionary<Direction, int>
                {
                    { Direction.Left, -1 },
                    { Direction.Right, -1 },
                    { Direction.Top, -1 },
                    { Direction.Bottom, -1},
                };
            doors[entranceDirection] = leavedRoomIndex;
            RoomManager.Instance.CurrentRoomIndex = currentRoomIndex;
            RoomManager.Instance.SetRoomsCount(gameData.Rooms.Length + 1);
            RoomGenerator.GenerateRoom(new RoomParams(100, 100, doors));
            RoomGenerator.SpawnPlayer(entranceDirection, gameData.Player);

            sw.Stop();
            UnityEngine.Debug.Log($"Game loaded in {sw.Elapsed.TotalMilliseconds} ms");
        }

        /// <summary>
        /// Load game data from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private GameData LoadGameData(string fileName)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(path))
                return null;

            GameData gameData;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            }

            return gameData;
        }

        /// <summary>
        /// Populates game data object with current room info
        /// </summary>
        /// <returns></returns>
        private GameData GetGameData()
        {
            GameData data = new GameData();
            data.CurrentRoomIndex = RoomManager.Instance.CurrentRoomIndex;

            // Game ID
            data.GameID = GameManager.Instance.GameID;

            // Seed
            data.Seed = RandomGenerator.Instance.Seed;

            // Player
            data.Player = new PlayerData(FindObjectOfType<Player>());

            // Field
            data.Rooms = new RoomData[1];
            RoomData room = new RoomData();
            data.Rooms[0] = room;
            room.Field = new FieldData();
            room.Field.Width = (short)MapManager.Instance.Field.GetLength(0);
            room.Field.Height = (short)MapManager.Instance.Field.GetLength(1);
            room.Field.Tiles = new TileData[room.Field.Width, room.Field.Height];
            for (int i = 0; i < room.Field.Width; i++)
            {
                for (int j = 0; j < room.Field.Height; j++)
                {
                    room.Field.Tiles[i, j] = new TileData(MapManager.Instance.Field[i, j].Type);
                }
            }

            // Enemies
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            room.Enemies = enemies.Select(e => new EnemyData(e)).ToArray();

            // Entrance
            Entrance[] entrances = FindObjectsOfType<Entrance>();
            room.Entrances = entrances.Select(e => new EntranceData(e)).ToArray();

            // Chest
            Chest[] chests = FindObjectsOfType<Chest>();
            room.Chests = chests.Select(c => new ChestData(c)).ToArray();

            return data;
        }

        /// <summary>
        /// Adds current room info into existing rooms data
        /// </summary>
        /// <param name="oldGameData"></param>
        /// <param name="newGameData"></param>
        private void MergeGameData(GameData oldGameData, GameData newGameData)
        {
            if (newGameData.CurrentRoomIndex == oldGameData.Rooms.Length)
            {
                // Current room is new and was not previously saved
                // Add current room to the list of rooms
                newGameData.Rooms = oldGameData.Rooms.Concat(newGameData.Rooms).ToArray();
            }
            else
            {
                // Current room have been updated and saved
                // Replace old version with this one
                RoomData currentRoom = newGameData.Rooms[0];
                newGameData.Rooms = oldGameData.Rooms;
                newGameData.Rooms[newGameData.CurrentRoomIndex] = currentRoom;
            }
        }
    }
}
