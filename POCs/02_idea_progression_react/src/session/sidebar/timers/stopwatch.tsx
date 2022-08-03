import { atom, useAtom } from "jotai";
import { DateTime, Duration } from "luxon";
import { useEffect } from "react";
import ControlButtons from "./control-buttons";

const durationAtom = atom<Duration | null>(null);
const startTimeAtom = atom<DateTime | null>(null);
const pausedDurationAtom = atom<Duration | null>(null);

const Stopwatch = () => {
    const [duration, setDuration] = useAtom(durationAtom);
    const [startTime, setStartTime] = useAtom(startTimeAtom);
    const [pausedDuration, setPausedDuration] = useAtom(pausedDurationAtom);

    useEffect(() => {
        if(!startTime)
            return;

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

    const handlePause = () => {
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

    return (
        <>
            <span className="fw-semibold">Stopwatch</span>
            <h1>{duration?.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>
            <div className="d-flex">
                <ControlButtons
                        canPause={true}
                        isRunning={!!startTime}
                        isPaused={!!pausedDuration && !startTime}
                        onStart={handleStart}
                        onPause={handlePause}
                        onReset={handleReset}></ControlButtons>
            </div>
        </>
    );
}
export default Stopwatch;