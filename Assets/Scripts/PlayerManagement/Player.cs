using Assets.Scripts.CharactersManagement;

namespace Assets.Scripts.PlayerManagement
{
    public enum PlayerState
    {
        FreeControl,
        Waiting,
        InBattle,
    }

    /// <summary>
    /// Represents player in the game with his stats and interactions
    /// </summary>
    public class Player : Character
    {
        public PlayerState State = PlayerState.FreeControl;

        public int ActionPoints = 3;
        public int ActionPointsMax = 3;

        public void Start()
        {
            DetectOnTile();
        }
    }
}
