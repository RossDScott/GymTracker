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
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public MetricType MetricType { get; set; }
    public string[] BodyTarget { get; set; } = Array.Empty<string>();
    public string[] Equipment { get; set; } = Array.Empty<string>();
    public bool IsAcitve { get; set; } = true;
}