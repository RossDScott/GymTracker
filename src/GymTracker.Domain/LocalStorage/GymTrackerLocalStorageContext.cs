using GymTracker.Domain.Models;
using GymTracker.BlazorClient.LocalStorage.ContextAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.LocalStorage;
public class GymTrackerLocalStorageContext : LocalStorageContext
{
    public KeyItem<bool> HasInitialisedDefaultData { get; set; } = default!;
    public KeyListItem<Exercise> Exercises { get; init; } = default!;

}