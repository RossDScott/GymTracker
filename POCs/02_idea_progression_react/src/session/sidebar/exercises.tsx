import './session-sidebar.css';
import { focusAtom } from 'jotai/optics'
import { selectedExerciseAtom, sessionAtom } from '../shared/session.atoms';
import { Atom, PrimitiveAtom, useAtom, WritableAtom } from 'jotai';
import { splitAtom } from "jotai/utils";
import { SessionVM, ExerciseVM, SetMetrics } from '../shared/session.model';

const Exercises = () => {
    const [session] = useAtom(sessionAtom);
    const [selectedExercise, setSelectedExercise] = useAtom(selectedExerciseAtom);
    const exercises = session.exercises;

    const handleSelectExercise = (e: React.MouseEvent, exercise: ExerciseVM<SetMetrics>) => {
        e.preventDefault();
        setSelectedExercise(exercise);
    }

    const items = exercises.map((exercise, index) =>
        <li  
            key={index}
            className={`list-group-item ${selectedExercise === exercise ? 'fw-bold' : ''}`}
            onClick={(e) => handleSelectExercise(e, exercise)}>
            {exercise.exercise.name}
        </li>
    );

    return (
        <ul className="list-group list-group-flush me-1">
            {items}
            <li className="list-group-item"><i className="bi bi-plus-circle-fill fs-3"></i></li>
        </ul>
    )
}

export default Exercises;