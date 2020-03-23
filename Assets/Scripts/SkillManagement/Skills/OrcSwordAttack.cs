using Assets.Scripts.CharactersManagement;
using System;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class OrcSwordAttack : RangedSkill
    {
        public override string Name => nameof(OrcSwordAttack);
        public override int Cost => 2;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;

        protected override string iconPath => null;
        public override string Description => null;

        private float damage = 3;

        public override Skill Clone()
        {
            return new OrcSwordAttack();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, damage));
        }
    }
}
