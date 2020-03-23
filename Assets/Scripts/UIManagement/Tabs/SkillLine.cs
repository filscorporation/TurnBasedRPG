using Assets.Scripts.SkillManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIManagement.Tabs
{
    /// <summary>
    /// Skill line in UI skills tab
    /// </summary>
    public class SkillLine : TabLine<Skill>
    {
        public Button UpgreatButton;
        public Button AcceptButton;
        public Button DeclineButton;

        public override void Initialize(Skill source)
        {
            base.Initialize(source);
        }
    }
}
