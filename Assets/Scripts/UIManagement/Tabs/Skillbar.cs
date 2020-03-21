using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.PlayerManagement;
using Assets.Scripts.UIManagement.UIElements;
using UnityEngine;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Players current active skills to use in UI
    /// </summary>
    public class Skillbar : MonoBehaviour
    {
        public GameObject SkillButtonPrefab;
        private SkillController skillController;

        private const float buttonDistance = 10F;
        private const float xOffset = 30F;
        private const float yOffset = 25F;

        private List<SkillButton> buttons;

        public void Initialize(List<Skill> skills, SkillController controller)
        {
            skillController = controller;
            buttons = new List<SkillButton>();
            int i = 0;
            float contentSize = 0;
            foreach (Skill skill in skills)
            {
                GameObject go = Instantiate(SkillButtonPrefab);
                go.transform.SetParent(transform);
                SkillButton button = go.GetComponent<SkillButton>();
                go.GetComponent<RectTransform>().localPosition = new Vector2(i * (button.Width + buttonDistance) + xOffset, yOffset);
                go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
                button.Initialize(skill);
                buttons.Add(button);
                contentSize += button.Width;
                contentSize += buttonDistance;
                i++;
            }
            // Last button dont have offset to next one (there is no next one)
            contentSize -= buttonDistance;

            SetContentSize(contentSize);
        }

        private void SetContentSize(float size)
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(size + xOffset*2, rt.sizeDelta.y);
        }

        public void Clear()
        {
            foreach (SkillButton button in buttons)
            {
                Destroy(button.gameObject);
            }
            buttons.Clear();
            SetContentSize(0);
        }

        public void Insert(Skill skill, int index = 0)
        {
            throw new NotImplementedException();
        }

        public void Remove(Skill skill)
        {
            throw new NotImplementedException();
        }
    }
}
