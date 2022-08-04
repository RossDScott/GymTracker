import WeightSession from './weight-session/weight-session';

import './session-detail.css';
import { SelectedExercise } from '../../shared/session.model';

const SessionDetail: React.FC<SelectedExercise> = ({selectedExerciseAtom}) => {
    return (
        <div id="session" className="me-3 d-flex flex-column">
            <WeightSession selectedExerciseAtom={selectedExerciseAtom}></WeightSession>
            <div id="notes" className="border-top pt-3" >
                <label className="form-label">Notes</label>
                <textarea className="form-control" id="exampleFormControlTextarea1" rows={3}></textarea>
            </div>
        </div>
    )
}

export default SessionDetail;