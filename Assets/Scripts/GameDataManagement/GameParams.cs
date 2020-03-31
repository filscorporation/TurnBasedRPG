using Assets.Scripts.MapManagement;

namespace Assets.Scripts.GameDataManagement
{
    public enum GameMode
    {
        /// <summary>
        /// New game
        /// </summary>
        New,

        /// <summary>
        /// Game loaded from a save file
        /// </summary>
        Loaded,

        /// <summary>
        /// Game created from new room
        /// </summary>
        NextNewRoom,

        /// <summary>
        /// Game created from existing room
        /// </summary>
        NextExistingRoom,
    }

    /// <summary>
    /// Params passed from main menu or current room to next room
    /// </summary>
    public static class GameParams
    {
        /// <summary>
        /// Game mode
        /// </summary>
        public static GameMode GameMode;

        /// <summary>
        /// If game is not new, name of the file to load it from
        /// </summary>
        public static string GameFileToLoadName;

        /// <summary>
        /// Index of the room that we are entering
        /// </summary>
        public static int CurrentRoomIndex;

        /// <summary>
        /// Index of the room we just leaved to enter next one.
        /// Used to link entrance
        /// </summary>
        public static int LeavedRoomIndex;

        /// <summary>
        /// Direction where in room player is spawned and entrance gets link back
        /// </summary>
        public static Direction SpawnDirection;
    }
}
