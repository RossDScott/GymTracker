import { Type } from "@angular/core";
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
    setName: string;
    target: T;
}

export interface SetType{
    name: string;
}

export type metricType = "weight" | "time";

// export enum SetMetricsType {
//     weight,
//     time
// }

export interface SetMetrics{
    type: metricType;
}

export interface SetWeightMetrics extends SetMetrics{
    type: 'weight';
    weight: number;
    reps: number;
}

export interface SetTimeMetrics extends SetMetrics{
    type: 'time'
    timeMilliseconds: number;
}

export interface ExerciseSetVM<T extends SetMetrics> {
    name: string;
    targetMetrics: T;
    actualMetrics: T;
    completed: boolean;
    isEditing: boolean;
}

export interface ExerciseVM<T extends SetMetrics>{
    type: metricType;
    exercise: Exercise<T>;
    completed: boolean;
    
    sets: ExerciseSetVM<T>[];
}

export interface SessionVM{
    workoutStart: DateTime;
    workoutPlan: WorkoutPlan;


    exercises: ExerciseVM<SetMetrics>[];
}