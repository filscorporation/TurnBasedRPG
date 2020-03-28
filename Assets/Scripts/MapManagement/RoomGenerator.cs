using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.RewardManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates room: tiles, decorations, enemies, player, objects from parameters
/// </summary>
namespace Assets.Scripts.MapManagement
{
    public class RoomGenerator : MonoBehaviour
    {
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
            var minimums = GetLocalMinimums(field, roomParams.Width, roomParams.Height, pntox, pntoy, pnstep);
            foreach (Tile tile in minimums)
            {
                if (tile.Free)
                {
                    AddEnemyGroup(tile, roomParams);
                }
            }

            // Places doors (entrance) in different room sides
            foreach (Direction door in roomParams.Doors)
            {
                SpawnDoor(field, door);
            }

            // Spawns player
            SpawnPlayer();
        }
        
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

            // Tile details
            float s = RandomGenerator.Instance.RandomFloat(0.6F, 0.75F);
            float nv = Mathf.PerlinNoise(pnx, pny);
            if (nv > s)
            {
                int dirtIndex = RandomGenerator.Instance.RandomInt(DirtTiles.Count);
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

        /// <summary>
        /// Gets points of local minimums of perlin noise as an options to spawn enemies
        /// </summary>
        /// <param name="field"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="pnox"></param>
        /// <param name="pnoy"></param>
        /// <param name="pnstep"></param>
        /// <returns></returns>
        private IEnumerable<Tile> GetLocalMinimums(Tile[,] field, int w, int h, float pnox, float pnoy, float pnstep)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = pnox + i * pnstep;
                    float y = pnoy + j * pnstep;
                    float v = Mathf.PerlinNoise(x, y);
                    // Get noise value for all tiles around
                    // If there is edge of the map, use 1 as maximum
                    float l = i - 1 < 0  ? 1 : Mathf.PerlinNoise(x - pnstep, y);
                    float r = i + 1 == w ? 1 : Mathf.PerlinNoise(x + pnstep, y);
                    float u = j - 1 < 0 ? 1 : Mathf.PerlinNoise(x, y - pnstep);
                    float d = j + 1 == h ? 1 : Mathf.PerlinNoise(x, y + pnstep);

                    if (v < l && v < r && v < u && v < d)
                    {
                        // Value at this point of the noise map is lower than around
                        // so its local minimum
                        yield return field[i, j];
                    }
                }
            }
        }

        private void AddEnemyGroup(Tile tile, RoomParams roomParams)
        {
            int enemyIndex = RandomGenerator.Instance.RandomInt(Enemies.Count);
            Enemy enemy = Enemies[enemyIndex].GetComponent<Enemy>();
            int enemyCount = RandomGenerator.Instance.RandomInt(enemy.GroupMinSize, enemy.GroupMaxSize);
            
            if (enemyCount == 1)
            {
                SpawnEnemy(tile, Enemies[enemyIndex]);
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
                    SpawnEnemy(n, Enemies[enemyIndex]);
                    enemyCount--;
                    if (enemyCount == 0)
                        break;
                }
            }
        }

        private void SpawnEnemy(Tile tile, GameObject enemy)
        {
            GameObject go = Instantiate(enemy, tile.transform.position, Quaternion.identity);
            go.GetComponent<Enemy>().OnTile = tile;
        }

        private void SpawnChest(Tile tile)
        {
            GameObject go = Instantiate(Chest, tile.transform.position, Quaternion.identity);
            tile.Free = false;
        }

        private void SpawnDoor(Tile[,] field, Direction direction)
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
            GameObject go = Instantiate(Door, tile.transform.position, Quaternion.identity);
            go.GetComponent<Entrance>().OnTile = tile;
        }

        private void SpawnPlayer()
        {
            foreach(Tile tile in FindObjectsOfType<Entrance>().Select(d => d.OnTile))
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

        private void SpawnPlayer(Tile tile)
        {
            GameObject go = Instantiate(Player, tile.transform.position, Quaternion.identity);
            Player player = go.GetComponent<Player>();
            player.OnTile = tile;
            FindObjectOfType<PlayerController>().Player = player;
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
    }
}
