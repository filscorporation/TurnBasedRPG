using Assets.Scripts.CharactersManagement;
using System;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class HeavySwordAttack : RangedSkill
    {
        // TODO: temporary skill
        public override string Name => nameof(HeavySwordAttack);
        public override int Cost => 3;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;

        protected override string iconPath => "Icons/HeavySwordAttackIcon";
        public override string Description => "heavy_sword_attack_skill_description";

        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.45F;

        protected override int range => 1;
        private float damage = 10;

        public override Skill Clone()
        {
            return new HeavySwordAttack();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, damage));
        }
    }
}
