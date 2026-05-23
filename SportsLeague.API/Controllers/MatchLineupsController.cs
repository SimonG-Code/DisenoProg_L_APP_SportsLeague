using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsLeague.API.DTOs;
using SportsLeague.API.MatchLineup;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchLineupsController : ControllerBase
    {
        private readonly LeagueDbContext _context;
        private readonly IMapper _mapper;

        public MatchLineupsController(
            LeagueDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/matchlineups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadMatchLineupDto>>> GetAll()
        {
            var lineups = await _context.MatchLineups.ToListAsync();

            var lineupDtos = _mapper.Map<List<ReadMatchLineupDto>>(lineups);

            return Ok(lineupDtos);
        }

        // GET: api/matchlineups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadMatchLineupDto>> GetById(int id)
        {
            var lineup = await _context.MatchLineups
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lineup == null)
            {
                return NotFound();
            }

            var lineupDto = _mapper.Map<ReadMatchLineupDto>(lineup);

            return Ok(lineupDto);
        }
    }
}