﻿using Assets.Scripts.SkillManagement;
using Assets.Scripts.SkillManagement.Skills.Potions;

namespace Assets.Scripts.ItemManagement.Consumables
{
    /// <summary>
    /// Damage dealing potion
    /// </summary>
    public class FlamePotion : Consumable
    {
        public override string Name => nameof(FlamePotion);
        protected override string iconPath => "Icons/FlamePotionIcon";
        public override string Description => "flame_potion_description";

        private Skill effect;
        public override Skill UsageEffect => effect;

        public FlamePotion() : base()
        {
            effect = Skill.SkillDictionary[nameof(FlamePotionEffect)];
            effect.SourceConsumable = this;
        }
    }
}
