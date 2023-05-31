using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Domain.Models;
public record AppSettings
{
    public string? AzureBlobBackupContainerSASURI { get; init; } = null;
}
