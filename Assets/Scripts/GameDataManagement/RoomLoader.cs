using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Generates room: tiles, decorations, enemies, player, objects from loaded data
    /// </summary>
    public class RoomLoader : RoomBuilderBase
    {
        private static RoomLoader instance;
        public static RoomLoader Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<RoomLoader>();
                return instance;
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
                SpawnDoor(field[entranceData.OnTileX, entranceData.OnTileY], (Direction)entranceData.Direction,
                    entranceData.RoomToIndex);
            }
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
    }
}
