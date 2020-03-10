using System;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// Any object that sits on map and occupies tiles
    /// </summary>
    public class MapObject : MonoBehaviour
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

        private const string MapLayer = "Map";

        public void Start()
        {
            DetectOnTile();
        }

        private void DetectOnTile()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask(MapLayer));
            if (hit == null)
                throw new Exception("Cannot detect on tile for character");

            OnTile = hit.gameObject.GetComponent<Tile>();
        }
    }
}
