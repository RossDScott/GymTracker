import { atom } from "jotai";
import { buildExercises, buildSetTypes } from "./exercises.service";

export const exercisesAtom = atom(buildExercises())
export const setTypesAtom = atom(buildSetTypes());




