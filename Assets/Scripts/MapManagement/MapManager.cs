﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Stores and controles all tiles of the map
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        private PathFinder<Tile> pathFinder;

        private Tile[,] field;

        public GameObject selectedTilePrefab;
        private List<GameObject> selectedTiles;

        public void Start()
        {
            pathFinder = new PathFinder<Tile>();
            InitializeField();
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

        /// <summary>
        /// Return path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Tile> GetPath(Tile a, Tile b)
        {
            return pathFinder.FindPath(a, b, field);
        }

        /// <summary>
        /// Draw and return path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Tile> BuildPath(Tile a, Tile b)
        {
            List<Tile> path = pathFinder.FindPath(a, b, field);
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
    }
}
