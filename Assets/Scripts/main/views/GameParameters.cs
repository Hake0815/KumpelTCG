namespace gameview
{
    public static class GameParameters
    {
        public const string GAME_LOG_FILE = "action_log.json";
        public static LoadModus LoadModus { get; set; } = LoadModus.NewGame;
    }

    public enum LoadModus
    {
        NewGame,
        ResumeGame,
    }
}
