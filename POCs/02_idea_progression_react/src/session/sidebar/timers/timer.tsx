import userEvent from "@testing-library/user-event";
import { atom, useAtom } from "jotai";
import { DateTime, Duration } from "luxon";
import { useEffect, useMemo } from "react";
import { animated, config, useSpring, useTransition } from '@react-spring/web'
import { defaultTimerDuration, sessionAtom, timerStartDurationAtom } from "../../shared/session.atoms";


const durationAtom = atom<Duration>(defaultTimerDuration);
const startTimeAtom = atom<DateTime | null>(null);
const pausedDurationAtom = atom<Duration | null>(null);
const editModeAtom = atom(false);
const timesUpAtom = atom(false);

const Timer = () => {
    const [startDuration, setStartDuration] = useAtom(timerStartDurationAtom);
    const [duration, setDuration] = useAtom(durationAtom);
    const [startTime, setStartTime] = useAtom(startTimeAtom);
    
    const [pausedDuration, setPausedDuration] = useAtom(pausedDurationAtom);
    const [editMode, setEditMode] = useAtom(editModeAtom);
    const [timesUp, setTimesUp] = useAtom(timesUpAtom);

    useEffect(() => {
        const timer = setInterval(()=> {
            if(!startTime)
                return;

            let durationCalc: Duration = startTime!.diff(DateTime.now()).plus(startDuration!);

            if(pausedDuration)
                durationCalc = startTime!.diff(DateTime.now()).plus(startDuration!);
            
            if(durationCalc.toMillis() < 0){
                durationCalc = Duration.fromMillis(0);
                handleStop();
                handleReset();
                setTimesUp(true);
                setInterval(() => setTimesUp(false), 5000);
            }
                
            setDuration(durationCalc);
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
        setDuration(defaultTimerDuration)
        setTimesUp(false);
    }

    const handleEditTime = () => {
        setEditMode(true);
    }

    const timerFace = editMode
                        ? <h2><input type="time" value={duration!.toFormat("mm:ss")} step="1" min="00:00:01" max="00:59:59"></input></h2>
                        : <h1 onClick={handleEditTime}>{duration?.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>

    const timeControlButtons = (
        <>
            {startTime &&
                <button  
                    type="button" 
                    className="btn btn-outline-primary btn-sm ms-auto pt-0 pb-0"
                    onClick={handleStop}>Stop</button>
            }

            {pausedDuration && !startTime &&
                <button  
                    type="button" 
                    className="btn btn-outline-primary btn-sm ms-auto me-1 pt-0 pb-0"
                    onClick={handleReset}>Reset</button>
            }

            {!startTime && 
                <button 
                    type="button" 
                    className={`btn btn-outline-primary btn-sm pt-0 pb-0 ${pausedDuration ? 'ms-1' : 'ms-auto'}`}
                    onClick={handleStart}>{pausedDuration ? 'Resume' : 'Start'}</button>
            }
        </>
    )

    const handleSetTime = () => {
        setEditMode(false);
    }
    const editButton =
        <button  
            type="button" 
            className="btn btn-outline-primary btn-sm ms-auto me-1 pt-0 pb-0"
            onClick={handleSetTime}>Set</button>

    const timesUpProps = useSpring({ to: { opacity: 1, color: 'red' }, from: { opacity: 0 }, reset: true, delay: 200, config:{duration:500}, loop: { reverse: true }});

    return (
        <>
            <span className="fw-semibold">Timer</span>
            {!timesUp && timerFace}
            {timesUp && <animated.h1 style={timesUpProps} onClick={handleReset}>Times Up!</animated.h1>}
            <div className="d-flex">
                {editMode ? editButton : timeControlButtons}
            </div>
        </>
    );
}
export default Timer;