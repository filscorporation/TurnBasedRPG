using System;
using System.Linq;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.SkillManagement.Effects;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class Cut : RangedSkill
    {
        public override string Name => nameof(Cut);
        public override int Cost => 2;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;
        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.45F;
        protected override string iconPath => "Icons/Cut";
        public override string Description => "cut_skill_description";
        protected override int Range => 1;

        public float Damage = 3F;
        public int Duration = 4;

        public override Skill Clone()
        {
            return new Cut();
        }

        public override void Use(Character user, SkillTarget target)
        {
            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, 0F));
            target.CharacterTargets.First().AddEffect(new Bleed(Damage, Duration), user);
        }
    }
}
