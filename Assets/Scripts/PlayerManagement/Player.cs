using Assets.Scripts.CharactersManagement;

namespace Assets.Scripts.PlayerManagement
{
    public enum PlayerState
    {
        Idle,
        Moving,
    }

    /// <summary>
    /// Represents player in the game with his stats and interactions
    /// </summary>
    public class Player : Character
    {
        public PlayerState State = PlayerState.Idle;

        public void Start()
        {
            DetectOnTile();
        }
    }
}
