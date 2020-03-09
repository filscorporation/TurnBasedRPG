using System;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.CharactersManagement
{
    /// <summary>
    /// Player or enemy in the game
    /// </summary>
    public class Character : MonoBehaviour
    {
        private Tile onTile;
        public Tile OnTile
        {
            get => onTile;
            set
            {
                if (onTile != null)
                    onTile.Free = true;
                value.Free = false;
                onTile = value;
            }
        }

        public float MovingSpeed = 0.2F;

        private const string MapLayer = "Map";

        protected void DetectOnTile()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask(MapLayer));
            if (hit == null)
                throw new Exception("Cannot detect on tile for character");

            OnTile = hit.gameObject.GetComponent<Tile>();
        }
    }
}
