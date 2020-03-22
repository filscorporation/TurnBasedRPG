using UnityEngine;

namespace Assets.Scripts.ItemManagement.Consumables
{
    /// <summary>
    /// Damage dealing potion
    /// </summary>
    public class FlamePotion : Consumable
    {
        public override string Name => nameof(FlamePotion);
        // TODO: temp solution to set from inspector, lately will be changed with item serialization
        [SerializeField]
        private Sprite icon;
        public override Sprite Icon => icon;
        public override string Description => "flame_potion_description";
    }
}
