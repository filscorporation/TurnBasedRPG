﻿using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Map consists of this base tiles
    /// </summary>
    public class Tile : MonoBehaviour, IMapElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Row { get; set; }
        public bool Free { get; set; } = true;
        public int List { get; set; }
        public int FValue { get; set; }
        public int GValue { get; set; }
        public int HValue { get; set; }
        public int ParentX { get; set; }
        public int ParentZ { get; set; }

        public MapObject Occupier;
        
        /// <summary>
        /// Initialize X Y coordinates based on position
        /// </summary>
        public void DetectXY()
        {
            X = (int)transform.localPosition.x;
            Y = (int)transform.localPosition.y;
        }
        
        public override bool Equals(object o)
        {
            if (o is Tile tile)
                return tile.X == X && tile.Y == Y;

            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}