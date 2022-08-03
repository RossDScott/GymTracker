import { atom, useAtom, useAtomValue } from "jotai";
import { selectAtom } from "jotai/utils";
import { DateTime, Duration } from "luxon";
import { useEffect } from "react";
import { sessionAtom } from "../../shared/session.atoms";

export const workoutStartAtom = selectAtom(sessionAtom, session => session.workoutStart);
const workoutDurationAtom = atom(Duration.fromMillis(0));

const WorkoutTime = () => {
    const workoutStart = useAtomValue(workoutStartAtom);
    const [workoutDuration, setWorkoutDuration] = useAtom(workoutDurationAtom);

    useEffect(() => {
        const timer = setInterval(() => setWorkoutDuration(DateTime.now().diff(workoutStart, ['hours', 'minutes'])), 1000);
        return () => clearInterval(timer);
    }, [workoutStart, setWorkoutDuration]);

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