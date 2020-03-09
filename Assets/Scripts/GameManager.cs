using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Controles all game processes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public void Start()
        {

        }
        
        //private void GenerateField(int x, int y)
        //{
        //    for (int i = 0; i < x; i++)
        //    {
        //        for (int j = 0; j < y; j++)
        //        {
        //            GameObject go = Instantiate(TileGO, new Vector3(i - x/2, j - y/2), Quaternion.identity, Map.transform);
        //            Tile tile = go.GetComponent<Tile>();
        //            tile.X = i;
        //            tile.Y = j;
        //        }
        //    }
        //}
    }
}
