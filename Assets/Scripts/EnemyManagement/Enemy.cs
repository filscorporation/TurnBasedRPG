using Assets.Scripts.CharactersManagement;
using UnityEngine;

namespace Assets.Scripts.EnemyManagement
{
    /// <summary>
    /// Represents enemy in the game with his stats and interactions
    /// </summary>
    public class Enemy : Character
    {
        public int LineOfSight = 3;

        public void Start()
        {
            DetectOnTile();
        }

        /// <summary>
        /// Checks if other character in sight of this
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool InSight(Character c)
        {
            return Mathf.Max(
                       Mathf.Abs(c.OnTile.X - OnTile.X),
                       Mathf.Abs(c.OnTile.Y - OnTile.Y))
                   <= LineOfSight;
        }
    }
}
