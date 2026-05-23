using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _matchLineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;

        public MatchLineupService(
            IMatchLineupRepository matchLineupRepository,
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository)
        {
            _matchLineupRepository = matchLineupRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
        }

        public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup)
        {
            // V1 -> El partido debe existir
            var match = await _matchRepository.GetByIdAsync(matchId);

            if (match == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el partido con ID {matchId}");
            }

            // V6 -> Debe estar Scheduled
            if (match.Status != MatchStatus.Scheduled)
            {
                throw new InvalidOperationException(
                    "Solo se pueden registrar alineaciones en partidos Scheduled");
            }

            // V2 -> El jugador debe existir
            var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);

            if (player == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el jugador con ID {lineup.PlayerId}");
            }

            // V3 -> El jugador debe pertenecer al partido
            if (player.TeamId != match.HomeTeamId &&
                player.TeamId != match.AwayTeamId)
            {
                throw new InvalidOperationException(
                    "El jugador no pertenece a ninguno de los equipos del partido");
            }

            // V4 -> No duplicados
            var exists = await _matchLineupRepository
                .ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);

            if (exists)
            {
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");
            }

            // V5 -> Máximo 11 titulares
            if (lineup.IsStarter)
            {
                var starters = await _matchLineupRepository
                    .CountStartersByMatchAndTeamAsync(matchId, player.TeamId);

                if (starters >= 11)
                {
                    throw new InvalidOperationException(
                        "El equipo ya tiene 11 titulares registrados en este partido");
                }
            }

            lineup.MatchId = matchId;

            return await _matchLineupRepository.CreateAsync(lineup);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            return await _matchLineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _matchLineupRepository
                .GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task DeleteAsync(int matchId, int id)
        {
            var lineup = await _matchLineupRepository.GetByIdAsync(id);

            if (lineup == null || lineup.MatchId != matchId)
            {
                throw new KeyNotFoundException(
                    $"No se encontró la alineación con ID {id}");
            }

            await _matchLineupRepository.DeleteAsync(id);
        }
    }
}