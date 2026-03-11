namespace GymTracker.AzureBlobStorage;

public record SasValidationStepResult(
    string StepName,
    bool Success,
    TimeSpan Duration,
    Exception? Exception = null);
