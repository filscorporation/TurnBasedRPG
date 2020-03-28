using Assets.Scripts.RewardManagement;
using System.Collections.Generic;

namespace Assets.Scripts.MapManagement
{
    public enum Direction
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3,
    }

    public class RoomParams
    {
        public int Height;

        public int Width;

        public float TreesDensity;

        public float ChestProbability;

        public List<Direction> Doors;

        public RoomParams(int height, int width)
        {
            Height = height;
            Width = width;
            TreesDensity = RandomGenerator.Instance.RandomFloat(0.15F, 0.45F);
            ChestProbability = 0.33F;
            Doors = new List<Direction> { Direction.Left, Direction.Right, Direction.Top, Direction.Bottom };
        }
    }
}
