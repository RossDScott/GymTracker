namespace GymTracker.AzureBlobStorage;

public interface ISasValidator
{
    Task<IReadOnlyList<SasValidationStepResult>> ValidateAsync(string sasUri);
}
