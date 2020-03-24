using Assets.Scripts.CharactersManagement;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class GolemsSmash : RangedSkill
    {
        public override string Name => nameof(GolemsSmash);
        public override int Cost => 2;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;

        protected override string iconPath => null;
        public override string Description => null;

        protected override int range => 2;
        private float damage = 7;
        
        public override float CastingTime => 2F;
        public override float CastingEffectTime => 0.9F;

        private string onHitEffectPath = "Prefabs/StoneParticlesEffect";
        public GameObject OnHitEffect;

        public override void LoadResources()
        {
            base.LoadResources();

            OnHitEffect = Resources.Load<GameObject>(onHitEffectPath);
        }

        public override void Use(Character user, SkillTarget target)
        {
            InstantiateEffect(OnHitEffect, user);

            if (target.CharacterTargets.Count != 1)
                throw new NotSupportedException();
            target.CharacterTargets.First().TakeDamage(new Damage(user, damage));
        }

        public override Skill Clone()
        {
            return new GolemsSmash();
        }
    }
}
