using System.Linq;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.EnemyManagement;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.SkillManagement.Skills
{
    public class Whirlwind : RangedSkill
    {
        public override string Name => nameof(Whirlwind);
        public override int Cost => 3;
        public override SkillTargetType TargetType => SkillTargetType.Enemy;
        public override float CastingTime => 1F;
        public override float CastingEffectTime => 0.5F;
        protected override string iconPath => "Icons/WhirlwindIcon";
        public override string Description => "whirlwind_skill_description";
        public int WhirlwindRange = 1;
        protected override int Range => WhirlwindRange;

        private string onHitEffectPath = "Prefabs/WhirlwindEffect";
        public GameObject OnHitEffect;

        public override void LoadResources()
        {
            base.LoadResources();

            OnHitEffect = Resources.Load<GameObject>(onHitEffectPath);
        }

        public override Skill Clone()
        {
            return new Whirlwind();
        }

        public override void Use(Character user, SkillTarget target)
        {
            InstantiateEffect(OnHitEffect, user);

            foreach (Enemy enemy in MapManager.Instance
                .GetNeighbours(user.OnTile, Range)
                .Select(t => t.Occupier)
                .Where(o => o is Enemy e && e.State != CharacterState.Dead)
                .Cast<Enemy>())
            {
                enemy.TakeDamage(new Damage(user, 4F));
            }
        }
    }
}
