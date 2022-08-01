import userEvent from "@testing-library/user-event";
import { atom, useAtom } from "jotai";
import { DateTime, Duration } from "luxon";
import { useEffect, useMemo } from "react";
import { sessionAtom } from "../../shared/session.atoms";

const durationAtom = atom<Duration | null>(null);
const startTimeAtom = atom<DateTime | null>(null);
const pausedDurationAtom = atom<Duration | null>(null);

const Stopwatch = () => {
    const [duration, setDuration] = useAtom(durationAtom);
    const [startTime, setStartTime] = useAtom(startTimeAtom);
    const [pausedDuration, setPausedDuration] = useAtom(pausedDurationAtom);

    useEffect(() => {
        const timer = setInterval(()=> {
            if(!startTime)
                return;

            if(pausedDuration){
                setDuration(DateTime.now().plus(pausedDuration).diff(startTime!));
                return;
            }

            setDuration(DateTime.now().diff(startTime!));
        }, 1);

        return () => clearInterval(timer);
    }, [startTime, setDuration]);

    const handleStop = () => {
        setPausedDuration(duration!);
        setStartTime(null);
    }

    const handleStart = () => {
        setStartTime(DateTime.now());
    }

    const handleReset = () => {
        setStartTime(null);
        setPausedDuration(null);
        setDuration(null)
    }

    const stopButton = startTime &&
        <button  
            type="button" 
            className="btn btn-outline-primary btn-sm ms-auto pt-0 pb-0"
            onClick={handleStop}>Stop</button>

    const startButton = !startTime &&
        <button  
            type="button" 
            className={`btn btn-outline-primary btn-sm pt-0 pb-0 ${pausedDuration ? 'ms-1' : 'ms-auto'}`}
            onClick={handleStart}>{pausedDuration ? 'Resume' : 'Start'}</button>

    const resetButton = pausedDuration && !startTime &&
        <button  
            type="button" 
            className="btn btn-outline-primary btn-sm ms-auto me-1 pt-0 pb-0"
            onClick={handleReset}>Reset</button>

    return (
        <>
            <span className="fw-semibold">Stopwatch</span>
            <h1>{duration?.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>
            <div className="d-flex">
                {stopButton}
                {resetButton}
                {startButton}
            </div>
        </>
    );
}
export default Stopwatch;