import { atom } from "jotai";
import { Duration } from "luxon";
import { setupFakeSession } from "./session-service";

const session = setupFakeSession();
export const sessionAtom = atom(session);
export const selectedExerciseAtom = atom(session.exercises[0]);

export const defaultTimerDuration = Duration.fromMillis(2 * 1000);
export const timerStartDurationAtom = atom<Duration>(defaultTimerDuration);