using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Abstractions.ClientStorage;
public interface IClientStorage
{
    public KeyItem<bool> HasInitialisedDefaultData { get; set; } = default!;
    public KeyListItem<Exercise> Exercises { get; init; } = default!;
}
