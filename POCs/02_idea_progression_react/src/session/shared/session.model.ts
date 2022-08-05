import { Atom, WritableAtom } from "jotai";
import { DateTime } from "luxon";
import { MetricType } from "../../exercises/exercises.model";

export interface WorkoutPlan{
    id: string;
    name: string;

    exercises: Exercise<SetMetrics>[];
}

export interface Exercise<T extends SetMetrics>{
    metricType: MetricType;
    name: string;
    targetSets: ExerciseTargetSet<T>[];
}

export interface ExerciseTargetSet<T extends SetMetrics>{
    setType: string;
    target: T;
}

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


export type SelectedExercise = {
    selectedExerciseAtom: WritableAtom<ExerciseVM<SetMetrics>, ExerciseVM<SetMetrics>, void>;
}