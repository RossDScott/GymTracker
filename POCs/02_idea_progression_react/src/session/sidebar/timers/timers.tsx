import { atom } from "jotai";
import { DateTime } from "luxon";
import CurrentTime from "./current-time";
import Stopwatch from "./stopwatch";
import Timer from "./timer";
import WorkoutTime from "./workout-time";

const clockAtom = atom(DateTime.now());

const Timers = () => {
    return (
        <div className="d-flex flex-column mb-3 timers">
            <hr className="mb-3 mt-0" />
            <div className="d-flex justify-content-center">
                <div style={{width: '12rem'}}>
                    <CurrentTime></CurrentTime>
                </div>
            </div>
            <div className="d-flex justify-content-center">
                <div style={{width: '12rem'}}>
                    <WorkoutTime></WorkoutTime>
                </div>
            </div>
            <div className="d-flex justify-content-center">
                <div style={{width: '12rem'}}>
                    <Stopwatch></Stopwatch>
                </div>
            </div>
            <div className="d-flex justify-content-center">
                <div style={{width: '12rem'}}>
                    <Timer></Timer>
                </div>
            </div>
        </div>

    )
}

export default Timers;