using System;
using Assets.Scripts.PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.UIElements
{
    /// <summary>
    /// Button of skill on skillbar
    /// </summary>
    public class SkillButton : MonoBehaviour
    {
        public float Width = 120F;
        public Image Icon;
        public Skill Skill;

        public void Initialize(Skill skill)
        {
            Skill = skill;
            Icon.sprite = skill.Icon;
        }
    }
}
