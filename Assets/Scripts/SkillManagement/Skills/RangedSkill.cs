using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.SkillManagement.Skills
{
    /// <summary>
    /// Skill with a range
    /// </summary>
    public abstract class RangedSkill : Skill
    {
        protected abstract int range { get; }

        public override bool InRange(Character user, Character target)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - target.OnTile.X),
                       Mathf.Abs(user.OnTile.Y - target.OnTile.Y))
                   <= range;
        }

        public override bool InRange(Character user, Tile tile)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - tile.X),
                       Mathf.Abs(user.OnTile.Y - tile.Y))
                   <= range;
        }
    }
}
