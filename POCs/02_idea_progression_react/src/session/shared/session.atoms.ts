import { Atom, atom, PrimitiveAtom, useAtom, useAtomValue, WritableAtom } from "jotai";
import { atomWithReset, splitAtom } from "jotai/utils";
import { focusAtom } from 'jotai/optics'

import { Duration } from "luxon";
import { setupFakeSession } from "./session-service";
import { useMemo } from "react";
import { ExerciseVM, SetMetrics } from "./session.model";

const session =  setupFakeSession();

export const sessionAtom = atom(session);
export const selectedExerciseIndexAtom = atom(0);

export const exercisesAtom = focusAtom(sessionAtom, (optic) => optic.prop('exercises'));
export const exercisesAtomAtoms = splitAtom(exercisesAtom);

export const defaultTimerDuration = Duration.fromMillis(10 * 1000);
export const timerStartDurationAtom = atom<Duration>(defaultTimerDuration);
export const startTimerWithDurationAtom = atomWithReset<Duration | null>(null);

export const selectedSetRowIndexAtom = atomWithReset<number | null>(null);