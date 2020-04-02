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
        protected abstract int Range { get; }

        public override void HighlightTargetTiles(Tile userOnTile)
        {
            if (TargetType == SkillTargetType.User)
                MapManager.Instance.SelectTargets(new []{ userOnTile });
            else
                MapManager.Instance.SelectTargets(userOnTile, Range);
        }

        public override void ClearHighlighted()
        {
            MapManager.Instance.ClearTargets();
        }

        public override bool InRange(Character user, Character target)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - target.OnTile.X),
                       Mathf.Abs(user.OnTile.Y - target.OnTile.Y))
                   <= Range;
        }

        public override bool InRange(Character user, Tile tile)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - tile.X),
                       Mathf.Abs(user.OnTile.Y - tile.Y))
                   <= Range;
        }
    }
}
