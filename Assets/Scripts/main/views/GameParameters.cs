namespace gameview
{
    public static class GameParameters
    {
        public const string GAME_LOG_FILE = "log.txt";
        public static LoadModus LoadModus { get; set; } = LoadModus.NewGame;
    }

    public enum LoadModus
    {
        NewGame,
        ResumeGame,
        ReplayGame,
    }
}
