using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Abstractions.Services.ClientStorage;
public interface ILocalStorageContext
{
    IEnumerable<IKeyItem> Keys { get; }
    public IKeyItem<bool> HasInitialisedDefaultData { get; init; }
}
