import { atom } from "jotai";
import { setupFakeSession } from "./shared/session-service";
import SideBar from "./sidebar/session-sidebar";
import './session.css';


const Session = () => {
    return (
        <div className="container-fluid d-flex">
            <SideBar></SideBar>
        </div>
    )
}

export default Session;