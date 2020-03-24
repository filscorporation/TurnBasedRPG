using Assets.Scripts.CharactersManagement;
using System;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class SwordAttack : RangedSkill
    {
        public override string Name => nameof(SwordAttack);
        public override int Cost => 2;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;

        protected override string iconPath => "Icons/SwordAttackIcon";
        public override string Description => "sword_attack_skill_description";

        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.45F;

        protected override int range => 1;
        private float damage = 5;

        public override Skill Clone()
        {
            return new SwordAttack();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, damage));
        }
    }
}
