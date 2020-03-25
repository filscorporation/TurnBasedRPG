using Assets.Scripts.UIManagement.Tabs;

namespace Assets.Scripts.RewardManagement
{
    /// <summary>
    /// Reward for the player after he won battle
    /// </summary>
    public class Reward
    {
        public int Gold;

        public int Experience;

        public IInventoryObject Item;

        public Reward(int gold, int experience, IInventoryObject item)
        {
            Gold = gold;
            Experience = experience;
            Item = item;
        }

        public override string ToString()
        {
            return $"[Gold: {Gold}, Experience: {Experience}, Item: {Item}]";
        }
    }
}
