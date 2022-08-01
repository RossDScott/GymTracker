import { atom, useAtom } from "jotai";
import { setupFakeSession } from "./shared/session-service";
import SideBar from "./sidebar/session-sidebar";
import './session.css';
import { timerStartDurationAtom } from "./shared/session.atoms";
import { Duration } from "luxon";


const Session = () => {
    const [,setTimerDuration] = useAtom(timerStartDurationAtom);
    
    const handleUpdateTimerDuration = () =>
        setTimerDuration(Duration.fromMillis(5000));

    return (
        <>
            <div className="container-fluid d-flex">
                <SideBar></SideBar>
                <div>
                    <button onClick={handleUpdateTimerDuration}>Test change time to 5secs</button>
                </div>
            </div>
            <div>
                
            </div>
        </>

    )
}

export default Session;