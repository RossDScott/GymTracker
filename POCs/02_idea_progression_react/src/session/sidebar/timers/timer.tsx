import { atom, useAtom, useAtomValue } from "jotai";
import { DateTime, Duration } from "luxon";
import React, { useCallback, useEffect, useRef } from "react";
import { animated, useSpring } from '@react-spring/web'
import { defaultTimerDuration, startTimerWithDurationAtom, timerStartDurationAtom } from "../../shared/session.atoms";
import { useResetAtom } from "jotai/utils";
import ControlButtons from "./control-buttons";

const durationAtom = atom<Duration>(defaultTimerDuration);
const startTimeAtom = atom<DateTime | null>(null);
const pausedDurationAtom = atom<Duration | null>(null);
const editModeAtom = atom(false);
const timesUpAtom = atom(false);

const Timer = () => {
    const startTimerWithDuration = useAtomValue(startTimerWithDurationAtom);
    const resetStartTimerWithDuration = useResetAtom(startTimerWithDurationAtom)
    const [startDuration, setStartDuration] = useAtom(timerStartDurationAtom);
    const [duration, setDuration] = useAtom(durationAtom);
    const [startTime, setStartTime] = useAtom(startTimeAtom);
    
    const [pausedDuration, setPausedDuration] = useAtom(pausedDurationAtom);
    const [editMode, setEditMode] = useAtom(editModeAtom);
    const [timesUp, setTimesUp] = useAtom(timesUpAtom);

    useEffect(function calculateRemainingDuration() {
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
                handlePause();
                handleReset();
                setTimesUp(true);
                setInterval(() => setTimesUp(false), 5000);
            }
                
            setDuration(durationCalc);
        }, 1);

        return () => clearInterval(timer);
    }, [startTime, setDuration]);

    useEffect(function triggerStartTimer() {
        if(!startTimerWithDuration)
            return;

        handleReset();
        setStartDuration(startTimerWithDuration);
        handleStart();
        
        resetStartTimerWithDuration();
    }, [startTimerWithDuration]);

    const handlePause = () => {
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
    }

    const showPickerRef = useCallback((node: any) => {
        if (node !== null) {
            node.focus();
            node.showPicker();
        }
      }, []);

    const handleDurationInputChange = (e: React.FormEvent<HTMLInputElement>) => {
        console.log(e.currentTarget.value)
        const duration = Duration.fromISOTime(e.currentTarget.value);
        console.dir(duration);
        setStartDuration(duration);
        setEditMode(false);
    }
    const handleSetTime = () => {
        setEditMode(false);
    }

    const startTimerFace = 
        editMode
            ? <h2><input ref={showPickerRef} type="time" value={startDuration.toFormat("hh:mm:ss")} step="1" min="00:00:01" max="00:59:59" onChange={handleDurationInputChange} onBlur={handleSetTime}></input></h2>
            : <h1 onClick={handleEditTime}>{startDuration.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>

    const timerFace = <h1>{duration.toFormat("mm:ss.SSS") ?? '00:00:000'}</h1>

    const timesUpProps = useSpring({ to: { opacity: 1, color: 'red' }, from: { opacity: 0 }, reset: true, delay: 200, config:{duration:500}, loop: { reverse: true }});

    return (
        <>
            <span className="fw-semibold">Timer</span>
            {!startTime && !timesUp && !pausedDuration && startTimerFace}
            {!timesUp && (startTime || pausedDuration) && timerFace}
            {timesUp && <animated.h1 style={timesUpProps} onClick={handleReset}>Times Up!</animated.h1>}
            <div className="d-flex">
                {!editMode &&
                    <ControlButtons
                        canPause={true}
                        isRunning={!!startTime}
                        isPaused={!!pausedDuration && !startTime}
                        onStart={handleStart}
                        onPause={handlePause}
                        onReset={handleReset}></ControlButtons>}
            </div>
        </>
    );
}
export default Timer;