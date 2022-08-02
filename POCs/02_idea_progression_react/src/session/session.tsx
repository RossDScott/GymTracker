import { useUpdateAtom } from 'jotai/utils'
import SideBar from "./sidebar/session-sidebar";
import './session.css';
import { startTimerWithDurationAtom, timerStartDurationAtom } from "./shared/session.atoms";
import { Duration } from "luxon";


const Session = () => {
    const setTimerDuration = useUpdateAtom(timerStartDurationAtom);
    const startTimerWithDuration = useUpdateAtom(startTimerWithDurationAtom);

    const handleUpdateTimerDuration = () =>
        setTimerDuration(Duration.fromMillis(5000));

    const handleStartTimerWithDuration = () =>
        startTimerWithDuration(Duration.fromMillis(3000));

    return (
        <>
            <div className="container-fluid d-flex">
                <SideBar></SideBar>
                <div>
                    <button onClick={handleUpdateTimerDuration}>Test change time to 5secs</button>
                    <button onClick={handleStartTimerWithDuration}>Start timer with 3secs</button>
                </div>
            </div>
            <div>
                
            </div>
        </>

    )
}

export default Session;