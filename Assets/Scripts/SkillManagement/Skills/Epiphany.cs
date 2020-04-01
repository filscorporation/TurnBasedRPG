using Assets.Scripts.BattleManagement;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.MapManagement;
using System.Linq;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class Epiphany : Skill
    {
        public override string Name => nameof(Epiphany);

        public override int Cost => 0;

        public override SkillTargetType TargetType => SkillTargetType.Player;

        public override CharacterState CharacterTargetState => CharacterState.Casting;

        public override float CastingTime => 1F;

        public override float CastingEffectTime => 0.6F;

        public override string Description => "epiphany_skill_description";

        protected override string iconPath => "Icons/EpiphanyIcon";

        public override Skill Clone()
        {
            return new Epiphany();
        }

        public override void HighlightTargetTiles(Tile userOnTile)
        {
            MapManager.Instance.SelectTargets(BattleManager.Instance.CurrentBattle.Enemies
                .Where(e => e.State != CharacterState.Dead)
                .Select(e => e.OnTile));
        }

        public override void ClearHighlighted()
        {
            MapManager.Instance.ClearTargets();
        }

        public override bool InRange(Character user, Character target)
        {
            return true;
        }

        public override bool InRange(Character user, Tile tile)
        {
            return true;
        }

        public override void Use(Character user, SkillTarget target)
        {
            foreach (Enemy enemy in BattleManager.Instance.CurrentBattle.Enemies
                .Where(e => e.State != CharacterState.Dead))
            {
                enemy.TakeDamage(new Damage(user, 999F));
            }
        }
    }
}
