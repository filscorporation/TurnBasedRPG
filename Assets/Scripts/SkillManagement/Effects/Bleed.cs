using Assets.Scripts.CharactersManagement;
using UnityEngine;

namespace Assets.Scripts.SkillManagement.Effects
{
    public class Bleed : Effect
    {
        protected override GameObject OnApplyEffect => null;
        protected override Sprite Icon => null;

        private Character effectCaster;

        public float DamagePerTic;
        public int Duration;

        public Bleed(float damagePerTic, int duration)
        {
            DamagePerTic = damagePerTic;
            Duration = duration;
        }

        public override void EffectOnApply(Character target, Character caster)
        {
            effectCaster = caster;
        }

        public override void EffectOnCharacterTurnStart(Character target)
        {
            target.TakeDamage(new Damage(effectCaster, DamagePerTic), false);
            Duration--;
            if (Duration == 0)
            {
                target.RemoveEffect(this);
            }
        }
    }
}
