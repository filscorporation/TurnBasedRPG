﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Stores and controls all tiles of the map
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
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

        public Tile[,] Field;

        public GameObject SelectedTilePrefab;
        private List<GameObject> selectedTiles = new List<GameObject>();

        public GameObject TargetTilePrefab;
        private List<GameObject> targetTiles = new List<GameObject>();

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
            Field = new Tile[x, y];
            foreach (Tile tile in tiles)
            {
                tile.DetectXY();
                Field[tile.X, tile.Y] = tile;
            }

            Debug.Log($"Initialized field {x}:{y}");
        }

        public void SetField(Tile[,] newField)
        {
            if (Field != null)
                throw new NotImplementedException();

            Field = newField;
        }

        /// <summary>
        /// Return path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Tile> GetPath(Tile a, Tile b)
        {
            List<Tile> path = pathFinder.FindPath(a, b, Field, true);
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
            List<Tile> path = pathFinder.FindPath(a, b, Field, true);
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
        /// Highlight tiles around with target icon
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void SelectTargets(Tile center, int radius)
        {
            if (targetTiles != null && targetTiles.Any())
                ClearTargets();
            if (targetTiles == null)
                targetTiles = new List<GameObject>();

            foreach (Tile tile in GetNeighbours(center, radius))
            {
                GameObject tGO = Instantiate(TargetTilePrefab, tile.transform.position, Quaternion.identity);
                tGO.transform.SetParent(tile.transform);
                targetTiles.Add(tGO);
            }
        }

        /// <summary>
        /// Highlight tiles with target icon
        /// </summary>
        /// <param name="tiles"></param>
        public void SelectTargets(IEnumerable<Tile> tiles)
        {
            if (targetTiles != null && targetTiles.Any())
                ClearTargets();
            if (targetTiles == null)
                targetTiles = new List<GameObject>();

            foreach (Tile tile in tiles)
            {
                GameObject tGO = Instantiate(TargetTilePrefab, tile.transform.position, Quaternion.identity);
                tGO.transform.SetParent(tile.transform);
                targetTiles.Add(tGO);
            }
        }

        /// <summary>
        /// Clear drawn target tiles
        /// </summary>
        public void ClearTargets()
        {
            foreach (GameObject targetTile in targetTiles)
            {
                Destroy(targetTile);
            }
            targetTiles.Clear();
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
                GameObject sGO = Instantiate(SelectedTilePrefab, tile.transform.position, Quaternion.identity);
                sGO.transform.SetParent(tile.transform);
                selectedTiles.Add(sGO);
            }
        }

        /// <summary>
        /// Returns all existing neighbours for the tile
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public IEnumerable<Tile> GetNeighbours(Tile tile, int radius = 1)
        {
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if ((i != 0 || j != 0)
                        && tile.X + i >= 0 && tile.X + i < Field.GetLength(0)
                        && tile.Y + j >= 0 && tile.Y + j < Field.GetLength(1))
                        yield return Field[tile.X + i,tile.Y + j];
                }
            }
        }
    }
}
