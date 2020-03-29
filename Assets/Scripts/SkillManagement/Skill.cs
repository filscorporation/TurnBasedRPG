using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.ItemManagement;
using Assets.Scripts.MapManagement;
using Assets.Scripts.UIManagement.Tabs;
using UnityEngine;

namespace Assets.Scripts.SkillManagement
{
    /// <summary>
    /// Тип цели для умения
    /// </summary>
    public enum SkillTargetType
    {
        Enemy,
        Player,
        Tile,
    }

    /// <summary>
    /// Base players skill
    /// </summary>
    [Serializable]
    public abstract class Skill : IInventoryObject
    {
        public static Dictionary<string, Skill> SkillDictionary = new Dictionary<string, Skill>();

        public abstract string Name { get; }
        public int Level;
        public abstract int Cost { get; }
        public abstract SkillTargetType TargetType { get; }

        public abstract float CastingTime { get; }
        public abstract float CastingEffectTime { get; }
        public virtual CharacterState CharacterTargetState { get => CharacterState.Attacking; }

        protected abstract string iconPath { get; }
        public Sprite Icon { get; set; }
        public abstract string Description { get; }

        public Consumable SourceConsumable { get; set; } = null;

        public Skill()
        {
            LoadResources();
        }

        /// <summary>
        /// Loads skills icon and prefabs
        /// </summary>
        public virtual void LoadResources()
        {
            if (!string.IsNullOrWhiteSpace(iconPath))
                Icon = Resources.Load<Sprite>(iconPath);
        }

        /// <summary>
        /// Returns new instance of a skill
        /// </summary>
        /// <returns></returns>
        public abstract Skill Clone();

        /// <summary>
        /// Checks if character in range of the skill
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool InRange(Character user, Character target);

        /// <summary>
        /// Checks if tile in range of the skill
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        public abstract bool InRange(Character user, Tile tile);

        /// <summary>
        /// Uses skill on targets
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        public abstract void Use(Character user, SkillTarget target);

        /// <summary>
        /// Instantiates skill casting effect on its users position
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="user"></param>
        protected void InstantiateEffect(GameObject effect, Character user)
        {
            UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(
                effect,
                user.transform.position,
                effect.transform.rotation),
                3F);
        }

        public override string ToString() => Name;
    }
}
