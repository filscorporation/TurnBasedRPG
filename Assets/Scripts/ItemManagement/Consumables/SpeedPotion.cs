using Assets.Scripts.SkillManagement;
using Assets.Scripts.SkillManagement.Skills.Potions;

namespace Assets.Scripts.ItemManagement.Consumables
{
    /// <summary>
    /// Potion giving player action points
    /// </summary>
    public class SpeedPotion : Consumable
    {
        public override string Name => nameof(FlamePotion);
        protected override string iconPath => "Icons/SpeedPotionIcon";
        public override string Description => "speed_potion_description";

        private Skill effect;
        public override Skill UsageEffect => effect;

        public SpeedPotion() : base()
        {
            effect = Skill.SkillDictionary[nameof(SpeedPotionEffect)];
            effect.SourceConsumable = this;
        }
    }
}
