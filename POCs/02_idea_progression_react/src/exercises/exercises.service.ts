import { Exercise, SetType } from "./exercises.model";

export const buildExercises = (): Exercise[] => [
    { 
        id: 1,
        metricType: 'weight',
        name: "Barbell Bench Press"
    },
    { 
        id: 2,
        metricType: 'weight',
        name: "Dumbbell Bench Press"
    },
    { 
        id: 3,
        metricType: 'weight',
        name: "Triceps Pushdown"
    },
    { 
        id: 4,
        metricType: 'time',
        name: "Plank"
    },
];


export const buildSetTypes = (): SetType[] => [
    {id: 1, name: "Warm-up"},
    {id: 2, name: "Set"},
    {id: 3, name: "Drop set"},
    {id: 4, name: "Failure"},
]