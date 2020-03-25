using Assets.Scripts.CharactersManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement;
using System;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills.Potions
{
    public class SpeedPotionEffect : RangedSkill
    {
        public override string Name => nameof(SpeedPotionEffect);
        public override int Cost => 0;
        public override SkillTargetType TargetType => SkillTargetType.Player;

        public override CharacterState CharacterTargetState => CharacterState.Consuming;

        protected override string iconPath => "Icons/SpeedPotionIcon";
        public override string Description => null;

        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.5F;

        protected override int range => 0;

        public override Skill Clone()
        {
            return new SpeedPotionEffect();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1 || !(target.CharacterTargets.First() is Player player))
                throw new NotSupportedException();
            player.ActionPoints += 2;
            UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPoints);
        }
    }
}
