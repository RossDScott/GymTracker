import { DateTime } from "luxon";
import { Exercise, SessionVM, SetTimeMetrics, SetType, SetWeightMetrics, WorkoutPlan } from "./session.model";

export function setupFakeExercises(){
    return [
        { 
            metricType: 'weight',
            name: "Barbell Bench Press", 
            targetSets: [
                {setType: "Warm-up", target: {weight:20, reps: 10} },
                {setType: "Warm-up", target: {weight: 20, reps: 10}},
                {setType: "Set", target: {weight: 55, reps: 8} },
                {setType: "Set", target: {weight: 55, reps: 8}},
                {setType: "Set", target: {weight: 55, reps: 8}}
            ]
        } as Exercise<SetWeightMetrics>,
        { 
            metricType: 'weight',
            name: "Dumbbell Bench Press", 
            targetSets: [
                {setType: "Set", target: {weight: 55, reps: 8}},
                {setType: "Set", target: {weight: 55, reps: 8}},
                {setType: "Set", target: {weight: 55, reps: 8}}
            ]
        } as Exercise<SetWeightMetrics>,
        { 
            metricType: 'weight',
            name: "Triceps Pushdown", 
            targetSets: [
                {setType: "Set", target: {weight: 55, reps: 8}},
                {setType: "Set", target: {weight: 55, reps: 8}},
                {setType: "Set", target: {weight: 55, reps: 8}},
            ]
        } as Exercise<SetWeightMetrics>,
        { 
            metricType: 'time',
            name: "Plank", 
            targetSets: [
                {setType: "Set", target: {timeMilliseconds: 60000}},
                {setType: "Set", target: {timeMilliseconds: 60000}}
            ]
        } as Exercise<SetTimeMetrics>,
    ];
}


export function setupFakeSession(){
    let workoutPlan: WorkoutPlan = {
        id: "1",
        name: "Push Day",
        exercises: setupFakeExercises()
    }

    let session: SessionVM = {
        workoutPlan: workoutPlan,
        workoutStart: DateTime.now(),
        exercises: workoutPlan.exercises.map(workoutExercise => ({
            exercise: workoutExercise,
            sets: workoutExercise.targetSets.map(targetSet => ({
                setType: targetSet.setType,
                targetMetrics: targetSet.target,
                actualMetrics: null,
                completed: false,
                isEditing: false
            })),
            completed: false
        }))
    };

    return session;
}