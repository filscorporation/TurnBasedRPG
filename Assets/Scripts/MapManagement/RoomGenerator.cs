using Assets.Scripts.EnemyManagement;
using Assets.Scripts.GameDataManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using Assets.Scripts.SkillManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Generates room: tiles, decorations, enemies, player, objects from parameters or loaded data
    /// </summary>
    public class RoomGenerator : MonoBehaviour
    {
        private static RoomGenerator instance;
        public static RoomGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<RoomGenerator>();
                return instance;
            }
        }

        public GameObject Player;

        public GameObject DefaultTile;
        public List<Sprite> GrassDetailsTiles;
        public List<Sprite> DirtTiles;
        public Sprite Grid;
        public List<GameObject> Trees;
        public List<GameObject> OnGroundDetails;

        // TODO: enemy groups
        public List<GameObject> Enemies;

        public GameObject Chest;
        public GameObject Door;

        private const string MapSortingLayer = "Map";
        private const string EnemiesParentObjectName = "Enemies";

        public object RoomsManager { get; private set; }

        /// <summary>
        /// Generates room from parameters
        /// </summary>
        /// <param name="roomParams"></param>
        public void GenerateRoom(RoomParams roomParams)
        {
            // Generating offsets and steps for perlin noise
            // For tiles
            float pnox = RandomGenerator.Instance.RandomFloat();
            float pnoy = RandomGenerator.Instance.RandomFloat();
            // For trees
            float pntox = RandomGenerator.Instance.RandomFloat();
            float pntoy = RandomGenerator.Instance.RandomFloat();
            float pnstep = 0.1F;
            // Generates field
            Tile[,] field = new Tile[roomParams.Width, roomParams.Height];
            for (int i = 0; i < roomParams.Width; i++)
            {
                for (int j = 0; j < roomParams.Height; j++)
                {
                    TileParams tp = new TileParams(i, j, roomParams, pnox, pnoy, pnstep, pntox, pntoy);
                    field[i, j] = RandomTile(tp);
                }
            }
            // Put field into map manager
            MapManager.Instance.SetField(field);

            // Spawns enemy groups and chests
            var minimums = GetLocalMaximums(field, roomParams.Width, roomParams.Height, pntox, pntoy, pnstep);
            GameObject enemyParent = new GameObject(EnemiesParentObjectName);
            foreach (Tile tile in minimums)
            {
                if (tile.Free)
                {
                    AddEnemyGroup(tile, roomParams, enemyParent.transform);
                }
            }

            // Places doors (entrance) in different room sides
            foreach (KeyValuePair<Direction, int> pair in roomParams.Doors)
            {
                SpawnDoor(field, pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Generates room from loaded data
        /// </summary>
        /// <param name="roomParams"></param>
        /// <param name="spawnPlayer"></param>
        public void GenerateRoom(GameData data, bool spawnPlayer = true)
        {
            RoomData room = data.Rooms[data.CurrentRoomIndex];
            // Generates field
            int w = room.Field.Width;
            int h = room.Field.Height;
            Tile[,] field = new Tile[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    field[i, j] = LoadedTile(i, j, w, h, room.Field.Tiles[i, j]);
                }
            }
            // Put field into map manager
            MapManager.Instance.SetField(field);

            // Spawns enemies
            GameObject enemyParent = new GameObject(EnemiesParentObjectName);
            foreach (EnemyData enemyData in room.Enemies)
            {
                // TODO: use enemy dictionary
                GameObject enemyGO = Enemies.First(e => e.GetComponent<Enemy>().Name == enemyData.Name);
                Enemy enemy = SpawnEnemy(field[enemyData.OnTileX, enemyData.OnTileY], enemyGO, enemyParent.transform);
                // We are not setting enemy as loaded because it gets its skill from names from prefab
            }

            // Spawns chests
            foreach (ChestData chestData in room.Chests)
            {
                Chest chest = SpawnChest(field[chestData.OnTileX, chestData.OnTileY]);
                chest.IsLooted = chestData.IsLooted;
                chest.Items = chestData.Items.ToList();
            }

            // Spawns doors
            foreach (EntranceData entranceData in room.Entrances)
            {
                SpawnDoor(field[entranceData.OnTileX, entranceData.OnTileY], entranceData.RoomToIndex,
                    (Direction)entranceData.Direction);
            }

            if (spawnPlayer)
            {
                // Spawns player
                SpawnPlayer(field, data.Player);
            }
        }

        /// <summary>
        /// Spawns player at a random entrance
        /// </summary>
        public void SpawnPlayer()
        {
            foreach (Tile tile in FindObjectsOfType<Entrance>().Select(d => d.OnTile))
            {
                Tile freeTile = MapManager.Instance.GetNeighbours(tile).FirstOrDefault(t => t.Free);
                if (freeTile != null)
                {
                    SpawnPlayer(freeTile);
                    return;
                }
            }
            // No free tiles near entrance
            SpawnPlayer(FindObjectOfType<Entrance>().OnTile);
        }

        /// <summary>
        /// Spawns player at a certain entrance
        /// </summary>
        /// <param name="entranceDirection"></param>
        /// <param name="playerData"></param>
        public void SpawnPlayer(Direction entranceDirection, PlayerData playerData)
        {
            Player player = SpawnPlayer(MapManager.Instance.Field, playerData);

            Entrance entrance = FindObjectsOfType<Entrance>().First(e => e.Direction == entranceDirection);

            Tile freeTile = MapManager.Instance.GetNeighbours(entrance.OnTile).FirstOrDefault(t => t.Free);
            if (freeTile == null)
                freeTile = entrance.OnTile;

            player.OnTile = freeTile;
            player.gameObject.transform.position = freeTile.transform.position;
        }

        #region Private

        /// <summary>
        /// Generates random tile (grass, dirst, details) based on perlin noise values
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private Tile RandomTile(TileParams tp)
        {
            // Perlin noise values
            float pnx = tp.pnox + tp.x * tp.pnstep;
            float pny = tp.pnoy + tp.y * tp.pnstep;
            Vector2 pos = new Vector2(tp.x - tp.roomParams.Width / 2, tp.y - tp.roomParams.Height / 2);
            Transform parent = MapManager.Instance.TilesParent;
            GameObject tilePrefab = DefaultTile;

            GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, parent);
            Tile tile = tileGO.GetComponent<Tile>();
            tile.X = tp.x;
            tile.Y = tp.y;
            tile.Type = new TileType();

            // Tile details
            float s = RandomGenerator.Instance.RandomFloat(0.6F, 0.75F);
            float nv = Mathf.PerlinNoise(pnx, pny);
            if (nv > s)
            {
                int dirtIndex = RandomGenerator.Instance.RandomInt(DirtTiles.Count);
                tile.Type.Dirt = (short)dirtIndex;
                Sprite dirt = DirtTiles[dirtIndex];
                tileGO.GetComponent<SpriteRenderer>().sprite = dirt;
            }
            else
            {
                float detailsProb = 0.2F;
                bool isDetails = RandomGenerator.Instance.RandomFloat() < detailsProb;
                if (isDetails)
                {
                    int detailsIndex = RandomGenerator.Instance.RandomInt(GrassDetailsTiles.Count);
                    tile.Type.Details = (short)detailsIndex;
                    Sprite details = GrassDetailsTiles[detailsIndex];
                    tileGO.GetComponent<SpriteRenderer>().sprite = details;
                }
            }

            // Grid
            GameObject grid = new GameObject("Grid");
            grid.transform.parent = tileGO.transform;
            grid.transform.localScale = new Vector3(1, 1, 1);
            grid.transform.localPosition = new Vector3(0, 0, 0);
            SpriteRenderer sr = grid.AddComponent<SpriteRenderer>();
            sr.sprite = Grid;
            sr.sortingLayerName = MapSortingLayer;

            // Trees
            float pntx = tp.pntox + tp.x * tp.pnstep;
            float pnty = tp.pntoy + tp.y * tp.pnstep;
            float st = tp.roomParams.TreesDensity;
            float nvt = Mathf.PerlinNoise(pntx, pnty);
            if (nvt < st)
            {
                int treeIndex = RandomGenerator.Instance.RandomInt(Trees.Count);
                tile.Type.Tree = (short)treeIndex;
                GameObject tgo = Instantiate(
                         Trees[treeIndex],
                         tileGO.transform.position,
                         Quaternion.identity,
                         tileGO.transform);
                tile.Free = false;
            }

            // On ground details
            if (tile.Free)
            {
                float ogDetailsProb = 0.05F;
                bool isOGDetails = RandomGenerator.Instance.RandomFloat() < ogDetailsProb;
                if (isOGDetails)
                {
                    int detailsIndex = RandomGenerator.Instance.RandomInt(OnGroundDetails.Count);
                    tile.Type.OnGroundObject = (short)detailsIndex;
                    GameObject ogdgo = Instantiate(
                        OnGroundDetails[detailsIndex],
                        tileGO.transform.position,
                        Quaternion.identity,
                        tileGO.transform);
                    tile.Free = false;
                }
            }

            return tile;
        }

        private Tile LoadedTile(int x, int y, int width, int height, TileData tileData)
        {
            Vector2 pos = new Vector2(x - width / 2, y - height / 2);
            Transform parent = MapManager.Instance.TilesParent;
            GameObject tilePrefab = DefaultTile;

            GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, parent);
            Tile tile = tileGO.GetComponent<Tile>();
            tile.X = x;
            tile.Y = y;
            tile.Type = new TileType
            {
                Details = tileData.Details,
                Dirt = tileData.Dirt,
                Tree = tileData.Tree,
                OnGroundObject = tileData.OnGroundObject,
            };

            // Grid
            GameObject grid = new GameObject("Grid");
            grid.transform.parent = tileGO.transform;
            grid.transform.localScale = new Vector3(1, 1, 1);
            grid.transform.localPosition = new Vector3(0, 0, 0);
            SpriteRenderer sr = grid.AddComponent<SpriteRenderer>();
            sr.sprite = Grid;
            sr.sortingLayerName = MapSortingLayer;

            if (tile.Type.Details != -1)
            {
                Sprite details = GrassDetailsTiles[tile.Type.Details];
                tileGO.GetComponent<SpriteRenderer>().sprite = details;
            }
            if (tile.Type.Dirt != -1)
            {
                Sprite dirt = DirtTiles[tile.Type.Dirt];
                tileGO.GetComponent<SpriteRenderer>().sprite = dirt;
            }
            if (tile.Type.Tree != -1)
            {
                GameObject tgo = Instantiate(
                         Trees[tile.Type.Tree],
                         tileGO.transform.position,
                         Quaternion.identity,
                         tileGO.transform);
                tile.Free = false;
            }
            if (tile.Type.OnGroundObject != -1)
            {
                GameObject ogdgo = Instantiate(
                    OnGroundDetails[tile.Type.OnGroundObject],
                    tileGO.transform.position,
                    Quaternion.identity,
                    tileGO.transform);
                tile.Free = false;
            }

            return tile;
        }

        /// <summary>
        /// Gets points of local maximums of perlin noise as an options to spawn enemies
        /// </summary>
        /// <param name="field"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="pnox"></param>
        /// <param name="pnoy"></param>
        /// <param name="pnstep"></param>
        /// <returns></returns>
        private IEnumerable<Tile> GetLocalMaximums(Tile[,] field, int w, int h, float pnox, float pnoy, float pnstep)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = pnox + i * pnstep;
                    float y = pnoy + j * pnstep;
                    float v = Mathf.PerlinNoise(x, y);
                    // Get noise value for all tiles around
                    // If there is edge of the map, use 1 to NOT spawn
                    float l = i - 1 < 0  ? 1 : Mathf.PerlinNoise(x - pnstep, y);
                    float r = i + 1 == w ? 1 : Mathf.PerlinNoise(x + pnstep, y);
                    float u = j - 1 < 0  ? 1 : Mathf.PerlinNoise(x, y - pnstep);
                    float d = j + 1 == h ? 1 : Mathf.PerlinNoise(x, y + pnstep);

                    if (v > l && v > r && v > u && v > d)
                    {
                        // Value at this point of the noise map is greater than around
                        // so its local maximum
                        yield return field[i, j];
                    }
                }
            }
        }

        private void AddEnemyGroup(Tile tile, RoomParams roomParams, Transform parent)
        {
            int enemyIndex = RandomGenerator.Instance.RandomInt(Enemies.Count);
            Enemy enemy = Enemies[enemyIndex].GetComponent<Enemy>();
            int enemyCount = RandomGenerator.Instance.RandomInt(enemy.GroupMinSize, enemy.GroupMaxSize);
            
            if (enemyCount == 1)
            {
                SpawnEnemy(tile, Enemies[enemyIndex], parent);
                return;
            }

            bool isChest = RandomGenerator.Instance.RandomFloat() < roomParams.ChestProbability;
            List<Tile> toSpawnTiles = MapManager.Instance.GetNeighbours(tile, 2).ToList();
            Shuffle(toSpawnTiles);
            foreach (Tile n in toSpawnTiles)
            {
                if (n.Free)
                {
                    if (isChest)
                    {
                        SpawnChest(n);
                        isChest = false;
                        continue;
                    }
                    SpawnEnemy(n, Enemies[enemyIndex], parent);
                    enemyCount--;
                    if (enemyCount == 0)
                        break;
                }
            }
        }

        private Enemy SpawnEnemy(Tile tile, GameObject enemy, Transform parent)
        {
            GameObject go = Instantiate(enemy, tile.transform.position, Quaternion.identity, parent);
            go.GetComponent<Enemy>().OnTile = tile;

            return go.GetComponent<Enemy>();
        }

        private Chest SpawnChest(Tile tile)
        {
            GameObject go = Instantiate(Chest, tile.transform.position, Quaternion.identity);
            tile.Free = false;

            return go.GetComponent<Chest>();
        }

        private void SpawnDoor(Tile[,] field, Direction direction, int roomIndex)
        {
            Tile tile;
            switch(direction)
            {
                case Direction.Left:
                    tile = field[0, field.GetLength(1) / 2];
                    break;
                case Direction.Right:
                    tile = field[field.GetLength(0) - 1, field.GetLength(1) / 2];
                    break;
                case Direction.Top:
                    tile = field[field.GetLength(0) / 2, field.GetLength(1) - 1];
                    break;
                case Direction.Bottom:
                    tile = field[field.GetLength(0) / 2, 0];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SpawnDoor(tile, roomIndex, direction);
        }

        private void SpawnDoor(Tile tile, int roomIndex, Direction direction)
        {
            GameObject go = Instantiate(Door, tile.transform.position, Quaternion.identity);
            Entrance entrance = go.GetComponent<Entrance>();
            entrance.OnTile = tile;
            entrance.RoomToIndex = roomIndex;
            entrance.Direction = direction;
        }

        private Player SpawnPlayer(Tile[,] field, PlayerData playerData)
        {
            Tile tile = field[playerData.OnTileX, playerData.OnTileY];
            Player player = SpawnPlayer(tile);

            player.MovingSpeed = playerData.MovingSpeed;
            player.ActionPoints = playerData.ActionPoints;
            player.ActionPointsMax = playerData.ActionPointsMax;
            player.Health = playerData.Health;
            player.HealthMax = playerData.HealthMax;
            player.Skills = playerData.Skills
                .Select(s => GetSkillFromString(s)).ToList();
            player.Level = playerData.Level;
            player.SkillPoints = playerData.SkillPoints;
            player.Experience = playerData.Experience;
            // TODO: skill level
            player.SkillBook = playerData.SkillBook.Select(s => Skill.SkillDictionary[s.Name]).ToList();
            // TODO: amount
            player.Inventory.Consumables = playerData.Consumables
                .Select(c => Consumable.ConsumablesDictionary[c.Name]).ToList();
            player.Inventory.Items = playerData.Items.Select(i => Item.ItemDictionary[i]).ToList();
            player.Inventory.Gold = playerData.Gold;

            player.SetIsLoaded();

            return player;
        }

        private Skill GetSkillFromString(string skillName)
        {
            if (Skill.SkillDictionary.TryGetValue(skillName, out Skill skill))
                return skill;
            return Consumable.ConsumablesDictionary[skillName].UsageEffect;
        }

        private Player SpawnPlayer(Tile tile)
        {
            GameObject go = Instantiate(Player, tile.transform.position, Quaternion.identity);
            Player player = go.GetComponent<Player>();
            player.OnTile = tile;
            FindObjectOfType<PlayerController>().Player = player;

            return player;
        }

        private void Shuffle(IList<Tile> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = RandomGenerator.Instance.RandomInt(n + 1);
                Tile value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private class TileParams
        {
            public int x;
            public int y;
            public RoomParams roomParams;
            public float pnox;
            public float pnoy;
            public float pnstep;
            public float pntox;
            public float pntoy;

            public TileParams(int x, int y, RoomParams roomParams, float pnox, float pnoy, float pnstep, float pntox, float pntoy)
            {
                this.x = x;
                this.y = y;
                this.roomParams = roomParams;
                this.pnox = pnox;
                this.pnoy = pnoy;
                this.pnstep = pnstep;
                this.pntox = pntox;
                this.pntoy = pntoy;
            }
        }

        #endregion
    }
}
