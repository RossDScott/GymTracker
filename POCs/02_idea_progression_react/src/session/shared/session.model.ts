import { DateTime } from "luxon";

export interface WorkoutPlan{
    id: string;
    name: string;

    exercises: Exercise<SetMetrics>[];
}

export interface Exercise<T extends SetMetrics>{
    metricType: metricType;
    name: string;
    targetSets: ExerciseTargetSet<T>[];
}

export interface ExerciseTargetSet<T extends SetMetrics>{
    setType: string;
    target: T;
}

export interface SetType{
    name: string;
}

export type metricType = "weight" | "time";


export interface SetMetrics{

}

export interface SetWeightMetrics extends SetMetrics{
    weight: number;
    reps: number;
}

export interface SetTimeMetrics extends SetMetrics{
    timeMilliseconds: number;
}

export interface ExerciseSetVM<T extends SetMetrics> {
    setType: string;
    targetMetrics: T;
    actualMetrics: T | null;
    completed: boolean;
    isEditing: boolean;
}

export interface ExerciseVM<T extends SetMetrics>{
    exercise: Exercise<T>;
    completed: boolean;
    
    sets: ExerciseSetVM<T>[];
}

export interface SessionVM{
    workoutStart: DateTime;
    workoutPlan: WorkoutPlan;


    exercises: ExerciseVM<SetMetrics>[];
}