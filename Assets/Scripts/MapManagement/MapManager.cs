using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Stores and controles all tiles of the map
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;
        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<MapManager>();
                return instance;
            }
        }

        private PathFinder<Tile> pathFinder;

        private Tile[,] field;

        public GameObject selectedTilePrefab;
        private List<GameObject> selectedTiles = new List<GameObject>();

        public Transform TilesParent;
        public Transform MapObjectsParent;

        public void Start()
        {
            pathFinder = new PathFinder<Tile>();
            //InitializeField();
        }

        private void InitializeField()
        {
            Tile[] tiles = GameObject.FindObjectsOfType<Tile>();
            //TODO: support not square field
            int x = (int) Mathf.Sqrt(tiles.Length);
            int y = x;
            field = new Tile[x, y];
            foreach (Tile tile in tiles)
            {
                tile.DetectXY();
                field[tile.X, tile.Y] = tile;
            }

            Debug.Log($"Initialized field {x}:{y}");
        }

        public void SetField(Tile[,] newField)
        {
            if (field != null)
                throw new NotImplementedException();

            field = newField;
        }

        /// <summary>
        /// Return path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Tile> GetPath(Tile a, Tile b)
        {
            List<Tile> path = pathFinder.FindPath(a, b, field, true);
            // If path end tile was not free, we should remove it from final path
            // This is made for enemies to find path to occupied by player tile
            if (path != null && path.Any() && !b.Free)
                path.Remove(b);
            return path;
        }

        /// <summary>
        /// Draw and return path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Tile> BuildPath(Tile a, Tile b)
        {
            List<Tile> path = pathFinder.FindPath(a, b, field, true);
            // If path end tile was not free, we should remove it from final path
            // Used to move to intaractable objects
            if (path != null && path.Any() && !b.Free)
                path.Remove(b);
            SelectPath(path);
            return path;
        }

        /// <summary>
        /// Clear drawn path
        /// </summary>
        public void ClearPath()
        {
            foreach (GameObject selectedTile in selectedTiles)
            {
                Destroy(selectedTile);
            }
            selectedTiles.Clear();
        }

        /// <summary>
        /// Clear drawn path element
        /// </summary>
        public void ClearPath(int index)
        {
            // TODO: solve bug with double click on next to player tile while moving
            if (selectedTiles.Count >= index)
                Destroy(selectedTiles[index]);
        }

        private void SelectPath(List<Tile> path)
        {
            if (selectedTiles != null && selectedTiles.Any())
                ClearPath();
            if (selectedTiles == null)
                selectedTiles = new List<GameObject>();

            foreach (Tile tile in path)
            {
                GameObject sGO = Instantiate(selectedTilePrefab, tile.transform.position, Quaternion.identity);
                sGO.transform.SetParent(tile.transform);
                selectedTiles.Add(sGO);
            }
        }

        /// <summary>
        /// Returns all existing neighbours for the tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public IEnumerable<Tile> GetNeighbours(Tile tile, int radius = 1)
        {
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if ((i != 0 || j != 0)
                        && tile.X + i >= 0 && tile.X + i < field.GetLength(0)
                        && tile.Y + j >= 0 && tile.Y + j < field.GetLength(1))
                        yield return field[tile.X + i,tile.Y + j];
                }
            }
        }
    }
}
