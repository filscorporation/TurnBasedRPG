using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
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

        public void Save(string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                GameData gameData = GetGameData();
                binaryFormatter.Serialize(fileStream, gameData);
            }

            sw.Stop();
            UnityEngine.Debug.Log($"Game saved in {sw.Elapsed.TotalMilliseconds}");
        }

        public void Load(string fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, fileName);

            GameData gameData;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            }

            RandomGenerator.Instance.Seed = gameData.Seed;
            RoomGenerator.GenerateRoom(gameData);

            sw.Stop();
            UnityEngine.Debug.Log($"Game loaded in {sw.Elapsed.TotalMilliseconds}");
        }

        private GameData GetGameData()
        {
            GameData data = new GameData();

            // Seed
            data.Seed = RandomGenerator.Instance.Seed;

            // Player
            data.Player = new PlayerData(FindObjectOfType<Player>());

            // Field
            data.Room = new RoomData();
            data.Room.Field = new FieldData();
            data.Room.Field.Width = (short)MapManager.Instance.Field.GetLength(0);
            data.Room.Field.Height = (short)MapManager.Instance.Field.GetLength(1);
            data.Room.Field.Tiles = new TileData[data.Room.Field.Width, data.Room.Field.Height];
            for (int i = 0; i < data.Room.Field.Width; i++)
            {
                for (int j = 0; j < data.Room.Field.Height; j++)
                {
                    data.Room.Field.Tiles[i, j] = new TileData(MapManager.Instance.Field[i, j].Type);
                }
            }

            // Enemies
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            data.Room.Enemies = enemies.Select(e => new EnemyData(e)).ToArray();

            // Entrance
            Entrance[] entrances = FindObjectsOfType<Entrance>();
            data.Room.Entrances = entrances.Select(e => new EntranceData(e)).ToArray();

            // Chest
            Chest[] chests = FindObjectsOfType<Chest>();
            data.Room.Chests = chests.Select(c => new ChestData(c)).ToArray();

            return data;
        }
    }
}
