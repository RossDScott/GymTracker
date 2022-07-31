import { atom } from "jotai";
import { setupFakeSession } from "./session-service";

const session = setupFakeSession();
export const sessionAtom = atom(session);
export const selectedExerciseAtom = atom(session.exercises[0]);
