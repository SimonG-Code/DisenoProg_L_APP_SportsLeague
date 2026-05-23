namespace SportsLeague.API.MatchLineup
{
    public class ReadMatchLineupDto
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int PlayerId { get; set; }

        public bool IsStarter { get; set; }

        public string Position { get; set; } = string.Empty;
    }
}
