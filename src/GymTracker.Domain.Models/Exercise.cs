using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Models;
public enum MetricType
{
    Weight,
    Time,
    TimeAndDistance
}

public record Exercise
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public MetricType MetricType { get; set; }
    public ICollection<string> BodyTarget { get; set; } = new List<string>();
}