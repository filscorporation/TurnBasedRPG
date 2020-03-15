using System;
using System.Collections.Generic;
using Assets.Scripts.CharactersManagement;
using Assets.Scripts.MapManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerManagement
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
    /// Players skill stats
    /// </summary>
    [Serializable]
    public class Skill
    {
        public string Name;

        public int Range;

        public int Cost;

        public SkillTargetType TargetType;

        public float Damage;

        public float CastingTime = 1F;
        public float CastingEffectTime = 0.5F;

        public GameObject OnHitEffect;

        /// <summary>
        /// Checks if character in range of the skill
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool InRange(Character user, Character target)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - target.OnTile.X),
                       Mathf.Abs(user.OnTile.Y - target.OnTile.Y))
                   <= Range;
        }

        /// <summary>
        /// Checks if tile in range of the skill
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool InRange(Character user, Tile tile)
        {
            return Mathf.Max(
                       Mathf.Abs(user.OnTile.X - tile.X),
                       Mathf.Abs(user.OnTile.Y - tile.Y))
                   <= Range;
        }

        /// <summary>
        /// Uses skill on targets
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        public virtual void Use(Character user, SkillTarget target)
        {
            if (OnHitEffect != null)
            {
                GameObject.Destroy(GameObject.Instantiate(
                    OnHitEffect,
                    user.transform.position,
                    OnHitEffect.transform.rotation),
                    3F);
            }

            if (target.TileTarget != null)
            {
                // Skill used on the groud, probably AOE or some summoning
                // Will be used in skill overloads
                return;
            }

            foreach (Character character in target.CharacterTargets)
            {
                character.TakeDamage(new Damage(user, Damage));
            }
        }
    }
}
