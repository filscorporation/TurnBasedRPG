using Assets.Scripts.EnemyManagement;
using Assets.Scripts.InteractableObjects;
using Assets.Scripts.MapManagement;
using Assets.Scripts.RewardManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Base class for room loader and generator
    /// </summary>
    public abstract class RoomBuilderBase : MonoBehaviour
    {
        protected const string MapSortingLayer = "Map";
        protected const string EnemiesParentObjectName = "Enemies";

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

        /// <summary>
        /// Randomly shuffles tile list
        /// </summary>
        /// <param name="list"></param>
        protected void Shuffle(IList<Tile> list)
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

        /// <summary>
        /// Place the enemy on the tile
        /// </summary>
        /// <param name="tile">Tile</param>
        /// <param name="enemy">Enemy prefab</param>
        /// <param name="parent">Parent transform for all enemies</param>
        /// <returns></returns>
        protected Enemy SpawnEnemy(Tile tile, GameObject enemy, Transform parent)
        {
            GameObject go = Instantiate(enemy, tile.transform.position, Quaternion.identity, parent);
            go.GetComponent<Enemy>().OnTile = tile;

            return go.GetComponent<Enemy>();
        }

        /// <summary>
        /// Place chest on the tile
        /// </summary>
        /// <param name="tile">Tile</param>
        /// <returns></returns>
        protected Chest SpawnChest(Tile tile)
        {
            GameObject go = Instantiate(Chest, tile.transform.position, Quaternion.identity);
            tile.Free = false;

            return go.GetComponent<Chest>();
        }

        /// <summary>
        /// Place the entrance in certain field part
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="direction">Part of the field where to place the door</param>
        /// <param name="roomIndex">Room index where this door leads</param>
        protected void SpawnDoor(Tile[,] field, Direction direction, int roomIndex)
        {
            Tile tile;
            switch (direction)
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
            SpawnDoor(tile, direction, roomIndex);
        }

        /// <summary>
        /// Place entrance on the tile
        /// </summary>
        /// <param name="tile">Tile</param>
        /// <param name="direction">Direction of field where entrance is placed</param>
        /// <param name="roomIndex">Room index where this door leads</param>
        protected void SpawnDoor(Tile tile, Direction direction, int roomIndex)
        {
            GameObject go = Instantiate(Door, tile.transform.position, Quaternion.identity);
            Entrance entrance = go.GetComponent<Entrance>();
            entrance.OnTile = tile;
            entrance.RoomToIndex = roomIndex;
            entrance.Direction = direction;
        }
    }
}
