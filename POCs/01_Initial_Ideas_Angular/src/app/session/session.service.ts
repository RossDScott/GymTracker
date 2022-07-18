import { Injectable } from '@angular/core';
import { DateTime } from 'luxon';
import { SessionVM, SetType, WorkoutPlan, ExerciseSetVM, SetWeightMetrics, SetTimeMetrics, ExerciseTargetSet, Exercise, SetMetrics } from './session.model';

@Injectable({
    providedIn: 'root'
})
export class SessionService {

    constructor() {

    }

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
                        {setName: "Warm-up 1", target: {weight:20, reps: 10} },
                        {setName: "Warm-up 2", target: {weight: 20, reps: 10}},
                        {setName: "Set 1", target: {weight: 55, reps: 8} },
                        {setName: "Set 2", target: {weight: 55, reps: 8}},
                        {setName: "Set 3", target: {weight: 55, reps: 8}}
                    ]
                } as Exercise<SetWeightMetrics>,
                { 
                    metricType: 'weight',
                    name: "Dumbbell Bench Press", 
                    targetSets: [
                        {setName: "Set 1", target: {weight: 55, reps: 8}},
                        {setName: "Set 2", target: {weight: 55, reps: 8}},
                        {setName: "Set 3", target: {weight: 55, reps: 8}}
                    ]
                } as Exercise<SetWeightMetrics>,
                { 
                    metricType: 'weight',
                    name: "Triceps Pushdown", 
                    targetSets: [
                        {setName: "Set 1", target: {weight: 55, reps: 8}},
                        {setName: "Set 2", target: {weight: 55, reps: 8}},
                        {setName: "Set 3", target: {weight: 55, reps: 8}},
                    ]
                } as Exercise<SetWeightMetrics>,
                { 
                    metricType: 'time',
                    name: "Plank", 
                    targetSets: [
                        {setName: "Set 1", target: {timeMilliseconds: 60000}},
                        {setName: "Set 2", target: {timeMilliseconds: 60000}}
                    ]
                } as Exercise<SetTimeMetrics>
            ]
        }

        let session: SessionVM = {
            workoutPlan: workoutPlan,
            workoutStart: DateTime.now(),
            exercises: workoutPlan.exercises.map(workoutExercise => ({
                type: workoutExercise.targetSets[0].target.type,
                exercise: workoutExercise,
                sets: workoutExercise.targetSets.map(targetSet => ({
                    name: targetSet.setName,
                    targetMetrics: targetSet.target,
                    actualMetrics: Object.assign({}, targetSet) as unknown as SetMetrics,
                    completed: false,
                    isEditing: false
                })),
                completed: false
            }))
        };

        return session;
    }
}
