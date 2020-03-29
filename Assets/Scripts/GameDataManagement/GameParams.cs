namespace Assets.Scripts.GameDataManagement
{
    /// <summary>
    /// Params passed from main menu for game scene
    /// </summary>
    public static class GameParams
    {
        /// <summary>
        /// Is game new
        /// </summary>
        public static bool NewGame = true;

        /// <summary>
        /// If game is not new, name of the file to load it from
        /// </summary>
        public static string GameFileToLoadName;
    }
}
