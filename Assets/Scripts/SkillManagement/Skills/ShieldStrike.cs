using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CharactersManagement;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class ShieldStrike : RangedSkill
    {
        public override string Name => nameof(ShieldStrike);
        public override int Cost => 1;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;
        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.45F;
        protected override string iconPath => "Icons/ShieldAttackIcon";
        public override string Description => "shield_strike_skill_description";
        protected override int Range => 1;
        public override Skill Clone()
        {
            return new ShieldStrike();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, user.Block));
        }
    }
}
