using System.Linq;
using Assets.Scripts.BattleManagement;
using Assets.Scripts.EventManagement;
using Assets.Scripts.SkillManagement.Skills;

namespace Assets.Scripts.ItemManagement.Items
{
    public class Hilt : Item
    {
        public override string Name => nameof(Hilt);
        protected override string iconPath => "Icons/Hilt";
        public override string Description => "hilt_item_description";
        public override int Level => 3;

        public override void OnBattleStart(Battle battle, CancellationToken token)
        {
            Whirlwind skill;
            if ((skill = battle.Player.Skills.FirstOrDefault(s => s.Name == nameof(Whirlwind)) as Whirlwind) != null)
            {
                skill.WhirlwindRange ++;
            }
        }

        public override void OnBattleEnd(Battle battle, CancellationToken token)
        {
            Whirlwind skill;
            if ((skill = battle.Player.Skills.FirstOrDefault(s => s.Name == nameof(Whirlwind)) as Whirlwind) != null)
            {
                skill.WhirlwindRange --;
            }
        }
    }
}
