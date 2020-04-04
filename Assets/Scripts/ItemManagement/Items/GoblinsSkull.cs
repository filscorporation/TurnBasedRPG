using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement;

namespace Assets.Scripts.ItemManagement.Items
{
    public class GoblinsSkull : Item
    {
        public override string Name => nameof(GoblinsSkull);
        protected override string iconPath => "Icons/Skull";
        public override string Description => "goblins_skull_item_description";
        public override int Level => 2;

        public override void OnAfterCharacterDead(Character character, Character killer, CancellationToken token)
        {
            if (character is Enemy)
            {
                killer.ActionPoints += 2;
                if (killer is Player player && player.PlayerState == PlayerState.InBattle)
                {
                    UIManager.Instance.SetVariable(nameof(player.ActionPoints), player.ActionPoints);
                }
            }
        }
    }
}
