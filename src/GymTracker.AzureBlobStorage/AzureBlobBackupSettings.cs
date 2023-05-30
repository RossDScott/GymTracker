using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.AzureBlobStorage;
public record AzureBlobBackupSettings
{
    public string? ContainerSASURI { get; init; }
}