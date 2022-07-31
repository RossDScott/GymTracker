import userEvent from "@testing-library/user-event";
import { atom, useAtom } from "jotai";
import { DateTime, Duration } from "luxon";
import { useEffect, useMemo } from "react";
import { sessionAtom } from "../../shared/session.atoms";

export const workoutStartAtom = atom((get) => get(sessionAtom).workoutStart);
const workoutDurationAtom = atom(Duration.fromMillis(0));

const WorkoutTime = () => {
    const [workoutStart] = useAtom(workoutStartAtom);
    const [workoutDuration, setWorkoutDuration] = useAtom(workoutDurationAtom);

    useEffect(() => {
        const timer = setInterval(() => setWorkoutDuration(DateTime.now().diff(workoutStart, ['hours', 'minutes'])), 1000);
        return () => clearInterval(timer);
    }, [setWorkoutDuration]);

    return (
        <>
            <span className="fw-semibold">Workout Time</span>
            <h1>{workoutDuration.toFormat("hh:mm")}</h1>
            <div className="d-flex">
                <button type="button" className="btn btn-outline-primary btn-sm ms-auto pt-0 pb-0">Pause</button>
            </div>
        </>
    );
}

export default WorkoutTime;