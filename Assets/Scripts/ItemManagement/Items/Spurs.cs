using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.PlayerManagement;

namespace Assets.Scripts.ItemManagement.Items
{
    public class Spurs : Item
    {
        public override string Name => nameof(Spurs);
        protected override string iconPath => "Icons/Spurs";
        public override string Description => "spurs_item_description";
        public override int Level => 3;

        public override void OnBeforeCharacterTakesDamage(Character character, Damage damage, CancellationToken token)
        {
            if (damage.Source is Player player)
            {
                damage.Value += player.TilesPassedInCurrentTurn;
            }
        }
    }
}
