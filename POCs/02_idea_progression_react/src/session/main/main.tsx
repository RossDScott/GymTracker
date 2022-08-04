import { useAtomValue } from 'jotai';
import SessionDetail from './session-detail/session-detail';
import './main.css';

import { SelectedExercise } from '../shared/session.model';

const Main: React.FC<SelectedExercise> = ({selectedExerciseAtom}) => {
    const selectedExercise = useAtomValue(selectedExerciseAtom);

    return (
        <main className="ms-3 d-flex flex-column">
            <div id="title" className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                <h1 className="h2">29/06/22 - Push Day - {selectedExercise.exercise.name}</h1>
                <div className="btn-toolbar mb-2 mb-md-0">
                    <div className="btn-group me-2">
                        <i className="bi bi-list fs-1"></i>
                    </div>
                </div>
            </div>
            <div id="mainContent" className="d-flex">
                <SessionDetail selectedExerciseAtom={selectedExerciseAtom}></SessionDetail>
                <div id="sessionSidebar" className="border-start">
                    <ul className="nav nav-tabs">
                        <li className="nav-item">
                        <a className="nav-link " aria-current="page" href="#">History</a>
                        </li>
                        <li className="nav-item">
                        <a className="nav-link active">Charts</a>
                        </li>
                        <li className="nav-item">
                        <a className="nav-link">Learning</a>
                        </li>
                    </ul>
                    <div>
                        <canvas id="myChart"></canvas>
                    </div>
                </div>
            </div>
        </main>
    )
}

export default Main;