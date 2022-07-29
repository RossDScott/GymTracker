import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { SessionVM, SetType, WorkoutPlan, ExerciseSetVM, SetWeightMetrics, SetTimeMetrics, ExerciseTargetSet, Exercise, SetMetrics } from './session.model';

@Injectable({
    providedIn: 'root'
})
export class SessionService {
    constructor() {

    }

    fetchSetTypes = () : SetType[] => [
        {name: "Warm-up"},
        {name: "Set"},
        {name: "Drop set"},
        {name: "Failure"},
    ]

    setupFakeSession() {
        let warmupSetType: SetType = {name: "Warm-up"};
        let mainSetType: SetType = {name: "Set"};

        let workoutPlan: WorkoutPlan = {
            id: "1",
            name: "Push Day",
            exercises: [
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
                } as Exercise<SetTimeMetrics>
            ]
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

    
}
