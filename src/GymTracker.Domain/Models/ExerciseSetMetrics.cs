using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Models;
public record ExerciseSetMetrics
{
    public int? Reps { get; init; }
    public decimal? Weight { get; set; }
    public decimal? Time { get; set; }
    public decimal? Distance { get; set; }
}