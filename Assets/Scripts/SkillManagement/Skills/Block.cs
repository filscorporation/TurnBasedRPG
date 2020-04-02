using System;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class Block : Skill
    {
        public override string Name => nameof(Block);
        public override int Cost => 1;
        public override SkillTargetType TargetType => SkillTargetType.User;
        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.6F;
        public override CharacterState CharacterTargetState => CharacterState.Casting;
        protected override string iconPath => "Icons/ShieldIcon";
        public override string Description => "block_skill_description";
        public override Skill Clone()
        {
            return new Block();
        }

        public override void HighlightTargetTiles(Tile userOnTile)
        {
            MapManager.Instance.SelectTargets(new []{ userOnTile });
        }

        public override void ClearHighlighted()
        {
            MapManager.Instance.ClearTargets();
        }

        public override bool InRange(Character user, Character target)
        {
            return true;
        }

        public override bool InRange(Character user, Tile tile)
        {
            return true;
        }

        public override void Use(Character user, SkillTarget target)
        {
            user.GainBlock(5);
        }
    }
}
