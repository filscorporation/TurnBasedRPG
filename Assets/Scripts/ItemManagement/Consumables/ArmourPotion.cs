using Assets.Scripts.SkillManagement;
using Assets.Scripts.SkillManagement.Skills.Potions;

namespace Assets.Scripts.ItemManagement.Consumables
{
    public class ArmourPotion : Consumable
    {
        public override string Name => nameof(ArmourPotion);
        protected override string iconPath => "Icons/ArmourPotionIcon";
        public override string Description => "armour_potion_description";

        private Skill effect;
        public override Skill UsageEffect => effect;

        public ArmourPotion() : base()
        {
            effect = Skill.SkillDictionary[nameof(ArmourPotionEffect)];
            effect.SourceConsumable = this;
        }
    }
}
