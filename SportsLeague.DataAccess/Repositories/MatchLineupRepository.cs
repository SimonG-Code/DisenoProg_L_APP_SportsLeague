using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchLineupRepository : IMatchLineupRepository
    {
        private readonly LeagueDbContext _context;

        public MatchLineupRepository(LeagueDbContext context)
        {
            _context = context;
        }

        public async Task<MatchLineup> CreateAsync(MatchLineup lineup)
        {
            lineup.CreatedAt = DateTime.UtcNow;

            _context.MatchLineups.Add(lineup);

            await _context.SaveChangesAsync();

            return await _context.MatchLineups
                .Include(x => x.Player)
                    .ThenInclude(p => p.Team)
                .FirstAsync(x => x.Id == lineup.Id);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            return await _context.MatchLineups
                .Include(x => x.Player)
                    .ThenInclude(p => p.Team)
                .Where(x => x.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _context.MatchLineups
                .Include(x => x.Player)
                    .ThenInclude(p => p.Team)
                .Where(x =>
                    x.MatchId == matchId &&
                    x.Player.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId)
        {
            return await _context.MatchLineups
                .AnyAsync(x =>
                    x.MatchId == matchId &&
                    x.PlayerId == playerId);
        }

        public async Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _context.MatchLineups
                .CountAsync(x =>
                    x.MatchId == matchId &&
                    x.Player.TeamId == teamId &&
                    x.IsStarter);
        }

        public async Task<MatchLineup?> GetByIdAsync(int id)
        {
            return await _context.MatchLineups
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var lineup = await _context.MatchLineups.FindAsync(id);

            if (lineup != null)
            {
                _context.MatchLineups.Remove(lineup);

                await _context.SaveChangesAsync();
            }
        }
    }
}