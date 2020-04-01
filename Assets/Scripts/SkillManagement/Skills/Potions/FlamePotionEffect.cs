using Assets.Scripts.CharactersManagement;
using Assets.Scripts.ItemManagement;
using System;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills.Potions
{
    public class FlamePotionEffect : RangedSkill
    {
        public override string Name => nameof(FlamePotionEffect);
        public override int Cost => 0;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;

        protected override string iconPath => "Icons/FlamePotionIcon";
        public override string Description => null;

        public override CharacterState CharacterTargetState => CharacterState.Throwing;

        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.8F;

        protected override int range => 1;
        private float damage = 5;

        public override Skill Clone()
        {
            return new FlamePotionEffect();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, damage));
        }
    }
}
