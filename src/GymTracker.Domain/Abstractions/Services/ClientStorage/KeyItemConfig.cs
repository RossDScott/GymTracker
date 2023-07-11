using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public record KeyConfig<T>
{
    public bool AutoBackup { get; set; } = true;
    public Func<T>? DefaultConstructor { get; set; } = null;
    public bool CacheDataa { get; set; } = true;
}