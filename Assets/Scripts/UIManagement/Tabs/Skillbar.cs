using System;
using System.Collections.Generic;
using Assets.Scripts.SkillManagement;
using UnityEngine;
using UnityEngine.UI;

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

        public void Initialize(IEnumerable<Skill> skills, SkillController controller)
        {
            skillController = controller;

            Rebuild(skills);
        }

        /// <summary>
        /// Populates skillbar with skill buttons
        /// </summary>
        /// <param name="skills"></param>
        public void Rebuild(IEnumerable<Skill> skills)
        {
            if (buttons != null)
            {
                foreach(SkillButton button in buttons)
                {
                    Destroy(button.gameObject);
                }
                buttons.Clear();
            }
            else
            {
                buttons = new List<SkillButton>();
            }

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
                button.OnClick += HandleSkillButtonClick;
                buttons.Add(button);
                contentSize += button.Width;
                contentSize += buttonDistance;
                i++;
            }
            // Last button dont have offset to next one (there is no next one)
            contentSize -= buttonDistance;

            SetContentSize(contentSize);
        }

        private void HandleSkillButtonClick(object sender, EventArgs args)
        {
            if (sender is SkillButton button)
                skillController.HandleSkillClick(button.Skill);
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

        /// <summary>
        /// Activates all skills on skillbar
        /// </summary>
        public void Show()
        {
            foreach (SkillButton button in buttons)
            {
                button.gameObject.GetComponent<Button>().interactable = true;
            }
        }

        /// <summary>
        /// Deactivates all skills on skillbar
        /// </summary>
        public void Hide()
        {
            foreach (SkillButton button in buttons)
            {
                button.gameObject.GetComponent<Button>().interactable = false;
            }
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
