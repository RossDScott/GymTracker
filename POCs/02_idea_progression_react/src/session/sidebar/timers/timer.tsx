import { atom, useAtom, useAtomValue } from "jotai";
import { DateTime, Duration } from "luxon";
import React, { useEffect, useRef } from "react";
import { animated, useSpring } from '@react-spring/web'
import { defaultTimerDuration, startTimerWithDurationAtom, timerStartDurationAtom } from "../../shared/session.atoms";
import { useResetAtom } from "jotai/utils";

const durationAtom = atom<Duration>(defaultTimerDuration);
const startTimeAtom = atom<DateTime | null>(null);
const pausedDurationAtom = atom<Duration | null>(null);
const editModeAtom = atom(false);
const timesUpAtom = atom(false);
const showTimePickerAtom = atom(false);

const Timer = () => {
    const myRefname= useRef(null);

    const startTimerWithDuration = useAtomValue(startTimerWithDurationAtom);
    const resetStartTimerWithDuration = useResetAtom(startTimerWithDurationAtom)
    const [startDuration, setStartDuration] = useAtom(timerStartDurationAtom);
    const [duration, setDuration] = useAtom(durationAtom);
    const [startTime, setStartTime] = useAtom(startTimeAtom);
    
    const [pausedDuration, setPausedDuration] = useAtom(pausedDurationAtom);
    const [editMode, setEditMode] = useAtom(editModeAtom);
    const [timesUp, setTimesUp] = useAtom(timesUpAtom);
    const [showTimePicker, setShowTimePicker] = useAtom(showTimePickerAtom);

    useEffect(() => {
        if(!startTime)
            return;

        const timer = setInterval(()=> {
            if(!startTime)
                return;

            let durationCalc: Duration = startTime!.diff(DateTime.now()).plus(startDuration!);

            if(pausedDuration)
                durationCalc = startTime!.diff(DateTime.now()).plus(pausedDuration!);
            
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

    useEffect(() => {
        if(!startTimerWithDuration)
            return;

        handleReset();
        setStartDuration(startTimerWithDuration);
        handleStart();
        
        resetStartTimerWithDuration();
    }, [startTimerWithDuration]);

    const handleStop = () => {
        setPausedDuration(duration!);
        setStartTime(null);
    }

    const handleStart = () => {
        if(!pausedDuration)
            handleReset();

        setStartTime(DateTime.now());
    }

    const handleReset = () => {
        setStartTime(null);
        setPausedDuration(null);
        setDuration(startDuration);
        setTimesUp(false);
    }

    const handleEditTime = () => {
        setEditMode(true);
        setShowTimePicker(true);
    }
    useEffect(() => {
        if(!showTimePicker)
            return;

        const timeout = setTimeout(() => {
            (myRefname as any)!.current.focus();
            (myRefname as any)!.current.showPicker();
            setShowTimePicker(false);
        }, 50); //This is probably an antipattern, some more learning to do here
        
        return () => clearTimeout(timeout);
    }, [showTimePicker]);

    const handleDurationInputChange = (e: React.FormEvent<HTMLInputElement>) => {
        console.log(e.currentTarget.value)
        const duration = Duration.fromISOTime(e.currentTarget.value);
        console.dir(duration);
        setStartDuration(duration);
    }
    const handleSetTime = () => {
        setEditMode(false);
    }

    const startTimerFace = 
        editMode
            ? <h2><input ref={myRefname} type="time" value={startDuration.toFormat("hh:mm:ss")} step="1" min="00:00:01" max="00:59:59" onChange={handleDurationInputChange} onBlur={handleSetTime}></input></h2>
            : <h1 onClick={handleEditTime}>{startDuration.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>

    const timerFace = <h1>{duration.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>

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

            {!startTime && !timesUp && 
                <button 
                    type="button" 
                    className={`btn btn-outline-primary btn-sm pt-0 pb-0 ${pausedDuration ? 'ms-1' : 'ms-auto'}`}
                    onClick={handleStart}>{pausedDuration ? 'Resume' : 'Start'}</button>
            }
        </>
    )


    const editButton =
        <button  
            type="button" 
            className="btn btn-outline-primary btn-sm ms-auto me-1 pt-0 pb-0"
            onClick={handleSetTime}>Set</button>

    const timesUpProps = useSpring({ to: { opacity: 1, color: 'red' }, from: { opacity: 0 }, reset: true, delay: 200, config:{duration:500}, loop: { reverse: true }});

    return (
        <>
            <span className="fw-semibold">Timer</span>
            {!startTime && !timesUp && !pausedDuration && startTimerFace}
            {!timesUp && (startTime || pausedDuration) && timerFace}
            {timesUp && <animated.h1 style={timesUpProps} onClick={handleReset}>Times Up!</animated.h1>}
            <div className="d-flex">
                {editMode ? editButton : timeControlButtons}
            </div>
        </>
    );
}
export default Timer;