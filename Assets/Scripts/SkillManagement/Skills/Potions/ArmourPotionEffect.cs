using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;

namespace Assets.Scripts.SkillManagement.Skills.Potions
{
    public class ArmourPotionEffect : Skill
    {
        public override string Name => nameof(ArmourPotionEffect);
        public override int Cost => 0;
        public override SkillTargetType TargetType => SkillTargetType.User;
        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.5F;
        public override CharacterState CharacterTargetState => CharacterState.Consuming;
        protected override string iconPath => "Icons/ArmourPotionIcon";
        public override string Description => null;

        public override Skill Clone()
        {
            return new ArmourPotionEffect();
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
