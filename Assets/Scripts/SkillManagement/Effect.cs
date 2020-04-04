using Assets.Scripts.CharactersManagement;
using UnityEngine;

namespace Assets.Scripts.SkillManagement
{
    /// <summary>
    /// Effects that can be applied to characters
    /// </summary>
    public abstract class Effect
    {
        /// <summary>
        /// Visual effect to instantiate on target when applied
        /// </summary>
        protected abstract GameObject OnApplyEffect { get; }

        /// <summary>
        /// Icon of this effect shown near healthbar
        /// </summary>
        protected abstract Sprite Icon { get; }

        /// <summary>
        /// Called when effect is applied
        /// </summary>
        /// <param name="target">Target of effect</param>
        /// <param name="caster">Who applied effect to the target</param>
        public abstract void EffectOnApply(Character target, Character caster);

        /// <summary>
        /// Called when effect target turn is starting
        /// </summary>
        /// <param name="target"></param>
        public abstract void EffectOnCharacterTurnStart(Character target);
    }
}
