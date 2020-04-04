using Assets.Scripts.BattleManagement;
using Assets.Scripts.EventManagement;

namespace Assets.Scripts.ItemManagement.Items
{
    public class MightyBelt : Item
    {
        public override string Name => nameof(MightyBelt);
        protected override string iconPath => "Icons/MightyBelt";
        public override string Description => "mighty_belt_item_description";
        public override int Level => 3;

        public override void OnAfterPlayersTurnBegin(Battle battle, CancellationToken token)
        {
            battle.Player.GainBlock(3);
        }
    }
}
