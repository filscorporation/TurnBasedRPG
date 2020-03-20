using System.Collections.Generic;
using Assets.Scripts.PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Represents list of skills in UI
    /// </summary>
    public class SkillsTab : MonoBehaviour
    {
        public Text HealthText;
        public Text LevelText;
        public Text ExperienceText;
        public Text SkillPointsText;
        public Transform ScrollViewContent;
        public GameObject SkillLinePrefab;

        private List<SkillLine> lines;

        public void Initialize(List<Skill> skills)
        {
            lines = new List<SkillLine>();
            int i = 0;
            float contentSize = 0;
            foreach (Skill skill in skills)
            {
                GameObject go = Instantiate(SkillLinePrefab);
                go.transform.SetParent(ScrollViewContent);
                SkillLine line = go.GetComponent<SkillLine>();
                go.GetComponent<RectTransform>().localPosition = new Vector2(0, -i * line.Height);
                go.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
                line.Initialize(skill);
                lines.Add(line);
                contentSize += line.Height;
                i++;
            }

            contentSize += 600F; // TODO: remove, for demonstration purposes
            RectTransform rt = ScrollViewContent.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, contentSize);
        }

        public void Clear()
        {
            foreach (SkillLine skillLine in lines)
            {
                Destroy(skillLine.gameObject);
            }
            lines.Clear();
        }
    }
}
