using Assets.Scripts.CharactersManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
{
    /// <summary>
    /// Тип цели для умения
    /// </summary>
    public enum SkillTargetType
    {
        Enemy,
        Player,
        Tile,
    }

    /// <summary>
    /// Players skill stats
    /// </summary>
    public class Skill
    {
        public string Name;

        public int Range;

        public int Cost;

        public SkillTargetType TargetType;

        public float Damage;

        /// <summary>
        /// Checks if character in range of the skill
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool InRange(Character user, Character target)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - target.OnTile.X),
                       Mathf.Abs(user.OnTile.Y - target.OnTile.Y))
                   <= Range;
        }
    }
}
