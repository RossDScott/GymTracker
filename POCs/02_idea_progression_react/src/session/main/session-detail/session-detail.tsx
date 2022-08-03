import { useAtomValue } from 'jotai';
import { selectedExerciseAtom } from '../../shared/session.atoms';
import WeightSession from './weight-session/weight-session';

import './session-detail.css';

const SessionDetail = () => {
    const selectedExercise = useAtomValue(selectedExerciseAtom);
    
    return (
        <div id="session" className="me-3 d-flex flex-column">
            <WeightSession></WeightSession>
            <div id="notes" className="border-top pt-3" >
                <label className="form-label">Notes</label>
                <textarea className="form-control" id="exampleFormControlTextarea1" rows={3}></textarea>
            </div>
        </div>
    )
}

export default SessionDetail;