import './session-sidebar.css';
import Exercises from './exercises';
import Timers from './timers/timers';

const SideBar = () => {
    return (
        <div id="sidebar" className="col-md-3 d-md-block collapse pt-2 ">
            <div className="d-flex flex-column">
                <div className="ps-2">
                    <Exercises></Exercises>
                </div>
                <div className="d-grid mt-auto">
                    <button type="button" className="btn btn-outline-primary m-2">End Workout</button>
                </div>
                <Timers></Timers>
                
            </div>
        </div>
    )
}

export default SideBar;