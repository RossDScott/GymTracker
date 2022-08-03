import SideBar from "./sidebar/session-sidebar";
import './session.css';
import Main from './main/main';

const Session = () => {
    return (
        <>
            <div className="container-fluid d-flex">
                <SideBar></SideBar>
                <Main></Main>

            </div>
            <div>
                
            </div>
        </>

    )
}

export default Session;