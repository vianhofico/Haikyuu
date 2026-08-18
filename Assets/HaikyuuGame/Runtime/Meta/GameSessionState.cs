namespace HaikyuuGame.Meta
{
    public static class GameSessionState
    {
        public static GameMode CurrentMode { get; private set; } = GameMode.QuickMatch;
        public static int SessionRevision { get; private set; }
        public static int TournamentRound { get; set; }

        public static void SelectMode(GameMode mode)
        {
            CurrentMode = mode;
            SessionRevision++;
            if (mode != GameMode.Tournament)
            {
                TournamentRound = 0;
            }
        }
    }
}
