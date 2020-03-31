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

        public Dictionary<Direction, int> Doors;

        public RoomParams(int height, int width, Dictionary<Direction, int> doors = null)
        {
            Height = height;
            Width = width;
            TreesDensity = RandomGenerator.Instance.RandomFloat(0.15F, 0.45F);
            ChestProbability = 0.33F;

            if (doors == null)
            {
                Doors = new Dictionary<Direction, int>
                {
                    { Direction.Left, -1 },
                    { Direction.Right, -1 },
                    { Direction.Top, -1 },
                    { Direction.Bottom, -1},
                };
            }
            else
            {
                Doors = doors;
            }
        }
    }
}
