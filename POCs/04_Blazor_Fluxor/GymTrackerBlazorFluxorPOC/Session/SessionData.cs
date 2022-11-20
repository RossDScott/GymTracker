//namespace GymTrackerBlazorFluxorPOC.Session;

//public class SessionData
//{
//    public event Action? OnChange;

//    public SessionVM? CurrentSession { get; set; }

//    public int SelectedExerciseIndex { get; private set; } = 0;
//    public void SetSelectedIndex(int value)
//    {
//        SelectedExerciseIndex = value;
//        OnChange?.Invoke();
//    }

//    public ExerciseVM<SetMetrics>? SelectedExercise => CurrentSession?.Exercises?.ElementAt(SelectedExerciseIndex);

//    public void AddExercise(Exercise<SetMetrics> exercise)
//    {
//        var sessionExercise = new ExerciseVM<SetMetrics> { Exercise = exercise, Sets = new List<ExerciseSetVM<SetMetrics>>() };
//        CurrentSession!.Exercises!.Add(sessionExercise);
//    }
//}
