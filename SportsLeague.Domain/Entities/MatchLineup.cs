using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Entities
{

    public class MatchLineup
    {
        public int Id { get; set; }

        // FK al partido
        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;

        // FK al jugador
        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;

        // true = titular
        // false = suplente
        public bool IsStarter { get; set; }

        // Posición en el partido
        // Ejemplo:
        // GK, CB, ST, CAM
        public string Position { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


    }
}